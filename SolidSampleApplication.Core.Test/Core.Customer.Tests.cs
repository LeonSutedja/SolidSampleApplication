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
            evt.CurrentVersion.ShouldBe(0);
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
    }
}