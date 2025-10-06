using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories
{
    public interface IReportRequestReadOnlyRepository
    {
        Task<ReportRequests?> GetById(Guid id);
    }
}
