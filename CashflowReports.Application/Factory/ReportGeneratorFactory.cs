using CashFlow.Application.Factory.Generator.PDF;
using CashFlow.Application.Factory.Generator.Excel;
using CashFlow.Domain.Enums;
using CashFlow.Application.Factory.Generator;

namespace CashFlow.Application.Factory
{
    public class ReportGeneratorFactory : IReportGeneratorFactory
    {
        public IReportGenerator GetGenerator(ReportType reportType)
        {
            return reportType switch
            {
                ReportType.PDF => new PdfReportGenerator(),
                ReportType.EXCEL => new ExcelReportGenerator(),
                _ => throw new ArgumentOutOfRangeException(nameof(reportType), $"Unsupported report type: {reportType}")
            };
        }
    }
}