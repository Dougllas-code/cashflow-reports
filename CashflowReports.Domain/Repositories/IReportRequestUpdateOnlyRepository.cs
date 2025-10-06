namespace CashFlow.Domain.Repositories
{
    public interface IReportRequestUpdateOnlyRepository
    {
        Task UpdateStatus(Guid id, Enums.ReportStatus status);
    }
}
