using CashFlow.Application.Factory;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CashFlow.Application.UseCases.Reports
{
    internal class ProcessCashFlowMessageUseCase : IProcessCashFlowMessageUseCase
    {
        private readonly IReportGeneratorFactory _factory;
        private readonly IExpensesReadOnlyRepository _expenseRepository;
        private readonly IReportRequestReadOnlyRepository _reportRequestRepository;
        private readonly IReportRequestUpdateOnlyRepository _reportRequestUpdateRepository;
        private readonly ILogger<ProcessCashFlowMessageUseCase> _logger;

        public ProcessCashFlowMessageUseCase(
            IReportGeneratorFactory factory,
            IExpensesReadOnlyRepository expenseRepository,
            IReportRequestReadOnlyRepository reportRequestRepository,
            IReportRequestUpdateOnlyRepository reportRequestUpdateRepository,
            ILogger<ProcessCashFlowMessageUseCase> logger)
        {
            _factory = factory;
            _expenseRepository = expenseRepository;
            _reportRequestRepository = reportRequestRepository;
            _reportRequestUpdateRepository = reportRequestUpdateRepository;
            _logger = logger;
        }

        public async Task ExecuteAsync(EventReportRequested message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting processing for report request {ReportRequestId}", message.Id);
            var reportRequested = await _reportRequestRepository.GetById(message.Id);

            if (reportRequested is null ||
                reportRequested.Status == ReportStatus.PROCESSING || reportRequested.Status == ReportStatus.COMPLETED)
            {
                _logger.LogWarning("Report request {ReportRequestId} is null or already being processed/completed. Skipping.", message.Id);
                return;
            }

            await _reportRequestUpdateRepository.UpdateStatus(message.Id, ReportStatus.PROCESSING);
            _logger.LogInformation("Report request {ReportRequestId} status updated to PROCESSING", message.Id);

            try
            {
                var expenses = await _expenseRepository.GetByMonth(message.UserId, message.Month);

                if (expenses.Count == 0)
                {
                    _logger.LogWarning("No expenses found for user {UserId} and month {Month}. Aborting report generation.", message.UserId, message.Month);
                    return;
                }

                await GenerateReportFile(message, expenses);

                await _reportRequestUpdateRepository.UpdateStatus(message.Id, ReportStatus.COMPLETED);
                _logger.LogInformation("Report request {ReportRequestId} status updated to COMPLETED", message.Id);
            }
            catch (Exception ex)
            {
                await _reportRequestUpdateRepository.UpdateStatus(message.Id, ReportStatus.FAILED);
                _logger.LogError(ex, "Error processing report request {ReportRequestId}. Status updated to FAILED.", message.Id);
                throw;
            }
        }

        private async Task GenerateReportFile(EventReportRequested message, List<Expense> expenses)
        {
            var reportType = message.ReportType;
            var generator = _factory.GetGenerator(reportType);

            _logger.LogInformation("Generating report for user {UserId}, month {Month}, type {ReportType}", message.UserId, message.Month, reportType);
            var reportBytes = generator.GenerateAsync(expenses, message.Month);

            var reportsDir = Path.Combine(AppContext.BaseDirectory, "..", "..", "Reports");
            Directory.CreateDirectory(reportsDir);

            var extension = reportType == ReportType.PDF ? ".pdf" : ".xlsx";
            var fileName = $"report_{message.UserId}_{message.Month:yyyyMM}{extension}";
            var filePath = Path.Combine(reportsDir, fileName);

            await File.WriteAllBytesAsync(filePath, reportBytes);
            _logger.LogInformation("Report for request {ReportRequestId} saved to {FilePath}", message.Id, filePath);
        }
    }
}
