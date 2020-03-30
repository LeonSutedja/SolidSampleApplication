using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Api.Healthcheck
{
    [ApiController]
    [Route("healthcheck")]
    public class HealthcheckController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IEnumerable<IHealthcheckSystem> _healthCheckList;

        public HealthcheckController(IMediator mediator, IEnumerable<IHealthcheckSystem> healthCheckList)
        {
            _mediator = mediator;
            this._healthCheckList = healthCheckList;
        }

        [HttpGet]
        public ActionResult GetHealthcheck(string system, bool detail)
        {
            // if detail is check
            if (detail)
            {
                if (!string.IsNullOrEmpty(system))
                {
                    var systemToCheck = _healthCheckList.FirstOrDefault(s => s.Name.Equals(system, StringComparison.InvariantCultureIgnoreCase));
                    if (systemToCheck == null)
                        return BadRequest();
                    var result = systemToCheck.PerformCheck();
                    return new OkObjectResult(result);
                }

                var results = _healthCheckList.Select(sl => sl.PerformCheck());
                var hcResponse = new HealthcheckResponse(results);
                return new OkObjectResult(hcResponse);
            }

            // if not detail, but system specific only
            if (!string.IsNullOrEmpty(system))
            {
                var systemToCheck = _healthCheckList.FirstOrDefault(s => s.Name.Equals(system, StringComparison.InvariantCultureIgnoreCase));
                if (systemToCheck == null)
                    return BadRequest();
                var result = systemToCheck.PerformCheck();
                if (result.IsOk)
                    return Ok("OK");
                return Ok(result.Message);
            }

            // not detail, not system specific
            var allResults = _healthCheckList.Select(hc => hc.PerformCheck());
            return (allResults.Any(r => !r.IsOk))
                ? Ok("ERROR")
                : Ok("OK");
        }
    }
}