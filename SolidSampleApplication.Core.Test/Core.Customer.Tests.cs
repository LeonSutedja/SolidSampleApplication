using Pressius;
using Shouldly;
using System.Collections.Generic;
using Xunit;
using SolidSampleApplication.Api.Customers;
using System.Linq;
using System;

namespace SolidSampleApplication.Core.Test
{
    public class CoreCustomerTests
    {
        public static IEnumerable<object[]> ValidNames()
        {
            var permutor = new Permutor();
            var pressiusInputs = permutor
                .AddParameterDefinition("FirstName", new List<object> { "batman", "four", "1234567890", "~`@#$%^&*()_+{}:><?,./;[]=-'" }, true)
                .AddParameterDefinition("LastName", new List<object> { "batman", "four", "1234567890", "~`@#$%^&*()_+{}:><?,./;[]=-'" }, true)
                .AddParameterDefinition("Email", new List<object> { "batman@bat.com" }, true)
                .AddParameterDefinition("Username", new List<object> { "batman", "four", "1234567890", "~`@#$%^&*()_+{}:><?,./;[]=-'" }, true)
                .GeneratePermutation<RegisterCustomerCommand>();
            return pressiusInputs.Select(i => new object[] { i }).ToList();
        }

        [Theory]
        [MemberData(nameof(ValidNames))]
        public void CustomerRegistration_Should_CreateCustomer_And_CustomerRegisteredEvent(RegisterCustomerCommand cmd)
        {
            var c = Customer.Registration(cmd.Username, cmd.FirstName, cmd.LastName, cmd.Email);
            c.ShouldBeOfType<Customer>();
            c.PendingEvents.Count.ShouldBe(1);
            var evt = c.PendingEvents.Dequeue();
            evt.ShouldBeOfType<CustomerRegisteredEvent>();

            var evtAsType = (CustomerRegisteredEvent)evt;
            evtAsType.Username.ShouldBe(cmd.Username);
            evtAsType.FirstName.ShouldBe(cmd.FirstName);
            evtAsType.LastName.ShouldBe(cmd.LastName);
            evtAsType.Email.ShouldBe(cmd.Email);
            evtAsType.CurrentVersion.ShouldBe(0);
            evtAsType.Id.ShouldBe(c.Id);
        }

        public static IEnumerable<object[]> InValidNames()
        {
            var permutor = new Permutor();
            var pressiusInputs = permutor
                .AddParameterDefinition("FirstName", new List<object> { "batman", null }, true)
                .AddParameterDefinition("LastName", new List<object> { null, "robin" }, true)
                .AddParameterDefinition("Email", new List<object> { "batman@bat.com", null }, true)
                .AddParameterDefinition("Username", new List<object> { null, "batmanrobin" }, true)
                .GeneratePermutation<RegisterCustomerCommand>();
            return pressiusInputs.Select(i => new object[] { i }).ToList();
        }

        [Theory]
        [MemberData(nameof(InValidNames))]
        public void CustomerRegistration_ShouldNot_CreateCustomer_WithNullValues(RegisterCustomerCommand cmd)
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                var c = Customer.Registration(null, cmd.FirstName, cmd.LastName, cmd.Email);
            });
        }

        [Fact]
        public void CustomerChangeName_Should_ChangeName_And_CustomerNameChangedEvent()
        {
            // setup
            var firstname = "Bruce";
            var lastname = "Wayne";
            var c = Customer.Registration("username", "Mary", "Poppin", "mary@poppin.com.au");
            // act
            c.ChangeName(firstname, lastname);

            // assert
            c.PendingEvents.Count.ShouldBe(2);

            // remove the first event first
            c.PendingEvents.Dequeue();

            // assert
            var evt = c.PendingEvents.Dequeue();
            evt.ShouldBeOfType<CustomerNameChangedEvent>();
            var evtAsType = (CustomerNameChangedEvent)evt;
            evtAsType.FirstName.ShouldBe(firstname);
            evtAsType.LastName.ShouldBe(lastname);
            evtAsType.CurrentVersion.ShouldBe(1);
            evtAsType.Id.ShouldBe(c.Id);
        }

        [Theory]
        [InlineData(null, "Wayne")]
        [InlineData("Bruce", null)]
        public void CustomerChangeName_ShouldNot_ChangeName_With_InvalidNames(string firstname, string lastname)
        {
            // setup
            var c = Customer.Registration("username", "Mary", "Poppin", "mary@poppin.com.au");
            // remove the first event first
            c.PendingEvents.Dequeue();

            // act
            Should.Throw<ArgumentNullException>(() =>
            {
                c.ChangeName(firstname, lastname);
            });

            c.PendingEvents.Count.ShouldBe(0);
        }
    }
}