using MediatR;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class EarnPointsMembershipRequest : IRequest<DefaultResponse>
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
        protected EarnPointsMembershipRequest()
        {
        }

        public EarnPointsMembershipRequest(Guid id, MembershipPointsType type, double points)
        {
            Id = id;
            Type = type;
            Points = points;
        }
    }

    public class EarnPointsMembershipHandler : IRequestHandler<EarnPointsMembershipRequest, DefaultResponse>
    {
        private readonly IMembershipRepository _repository;

        public EarnPointsMembershipHandler(IMembershipRepository repository)
        {
            _repository = repository;
        }

        public async Task<DefaultResponse> Handle(EarnPointsMembershipRequest request, CancellationToken cancellationToken)
        {
            var totalPoints = _repository.EarnPoints(request.Id.Value, request.Type.Value, request.Points.Value);
            return DefaultResponse.Success(totalPoints);
        }
    }
}