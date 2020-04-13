using FluentValidation;
using MediatR;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class EarnPointsAggregateMembershipRequest : IRequest<DefaultResponse>
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
                if (_id != null) throw new Exception("Value has already been set");
                _id = value;
            }
        }

        private double? _points { get; set; }

        public double? Points
        {
            get
            {
                return _points;
            }
            set
            {
                if (_points != null) throw new Exception("Value has already been set");
                _points = value;
            }
        }

        private MembershipPointsType? _type { get; set; }

        public MembershipPointsType? Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type != null) throw new Exception("Value has already been set");
                _type = value;
            }
        }

        // empty constructor require for api
        protected EarnPointsAggregateMembershipRequest()
        {
        }

        public EarnPointsAggregateMembershipRequest(Guid id, MembershipPointsType type, double points)
        {
            Id = id;
            Type = type;
            Points = points;
        }
    }

    public class EarnPointsAggregateMembershipHandlerValidator : AbstractValidator<EarnPointsAggregateMembershipRequest>
    {
        public EarnPointsAggregateMembershipHandlerValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Points).NotNull();
            RuleFor(x => x.Type).NotNull();
        }
    }

    public class EarnPointsAggregateMembershipHandler : IRequestHandler<EarnPointsAggregateMembershipRequest, DefaultResponse>
    {
        private readonly IAggregateMembershipRepository _repository;

        public EarnPointsAggregateMembershipHandler(IAggregateMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(EarnPointsAggregateMembershipRequest request, CancellationToken cancellationToken)
        {
            var aggregateMembership = await _repository.EarnPoints(request.Id.Value, request.Type.Value, request.Points.Value);
            return DefaultResponse.Success(aggregateMembership);
        }
    }
}