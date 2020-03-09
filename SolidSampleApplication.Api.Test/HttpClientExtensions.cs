using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SolidSampleApplication.Api.Test
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostRequestAsStringContent(
            this HttpClient client,
            string url,
            object request)
        {
            var jsonString = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return client.PostAsync(url, content);
        }
    }
}