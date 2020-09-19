using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolidSampleApplication.Infrastructure
{
    public static class ServiceCollectionExtension
    {
        public static void AddEnumerableInterfaces<T>(this IServiceCollection services, Assembly assembly, ServiceLifetime serviceLifeTime = ServiceLifetime.Scoped)
        {
            var allTypes = assembly
                .GetTypes()
                .Where(x =>
                    !x.IsAbstract &&
                    !x.IsInterface &&
                    x.GetInterfaces()
                        .Any(i => i == typeof(T))).ToList();
            foreach(var t in allTypes)
            {
                if(serviceLifeTime == ServiceLifetime.Scoped)
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(T), t));
                if(serviceLifeTime == ServiceLifetime.Transient)
                    services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(T), t));
                else
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(T), t));
            }
        }

        public static void AddEnumerableGenericInterfacesAsSingleton<T>(this IServiceCollection services, Assembly assembly)
        {
            var allTypes = assembly.GetTypes().Where(x =>
                !x.IsAbstract &&
                !x.IsInterface &&
                x.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(T)))).ToList();
            foreach(var t in allTypes)
            {
                var interfaceType = t.GetInterfaces().First(i => i.GetGenericTypeDefinition() == typeof(T));
                services.TryAddEnumerable(ServiceDescriptor.Singleton(interfaceType, t));
            }
        }

        public static void AddImplementedInterfacesNameEndsWith(this IServiceCollection services, Assembly assembly, string endsWith, ServiceLifetime serviceLifeTime = ServiceLifetime.Scoped)
        {
            var allAssemblies = _getAllReferencedAssemblies(assembly);
            var allTypes = allAssemblies
                .SelectMany(a => a.GetTypes()
                    .Where(x =>
                        !x.IsAbstract &&
                        !x.IsInterface &&
                        x.GetInterfaces().Any(i => i.Name.EndsWith(endsWith))))
                .ToList();
            foreach(var t in allTypes)
            {
                var interfaceType = t.GetInterfaces().First(i => i.Name.EndsWith(endsWith));
                if(serviceLifeTime == ServiceLifetime.Scoped)
                    services.TryAddEnumerable(ServiceDescriptor.Scoped(interfaceType, t));
                if(serviceLifeTime == ServiceLifetime.Transient)
                    services.TryAddEnumerable(ServiceDescriptor.Transient(interfaceType, t));
                else
                    services.TryAddEnumerable(ServiceDescriptor.Singleton(interfaceType, t));
            }
        }

        private static List<Assembly> _getAllReferencedAssemblies<T>(Assembly mainAssembly)
        {
            var namespaceName = typeof(T).Namespace;
            var assembliesName = mainAssembly
                .GetReferencedAssemblies()
                .Where(a => a.Name.Contains("SolidSampleApplication"))
                .ToList();
            var loadedAssemblies = assembliesName.Select(a => Assembly.Load(a)).ToList();
            loadedAssemblies.Add(mainAssembly);
            return loadedAssemblies.ToList();
        }

        private static List<Assembly> _getAllReferencedAssemblies(Assembly mainAssembly)
        {
            var assembliesName = mainAssembly
                .GetReferencedAssemblies()
                .Where(a => a.Name.Contains("SolidSampleApplication"))
                .ToList();
            var loadedAssemblies = assembliesName.Select(a => Assembly.Load(a)).ToList();
            loadedAssemblies.Add(mainAssembly);
            return loadedAssemblies.ToList();
        }
    }
}