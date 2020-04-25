using FluentValidation;
using MediatR;
using SolidSampleApplication.Core;
using SolidSampleApplication.Infrastructure;
using SolidSampleApplication.Infrastructure.Repository;
using SolidSampleApplication.Infrastructure.Shared;
using SolidSampleApplication.Infrastucture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetCustomerMembershipDetailRequest : IRequest<DefaultResponse>
    {
        // Customer id
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

        public GetCustomerMembershipDetailRequest(Guid id)
        {
            Id = id;
        }
    }

    public class GetCustomerMembershipDetailRequestValidator : AbstractValidator<GetCustomerMembershipDetailRequest>
    {
        public GetCustomerMembershipDetailRequestValidator()
        {
            RuleFor(x => x.Id).NotNull();
        }
    }
}