using FluentValidation;
using MediatR;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class RegisterCustomerCommand : IRequest<DefaultResponse>
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
    {
        public RegisterCustomerCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(20);
            RuleFor(x => x.FirstName)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(50);
            RuleFor(x => x.LastName)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(50);
            RuleFor(x => x.Email)
                .NotNull()
                .EmailAddress();
        }
    }

    public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _context;
        private readonly IMediator _mediator;

        public RegisterCustomerCommandHandler(ReadModelDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<DefaultResponse> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {
            // pretend to run some sort of validation here.
            // username must be unique.
            var isUsernameExists = _context.Customers.Any(c => c.Username == request.Username);
            if(isUsernameExists)
                return DefaultResponse.Failed(request, "Username exists");

            // success
            var customerRegisteredEvent = new CustomerRegisteredEvent(Guid.NewGuid(), request.Username, request.FirstName, request.LastName, request.Email);
            var membershipEvent = new MembershipCreatedEvent(Guid.NewGuid(), customerRegisteredEvent.Id);

            await _mediator.Publish(customerRegisteredEvent);
            await _mediator.Publish(membershipEvent);

            var customer = _context.Customers.FirstOrDefault(c => c.Username == request.Username);
            return DefaultResponse.Success(customer);
        }
    }
}