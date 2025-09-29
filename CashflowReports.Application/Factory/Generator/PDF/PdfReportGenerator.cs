using CashFlow.Application.Factory.Generator;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.Factory.Generator.PDF
{
    internal class PdfReportGenerator : IReportGenerator
    {
        public string ReportType => throw new NotImplementedException();

        public byte[] GenerateAsync(List<Expense> expenses, DateOnly month)
        {
            throw new NotImplementedException();
        }
    }
}