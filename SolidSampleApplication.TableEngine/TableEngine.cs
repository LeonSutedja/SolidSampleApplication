using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SolidSampleApplication.TableEngine
{
    public class SimpleTableData
    {
        public List<SimpleColumnDefinition> Columns { get; set; }

        public List<SimpleTableRow> Rows { get; set; }

        public int TotalItems { get; set; }
    }

    public class SimpleColumnDefinition
    {
        public static SimpleColumnDefinition SimpleInt(string heading, string code, int position)
            => new SimpleColumnDefinition(heading, code, "int", position);

        public static SimpleColumnDefinition SimpleString(string heading, string code, int position)
            => new SimpleColumnDefinition(heading, code, "string", position);

        public static SimpleColumnDefinition SimpleDate(string heading, string code, int position)
            => new SimpleColumnDefinition(heading, code, "date", position);

        public string Heading { get; private set; }
        public string Code { get; private set; }
        public string Type { get; private set; }
        public int Position { get; private set; }

        private SimpleColumnDefinition(string heading, string code, string type, int position)
        {
            Heading = heading;
            Code = code;
            Type = type;
            Position = position;
        }
    }

    public class SimpleTableRow
    {
        public List<string> Cells { get; set; }
    }

    public interface ISimpleTableBuilder<TEntity, TInput>
    {
        SimpleTableData Build(TInput input);
    }

    public abstract class SimpleTableBuilderGeneric<TEntity, TInput> : ISimpleTableBuilder<TEntity, TInput>
    {
        public SimpleTableBuilderGeneric()
        {
        }

        public SimpleTableData Build(TInput input)
        {
            var tableData = new SimpleTableData();
            tableData.Columns = GetColumnDefinitionsForReport(input);
            tableData.Rows = GetDataAsSimpleTableRow(input);
            tableData.TotalItems = tableData.Rows.Count;
            return tableData;
        }

        protected abstract List<SimpleColumnDefinition> GetColumnDefinitionsForReport(TInput input);

        protected abstract List<SimpleTableRow> GetDataAsSimpleTableRow(TInput input);
    }

    public static class TableEngineExtensions
    {
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