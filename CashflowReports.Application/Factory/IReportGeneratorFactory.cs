using CashFlow.Application.Factory.Generator;
using CashFlow.Domain.Enums;

namespace CashFlow.Application.Factory
{
    public interface IReportGeneratorFactory
    {
        IReportGenerator GetGenerator(ReportType reportType);
    }
}