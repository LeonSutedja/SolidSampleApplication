using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class ChangeNameCustomerCommand : IRequest<DefaultResponse>
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ChangeNameCustomerCommandValidator : AbstractValidator<ChangeNameCustomerCommand>
    {
        public ChangeNameCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotNull();
            RuleFor(x => x.FirstName)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(50);
            RuleFor(x => x.LastName)
                .NotNull()
                .MinimumLength(3)
                .MaximumLength(50);
        }
    }

    public class ChangeNameCustomerCommandHandler : IRequestHandler<ChangeNameCustomerCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMediator _mediator;

        public ChangeNameCustomerCommandHandler(ReadModelDbContext readModelDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _mediator = mediator;
        }

        public async Task<DefaultResponse> Handle(ChangeNameCustomerCommand request, CancellationToken cancellationToken)
        {
            var @event = new CustomerNameChangedEvent(request.CustomerId, request.FirstName, request.LastName);
            await _mediator.Publish(@event);

            var customer = await _readModelDbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            return DefaultResponse.Success(customer);
        }
    }
}