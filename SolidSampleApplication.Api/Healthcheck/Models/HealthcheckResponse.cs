using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.Api.Healthcheck
{
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
}