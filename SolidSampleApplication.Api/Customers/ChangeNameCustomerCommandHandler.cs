using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Core.Services.CustomerServices;
using SolidSampleApplication.Infrastructure.ApplicationBus;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class ChangeNameCustomerCommand : ICommand<DefaultResponse>
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

    public class ChangeNameCustomerCommandHandler : ICommandHandler<ChangeNameCustomerCommand, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly ICustomerDomainService _service;

        public ChangeNameCustomerCommandHandler(ReadModelDbContext readModelDbContext, ICustomerDomainService service)
        {
            _readModelDbContext = readModelDbContext;
            _service = service;
        }

        public async Task<DefaultResponse> Handle(ChangeNameCustomerCommand request, CancellationToken cancellationToken)
        {
            await _service.ChangeCustomerNameAsync(request.CustomerId, request.FirstName, request.LastName);

            var customer = await _readModelDbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            return DefaultResponse.Success(customer);
        }
    }
}