using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SolidSampleApplication.ApplicationReadModel;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetAggregateMembershipQuery : IRequest<DefaultResponse>
    {
        // A way to make this value immutable, whilst at the same time able to be mapped from the controller
        private Guid? _id { get; set; }

        public Guid? Id
        {
            get
            {
                return _id;
            }
            set
            {
                if(_id != null)
                    throw new Exception($"Value has already been set {_id.ToString()}");
                _id = value;
            }
        }

        public GetAggregateMembershipQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetAggregateMembershipQueryValidator : AbstractValidator<GetAggregateMembershipQuery>
    {
        public GetAggregateMembershipQueryValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }

    public class GetAggregateMembershipQueryHandler : IRequestHandler<GetAggregateMembershipQuery, DefaultResponse>
    {
        private readonly ReadModelDbContext _readModelDbContext;

        public GetAggregateMembershipQueryHandler(ReadModelDbContext readModelDbContext)
        {
            _readModelDbContext = readModelDbContext;
        }

        public async Task<DefaultResponse> Handle(GetAggregateMembershipQuery request, CancellationToken cancellationToken)
        {
            var membership = await _readModelDbContext.Memberships.FirstOrDefaultAsync(m => m.Id == request.Id.Value);
            return DefaultResponse.Success(membership);
        }
    }
}