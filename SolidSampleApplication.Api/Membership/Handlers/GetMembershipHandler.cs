using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Membership
{
    public class GetMembershipResponse
    {
        public ActionResult Output { get; set; }
        public int ErrorId { get; set; }
        public string ErrorDescription { get; set; }

        public GetMembershipResponse(ActionResult output, int errorId = -1, string errorDescription = "")
        {
            Output = output;
            ErrorId = errorId;
            ErrorDescription = errorDescription;
        }
    }

    public class GetMembershipRequest : IRequest<GetMembershipResponse>
    {
    }

    public class GetMembershipHandler : IRequestHandler<GetMembershipRequest, GetMembershipResponse>
    {
        private readonly IMembershipRepository repository;

        public GetMembershipHandler(IMembershipRepository repository)
        {
            this.repository = repository;
        }

        public async Task<GetMembershipResponse> Handle(GetMembershipRequest request, CancellationToken cancellationToken)
        {
            var membership = repository.GetMemberships().FirstOrDefault();
            var points = repository.GetMembershipTotalPoints(membership.Id);
            var newOkObjectResult = new OkObjectResult(points);
            return new GetMembershipResponse(newOkObjectResult);
        }
    }
}