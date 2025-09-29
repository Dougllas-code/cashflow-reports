using CashFlow.Domain.Entities;

namespace CashFlow.Application.UseCases.Reports
{
    public interface IProcessCashFlowMessageUseCase
    {
        Task ExecuteAsync(EventReportRequested message, CancellationToken cancellationToken);
    }
}
