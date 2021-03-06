﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using System.Reflection;
using Wkhtmltopdf.NetCore;

namespace SolidSampleApplication.TableEngine
{
    public static class TableEngineExtensions
    {
        public static void AddTableEngine(this IServiceCollection services, Assembly assembly)
        {
            AddSimpleTableBuilders(services, assembly);
            services.AddWkhtmltopdf();
            services.AddTransient<IPdfGeneratorWrapper, PdfGeneratorWrapper>();
        }

        public static void AddSimpleTableBuilders(this IServiceCollection services, Assembly assembly)
        {
            var ty = typeof(ISimpleTableBuilder<,>);
            var allTypes = assembly.GetTypes().Where(x =>
                !x.IsAbstract &&
                !x.IsInterface &&
                x.GetInterfaces().Any(i => i.IsGenericType && (i.GetGenericTypeDefinition() == ty))).ToList();
            foreach(var t in allTypes)
            {
                var interfaceType = t.GetInterfaces().First(i => i.GetGenericTypeDefinition() == ty);
                services.TryAddEnumerable(ServiceDescriptor.Transient(interfaceType, t));
            }
        }
    }
}