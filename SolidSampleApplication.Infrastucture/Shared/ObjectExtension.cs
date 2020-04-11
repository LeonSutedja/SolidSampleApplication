using Newtonsoft.Json;
using System.Collections.Generic;

namespace SolidSampleApplication.Infrastructure
{
    public static class ObjectExtension
    {
        public static string ToJson<T>(this T toSerialize)
            => JsonConvert.SerializeObject(toSerialize);

        public static T FromJson<T>(this string toDeserialized)
            => JsonConvert.DeserializeObject<T>(toDeserialized);

        public static string TryGetId<T>(this T item, string defaultReturn)
        {
            if (item == null) return defaultReturn;
            try
            {
                return ((dynamic)item).Id.ToString();
            }
            catch
            {
                return defaultReturn;
            }
        }
    }
}