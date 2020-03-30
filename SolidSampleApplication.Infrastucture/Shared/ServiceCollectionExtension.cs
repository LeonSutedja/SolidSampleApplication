using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Reflection;

namespace SolidSampleApplication.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void AddEnumerableInterfacesAsSingleton<T>(this IServiceCollection services, Assembly assembly)
        {
            var allTypes = assembly
                .GetTypes()
                .Where(x =>
                    !x.IsAbstract &&
                    !x.IsInterface &&
                    x.GetInterfaces()
                        .Any(i => i == typeof(T))).ToList();
            foreach (var t in allTypes)
                services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(T), t));
        }

        public static void AddEnumerableGenericInterfacesAsSingleton<T>(this IServiceCollection services, Assembly assembly)
        {
            var allTypes = assembly.GetTypes().Where(x =>
                !x.IsAbstract &&
                !x.IsInterface &&
                x.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(T)))).ToList();
            foreach (var t in allTypes)
            {
                var interfaceType = t.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(T));
                services.TryAddEnumerable(ServiceDescriptor.Singleton(interfaceType, t));
            }
        }
    }
}