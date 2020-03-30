using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Api.Healthcheck
{
    [ApiController]
    [Route("pophealthcheck")]
    public class HealthcheckController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IEnumerable<IHealthcheckSystem> healthCheckList;

        public HealthcheckController(IMediator mediator, IEnumerable<IHealthcheckSystem> healthCheckList)
        {
            _mediator = mediator;
            this.healthCheckList = healthCheckList;
        }

        [HttpGet]
        public ActionResult GetHealthcheck(string system, bool detail)
        {
            if (detail)
            {
                if (!string.IsNullOrEmpty(system))
                {
                    var systemToCheck = healthCheckList.FirstOrDefault(s => s.Name.Equals(system, StringComparison.InvariantCultureIgnoreCase));
                    if (systemToCheck == null)
                        return BadRequest();
                    var result = systemToCheck.PerformCheck();
                    return new OkObjectResult(result);
                }
                var results = healthCheckList.Select(sl => sl.PerformCheck());
                var hcResponse = new HealthcheckResponse(results);
                return new OkObjectResult(hcResponse);
            }

            if (!string.IsNullOrEmpty(system))
            {
                var systemToCheck = healthCheckList.FirstOrDefault(s => s.Name.Equals(system, StringComparison.InvariantCultureIgnoreCase));
                if (systemToCheck == null)
                    return BadRequest();
                var result = systemToCheck.PerformCheck();
                if (result.IsOk)
                    return Ok("OK");
                return Ok(result.Message);
            }

            return Ok("OK");
        }
    }

    public class HealthcheckResponse
    {
        public bool IsOk { get; }
        public IEnumerable<HealthcheckSystemResult> SystemCheckResults { get; }

        public HealthcheckResponse(IEnumerable<HealthcheckSystemResult> systemCheckResults)
        {
            IsOk = !systemCheckResults.Any(r => !r.IsOk);
            SystemCheckResults = systemCheckResults;
        }
    }

    public class HealthcheckSystemResult
    {
        public string Name { get; }
        public bool IsOk { get; }
        public string Message { get; private set; }

        public HealthcheckSystemResult(string name, bool isOk, string message = "")
        {
            Name = name;
            IsOk = isOk;
            Message = message;
        }

        public void SetMessage(string message)
        {
            Message = message;
        }
    }

    public interface IHealthcheckSystem
    {
        string Name { get; }

        HealthcheckSystemResult PerformCheck();
    }

    public class DatabaseHealthcheck : IHealthcheckSystem
    {
        public string Name => "database";

        public HealthcheckSystemResult PerformCheck()
            => new HealthcheckSystemResult(Name, true);
    }

    public class ConfigurationHealthcheck : IHealthcheckSystem
    {
        public string Name => "config";

        public HealthcheckSystemResult PerformCheck()
            => new HealthcheckSystemResult(Name, true);
    }

    public class MediatorHealthcheck : IHealthcheckSystem
    {
        public string Name => "mediator";

        public HealthcheckSystemResult PerformCheck()
            => new HealthcheckSystemResult(Name, false, "failed instantiating");
    }
}