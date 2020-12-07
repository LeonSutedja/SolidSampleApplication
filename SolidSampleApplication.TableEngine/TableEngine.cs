using System.Collections.Generic;
using System.Linq;

namespace SolidSampleApplication.TableEngine
{
    public class SimpleTableData
    {
        public IEnumerable<SimpleColumnDefinition> Columns { get; init; }

        public IEnumerable<SimpleTableRow> Rows { get; init; }

        public int TotalItems { get; init; }
    }

    public class SimpleTableRow
    {
        public List<string> Cells { get; init; }
    }

    public interface ISimpleTableBuilder<TEntity, TInput>
    {
        SimpleTableData Build(TInput input);
    }

    public abstract class SimpleTableBuilderGeneric<TEntity, TInput> : ISimpleTableBuilder<TEntity, TInput>
    {
        protected SimpleTableBuilderGeneric()
        {
        }

        public SimpleTableData Build(TInput input)
        {
            var entities = GetDataAsEntity(input);
            var tableData = new SimpleTableData()
            {
                Columns = GetColumnDefinitionsForReport(input).ToList(),
                Rows = entities.Select(e => MapToTableRow(e)),
                TotalItems = entities.Count()
            };
            return tableData;
        }

        protected abstract IEnumerable<SimpleColumnDefinition> GetColumnDefinitionsForReport(TInput input);

        protected abstract SimpleTableRow MapToTableRow(TEntity entity);

        protected abstract IEnumerable<TEntity> GetDataAsEntity(TInput input);
    }
}