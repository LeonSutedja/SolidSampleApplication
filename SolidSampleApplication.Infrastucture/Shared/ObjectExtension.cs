using Newtonsoft.Json;

namespace SolidSampleApplication.Infrastructure
{
    public static class ObjectExtension
    {
        public static string ToJson<T>(this T toSerialize)
            => JsonConvert.SerializeObject(toSerialize);

        public static T FromJson<T>(this string toDeserialized)
            => JsonConvert.DeserializeObject<T>(toDeserialized);
    }
}