using MassTransit;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using Quartz;
using Shouldly;
using SolidSampleApplication.Api.Membership;
using SolidSampleApplication.Infrastructure;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace SolidSampleApplication.Api.Test
{
    public class XunitLogger<T> : ILogger<T>, IDisposable
    {
        private ITestOutputHelper _output;

        public XunitLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _output.WriteLine(state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }
    }

    public class SagaSampleStateMachineTests : IClassFixture<DefaultWebHostTestFixture>, IDisposable
    {
        private readonly DefaultWebHostTestFixture _fixture;
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly InMemoryTestHarness _harness;
        private readonly StateMachineSagaTestHarness<SagaSampleInstanceState, SagaSampleStateMachine> _stateMachineSagaTestHarness;
        private TimeSpan _testOffset;

        private void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            configurator.UseInMemoryScheduler("scheduler");
        }

        public SagaSampleStateMachineTests(DefaultWebHostTestFixture fixture, ITestOutputHelper output)
        {
            _testOffset = TimeSpan.Zero;
            _harness = new InMemoryTestHarness();
            _harness.OnConfigureInMemoryBus += ConfigureInMemoryBus;
            var machine = new SagaSampleStateMachine();
            _stateMachineSagaTestHarness = _harness.StateMachineSaga<SagaSampleInstanceState, SagaSampleStateMachine>(machine);
            _harness.Start().Wait();
            fixture.Output = output;
            _client = fixture.CreateClient();
            _fixture = fixture;
            _output = output;

            var inputQueueAddress = _harness.InputQueueAddress;
            var requestClient = _harness.CreateRequestClient<SagaStatusRequestedEvent>();
            MockedObjects.SagaStatusRequestClient.Setup(e => e.GetResponse<SagaStatus, SagaStatusNotFound>(It.IsAny<SagaStatusRequestedEvent>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
                .Returns((SagaStatusRequestedEvent o, CancellationToken token, RequestTimeout timeout) =>
                {
                    return requestClient.GetResponse<SagaStatus, SagaStatusNotFound>(o);
                });

            MockedObjects.IPublishEndpoint.Setup(e => e.Publish(It.IsAny<SagaStartedEvent>(), It.IsAny<CancellationToken>()))
                .Returns((SagaStartedEvent o, CancellationToken token) =>
                {
                    return _harness.Bus.Publish(o);
                });
        }

        public void Dispose()
        {
            _fixture.Output = null;
            _harness.Stop().Wait();
        }

        private void ActionJsonStringList(string jsonList, Action<dynamic, int> action)
        {
            dynamic jsonDynamicList = JValue.Parse(jsonList);
            var count = 0;
            foreach (var jsonItem in jsonDynamicList)
            {
                action(jsonItem, count);
                count++;
            }
        }

        [Fact]
        public async Task StartSaga_ShouldReturn_Ok()
        {
            // arrange
            var request = new StartSagaSampleCommand()
            {
                ItemId = 1,
                TextRequest = "Test"
            };

            var response = await _client.PostRequestAsStringContent("/SagaSample", request);
            response.EnsureSuccessStatusCode();

            MockedObjects.IPublishEndpoint.Verify(x => x.Publish(It.IsAny<SagaStartedEvent>(), It.IsAny<CancellationToken>()));
            (await _harness.Published.Any<SagaStartedEvent>()).ShouldBeTrue();
            _stateMachineSagaTestHarness.Consumed.Select<SagaStartedEvent>().Any().ShouldBeTrue();
        }

        [Fact]
        public async Task CheckStatusSagaRequest_ShouldReturn_Ok()
        {
            // arrange
            var id = 2;
            await _harness.Bus.Publish(new SagaStartedEvent { ItemId = id, Text = "test", Timestamp = DateTime.UtcNow });
            _harness.Published.Select<SagaStartedEvent>().Any().ShouldBeTrue();

            // act
            var response = await _client.GetAsync($"/SagaSample/{id}");

            var content = await response.Content.ReadAsStringAsync();
            content.ShouldNotBeEmpty();
            _output.WriteLine(content);

            //assert
            response.EnsureSuccessStatusCode();
            _stateMachineSagaTestHarness.Consumed.Select<SagaStatusRequestedEvent>().Any().ShouldBeTrue();
        }

        [Fact]
        public async Task StartSaga_ShouldStartSchedule_ReturnOk()
        {
            // arrange
            var id = 3;
            await _harness.Bus.Publish(new SagaStartedEvent { ItemId = id, Text = "test", Timestamp = DateTime.UtcNow });
            _harness.Published.Select<SagaStartedEvent>().Any().ShouldBeTrue();
            _stateMachineSagaTestHarness.Consumed.Select<SagaStartedEvent>().Any().ShouldBeTrue();
            (await _stateMachineSagaTestHarness.Created.Any(instance => instance.ItemId == id)).ShouldBeTrue();

            await Task.Delay(15000);

            var instance = _stateMachineSagaTestHarness.Sagas.Select(i => i.ItemId == id).First();
            _output.WriteLine(instance.Saga.ToJson());
        }
    }
}