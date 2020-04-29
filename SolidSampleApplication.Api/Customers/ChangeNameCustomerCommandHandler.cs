﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastructure.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;
using SolidSampleApplication.Core.Services.CustomerServices;

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
        private readonly ICustomerDomainService _service;

        public ChangeNameCustomerCommandHandler(ReadModelDbContext readModelDbContext, IMediator mediator, ICustomerDomainService service)
        {
            _readModelDbContext = readModelDbContext;
            _mediator = mediator;
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