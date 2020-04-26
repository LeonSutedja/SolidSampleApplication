﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastucture;
using SolidSampleApplication.ReadModelStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Customers
{
    public class ChangeNameCustomerRequest : IRequest<DefaultResponse>
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ChangeNameCustomerRequestHandler : IRequestHandler<ChangeNameCustomerRequest, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly IMediator _mediator;

        public ChangeNameCustomerRequestHandler(ReadModelDbContext readModelDbContext, IMediator mediator)
        {
            _readModelDbContext = readModelDbContext;
            _mediator = mediator;
        }

        public async Task<DefaultResponse> Handle(ChangeNameCustomerRequest request, CancellationToken cancellationToken)
        {
            var @event = new CustomerNameChangedEvent(request.CustomerId, request.FirstName, request.LastName);
            await _mediator.Publish(@event);

            var customer = await _readModelDbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId);
            return DefaultResponse.Success(customer);
        }
    }

    public class PersistCustomerNameChangedEventHandler : INotificationHandler<CustomerNameChangedEvent>
    {
        private readonly ReadModelDbContext _readModelDbContext;
        private readonly SimpleEventStoreDbContext _simpleEventStoreDbContext;

        public PersistCustomerNameChangedEventHandler(ReadModelDbContext readModelDbContext, SimpleEventStoreDbContext simpleEventStoreDbContext)
        {
            _readModelDbContext = readModelDbContext;
            _simpleEventStoreDbContext = simpleEventStoreDbContext;
        }

        public async Task Handle(CustomerNameChangedEvent notification, CancellationToken cancellationToken)
        {
            await _simpleEventStoreDbContext.SaveEventAsync(notification, 1, DateTime.Now, "Sample");

            var eventStoreFactory = new GenericEntityFactory<Customer>(_simpleEventStoreDbContext);
            var coreModel = await eventStoreFactory.GetEntityAsync(notification.Id.ToString());
            var updatedReadModel = CustomerReadModel.FromAggregate(coreModel);

            // this way, we don't need to 'get data and update'.
            _readModelDbContext.Customers.Attach(updatedReadModel).State = EntityState.Modified;
            await _readModelDbContext.SaveChangesAsync();
        }
    }
}