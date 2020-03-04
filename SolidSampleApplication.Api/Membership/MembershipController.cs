using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

    [ApiController]
    [Route("[controller]")]
    public class MembershipController : Controller
    {
        private readonly IMembershipRepository repository;

        public MembershipController(IMembershipRepository repository)
        {
            this.repository = repository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RegisterMember()
        {
            return View();
        }

        [HttpGet]
        public ActionResult<IEnumerable<MembershipTotalPoints>> GetMemberships()
        {
            var membership = repository.GetMemberships().FirstOrDefault();
            var points = repository.GetMembershipTotalPoints(membership.Id);
            return Ok(points);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<IEnumerable<MembershipTotalPoints>> GetMember(Guid id)
        {
            var membership = repository.GetMembership(id);
            return Ok(membership);
        }
    }
}