using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    public static class AssemblyExtension
    {
        public static List<Assembly> GetAllAssembliesInNamespace(this Assembly mainAssembly, string namespaceToCheck)
        {
            var allAssembliesName = mainAssembly
                .GetReferencedAssemblies()
                .Where(a => a.Name.ToLower().Contains(namespaceToCheck.ToLower()))
                .ToList();
            var loadedAssemblies = allAssembliesName.Select(a => Assembly.Load(a)).ToList();
            loadedAssemblies.Add(mainAssembly);
            return loadedAssemblies;
        }
    }
}