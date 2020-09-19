using Automatonymous;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    [ApiController]
    [Route("[controller]")]
    public class SagaSampleController : Controller
    {
        private readonly IApplicationBusService _bus;

        public SagaSampleController(IApplicationBusService bus)
        {
            _bus = bus;
        }

        [HttpPost]
        public async Task<ActionResult> StartSaga(StartSagaSampleCommand request)
        {
            return (await _bus.Send(request)).ActionResult;
        }

        //[HttpPost("/ChangeText")]
        //public async Task<ActionResult> ChangeSagaText(ChangeSagaTextCommand request)
        //{
        //    return (await _bus.Send(request)).ActionResult;
        //}

        //[HttpPost("/Reset")]
        //public async Task<ActionResult> Reset(ResetSagaCommand request)
        //{
        //    return (await _bus.Send(request)).ActionResult;
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult> SagaSample(int id)
        {
            var command = new CheckSagaCommand() { ItemId = id };
            return (await _bus.Send(command)).ActionResult;
        }
    }

    public class ResetSagaCommand : ICommand<DefaultResponse>
    {
        public int ItemId { get; set; }
    }

    public class StartSagaSampleCommand : ICommand<DefaultResponse>
    {
        public int ItemId { get; set; }
        public string TextRequest { get; set; }
    }

    public class ChangeSagaTextCommand : ICommand<DefaultResponse>
    {
        public int ItemId { get; set; }
        public string TextChangedTo { get; set; }
    }

    public class CheckSagaCommand : ICommand<DefaultResponse>
    {
        public int ItemId { get; set; }
    }

    public class SagaSampleCommandHandler :
        ICommandHandler<StartSagaSampleCommand, DefaultResponse>,
        ICommandHandler<CheckSagaCommand, DefaultResponse>,
        ICommandHandler<ChangeSagaTextCommand, DefaultResponse>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<SagaStatusRequestedEvent> _sagaStatusRequestClient;

        public SagaSampleCommandHandler(
            IPublishEndpoint publishEndpoint,
            IRequestClient<SagaStatusRequestedEvent> sagaStatusRequestClient)
        {
            this._publishEndpoint = publishEndpoint;
            this._sagaStatusRequestClient = sagaStatusRequestClient;
        }

        public async Task<DefaultResponse> Handle(StartSagaSampleCommand request, CancellationToken cancellationToken)
        {
            var @event = new SagaStartedEvent()
            {
                ItemId = request.ItemId,
                Timestamp = DateTime.Now,
                Text = request.TextRequest
            };
            await _publishEndpoint.Publish(@event);
            return DefaultResponse.Success();
        }

        public async Task<DefaultResponse> Handle(CheckSagaCommand request, CancellationToken cancellationToken)
        {
            var (status, notfound) = await _sagaStatusRequestClient.GetResponse<SagaStatus, SagaStatusNotFound>(
                new SagaStatusRequestedEvent { ItemId = request.ItemId });
            if(status.IsCompletedSuccessfully)
            {
                var response = await status;
                return DefaultResponse.Success(response);
            }
            else
            {
                var response = await notfound;
                return DefaultResponse.Failed(response);
            }
        }

        public async Task<DefaultResponse> Handle(ChangeSagaTextCommand request, CancellationToken cancellationToken)
        {
            var (status, notfound) = await _sagaStatusRequestClient.GetResponse<SagaStatus, SagaStatusNotFound>(
                new SagaTextChangedEvent { ItemId = request.ItemId, TextChangedTo = request.TextChangedTo });
            if(status.IsCompletedSuccessfully)
            {
                var response = await status;
                return DefaultResponse.Success(response);
            }
            else
            {
                var response = await notfound;
                return DefaultResponse.Failed(response);
            }
        }
    }

    public class SagaStartedEvent
    {
        public int ItemId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
    }

    public class SagaStatusRequestedEvent
    {
        public int ItemId { get; set; }
    }

    public class SagaTextChangedEvent
    {
        public int ItemId { get; set; }
        public string TextChangedTo { get; set; }
    }

    public class SagaStatus
    {
        public int ItemId { get; set; }
        public string TextStored { get; set; }
        public string CurrentState { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ElapsedTime { get; set; }
    }

    public class SagaStatusNotFound
    {
        public int ItemId { get; set; }
    }

    public class SagaSampleInstanceState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public int ItemId { get; set; }
        public string CurrentState { get; set; }
        public string Text { get; set; }
        public DateTime LastUpdated { get; set; }
        public Guid? ElapsedTokenId { get; set; }
    }

    public class SagaSampleStateMachine
        : MassTransitStateMachine<SagaSampleInstanceState>
    {
        // Pretend the command as an event
        public SagaSampleStateMachine()
        {
            // Correlates between the ids
            Event(() => SagaStarted, x =>
            {
                x.SelectId(x => NewId.NextGuid());
                //x.CorrelateBy<int>(x => x.ItemId, x => x.Message.ItemId);
                x.CorrelateById<int>(h => h.ItemId, z => z.Message.ItemId);
            });
            Event(() => SagaStatusRequested, x =>
            {
                //x.CorrelateBy<int>(x => x.ItemId, x => x.Message.ItemId);
                x.CorrelateById<int>(h => h.ItemId, z => z.Message.ItemId);
                x.OnMissingInstance(n =>
                {
                    return n.ExecuteAsync(r =>
                    {
                        return r.RespondAsync(new SagaStatusNotFound
                        { ItemId = r.Message.ItemId });
                    });
                });
            });

            Schedule(() => Elapsed15Seconds, instance => instance.ElapsedTokenId, s =>
            {
                s.Delay = TimeSpan.FromSeconds(15);
                s.Received = r => r.CorrelateById(y => y.ItemId, y => y.Message.ItemId);
            });
            Schedule(() => Elapsed30Seconds, instance => instance.ElapsedTokenId, s =>
            {
                s.Delay = TimeSpan.FromSeconds(15);
                s.Received = r => r.CorrelateById(y => y.ItemId, y => y.Message.ItemId);
            });

            InstanceState(x => x.CurrentState);

            Initially(
                When(SagaStarted)
                .Then(ctx =>
                {
                    ctx.Instance.ItemId = ctx.Data.ItemId;
                    ctx.Instance.Text = ctx.Data.Text;
                    ctx.Instance.LastUpdated = DateTime.UtcNow;
                })
                .Schedule(Elapsed15Seconds,
                        context => context.Init<Elapsed15SecondsEvent>(new Elapsed15SecondsEvent { ItemId = context.Instance.ItemId }))
                .TransitionTo(Started));

            During(Started,
                Ignore(SagaStarted));

            DuringAny(
                When(SagaStatusRequested)
                    .RespondAsync(x =>
                    {
                        return x.Init<SagaStatus>(new
                        {
                            x.Instance.ItemId,
                            TextStored = x.Instance.Text,
                            x.Instance.CurrentState,
                            x.Instance.LastUpdated,
                            ElapsedTime = $"{DateTime.UtcNow.Subtract(x.Instance.LastUpdated).TotalSeconds} Seconds"
                        });
                    }
                    ));

            During(Started,
                When(Elapsed15Seconds.Received)
                .Schedule(Elapsed30Seconds,
                        context => context.Init<Elapsed30SecondsEvent>(new Elapsed30SecondsEvent { ItemId = context.Instance.ItemId }))
                .TransitionTo(Elapsed15SecondsState));

            During(Elapsed15SecondsState,
                When(Elapsed30Seconds.Received)
                .TransitionTo(Completed));
        }

        public State Started { get; private set; }
        public State Elapsed15SecondsState { get; private set; }
        public State Completed { get; private set; }

        public Event<SagaStartedEvent> SagaStarted { get; private set; }
        public Event<SagaStatusRequestedEvent> SagaStatusRequested { get; private set; }
        public Schedule<SagaSampleInstanceState, Elapsed15SecondsEvent> Elapsed15Seconds { get; private set; }
        public Schedule<SagaSampleInstanceState, Elapsed30SecondsEvent> Elapsed30Seconds { get; private set; }
    }

    public class Elapsed15SecondsEvent
    {
        public int ItemId { get; set; }
    }

    public class Elapsed30SecondsEvent
    {
        public int ItemId { get; set; }
    }
}