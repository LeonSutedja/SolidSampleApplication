using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wkhtmltopdf.NetCore;

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

        byte[] GenerateFileContentsPdf(TInput input);
    }

    public class PdfGeneratorWrapper : IPdfGeneratorWrapper
    {
        private readonly IGeneratePdf _generatePdf;

        public PdfGeneratorWrapper(IGeneratePdf generatePdf)
        {
            _generatePdf = generatePdf;
        }

        public IGeneratePdf GeneratePdf => _generatePdf;
    }

    public interface IPdfGeneratorWrapper
    {
        public IGeneratePdf GeneratePdf { get; }
    }

    public abstract class SimpleTableBuilderGeneric<TEntity, TInput> : ISimpleTableBuilder<TEntity, TInput>
    {
        public IPdfGeneratorWrapper _generatePdf;

        protected SimpleTableBuilderGeneric(IPdfGeneratorWrapper generatePdf)
        {
            _generatePdf = generatePdf;
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

        public byte[] GenerateFileContentsPdf(TInput input)
        {
            var thisPath = AppDomain.CurrentDomain.BaseDirectory;
            var table = Build(input);
            string template = File.ReadAllText(Path.Combine(thisPath, "template.cshtml"));
            string html = Engine.Razor.RunCompile(template, "templateKey", null, new { Data = (dynamic)table });

            byte[] res = null;
            using(MemoryStream ms = new MemoryStream())
            {
                var pdf = _generatePdf.GeneratePdf.GetByteArrayViewInHtml(html).Result;
                ms.Write(pdf, 0, pdf.Length);
                res = ms.ToArray();
            }
            return res;
        }
    }
}