using Newtonsoft.Json;
using System;

namespace SolidSampleApplication.Infrastructure
{
    public static class ObjectExtension
    {
        public static string ToJson<T>(this T toSerialize)
            => JsonConvert.SerializeObject(toSerialize);

        public static object? FromJson(this string toDeserialized, Type type)
            => JsonConvert.DeserializeObject(toDeserialized, type);

        public static string TryGetId<T>(this T item, string defaultReturn)
        {
            if(item == null)
                return defaultReturn;
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