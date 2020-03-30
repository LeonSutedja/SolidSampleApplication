using Newtonsoft.Json;

namespace SolidSampleApplication.Infrastructure
{
    public static class ObjectExtension
    {
        public static string ToJson<T>(this T toSerialize)
        {
            return JsonConvert.SerializeObject(toSerialize);
        }
    }
}