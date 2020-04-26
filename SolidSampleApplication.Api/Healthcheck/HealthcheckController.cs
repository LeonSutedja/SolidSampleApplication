using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Healthcheck
{
    [ApiController]
    [Route("healthcheck")]
    public class HealthcheckController : Controller
    {
        private readonly IEnumerable<IHealthcheckSystem> _healthCheckList;

        public HealthcheckController(IEnumerable<IHealthcheckSystem> healthCheckList)
        {
            _healthCheckList = healthCheckList;
        }

        [HttpGet]
        public async Task<ActionResult> GetHealthcheck(string system, bool detail)
        {
            // if detail is check
            if(detail)
            {
                if(!string.IsNullOrEmpty(system))
                {
                    var systemToCheck = _healthCheckList.FirstOrDefault(s => s.Name.Equals(system, StringComparison.InvariantCultureIgnoreCase));
                    if(systemToCheck == null)
                        return BadRequest();
                    var result = systemToCheck.PerformCheck();
                    return new OkObjectResult(result);
                }

                var checkTasks = _healthCheckList.Select(sl => sl.PerformCheck());
                var results = await Task.WhenAll(checkTasks);
                var hcResponse = new HealthcheckResponse(results);
                return new OkObjectResult(hcResponse);
            }

            // if not detail, but system specific only
            if(!string.IsNullOrEmpty(system))
            {
                var systemToCheck = _healthCheckList.FirstOrDefault(s => s.Name.Equals(system, StringComparison.InvariantCultureIgnoreCase));
                if(systemToCheck == null)
                    return BadRequest();
                var result = await systemToCheck.PerformCheck();
                if(result.IsOk)
                    return Ok("OK");
                return Ok(result.Message);
            }

            // not detail, not system specific
            var allTasks = _healthCheckList.Select(hc => hc.PerformCheck());
            var allResults = await Task.WhenAll(allTasks);
            return (allResults.Any(r => !r.IsOk))
                ? Ok("ERROR")
                : Ok("OK");
        }
    }
}