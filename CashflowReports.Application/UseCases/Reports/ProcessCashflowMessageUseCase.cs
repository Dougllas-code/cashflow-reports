using CashFlow.Application.Factory;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Repositories;

namespace CashFlow.Application.UseCases.Reports
{
    internal class ProcessCashFlowMessageUseCase : IProcessCashFlowMessageUseCase
    {
        private readonly IReportGeneratorFactory _factory;
        private readonly IExpensesReadOnlyRepository _expenseRepository;

        public ProcessCashFlowMessageUseCase(
            IReportGeneratorFactory factory,
            IExpensesReadOnlyRepository expenseRepository)
        {
            _factory = factory;
            _expenseRepository = expenseRepository;
        }

        public async Task ExecuteAsync(EventReportRequested message, CancellationToken cancellationToken)
        {
            var reportType = message.ReportType;
            var generator = _factory.GetGenerator(reportType);

            var expenses = await _expenseRepository.GetByMonth(message.UserId, message.Month);

            if (expenses.Count == 0)
            {
                return;
            }

            var reportBytes = generator.GenerateAsync(expenses, message.Month);

            // Salvar o relatório gerado em uma pasta "Reports" na raiz da solution
            var reportsDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "Reports");
            Directory.CreateDirectory(reportsDir);

            var extension = reportType == ReportType.PDF ? ".pdf" : ".xlsx";
            var fileName = $"report_{message.UserId}_{message.Month:yyyyMM}{extension}";
            var filePath = Path.Combine(reportsDir, fileName);

            await File.WriteAllBytesAsync(filePath, reportBytes, cancellationToken);
        }
    }
}
