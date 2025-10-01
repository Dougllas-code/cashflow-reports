using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Repositories;
using CashFlow.Infra.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infra.Repositories
{
    internal class ReportRequestRepository : IReportRequestReadOnlyRepository, IReportRequestUpdateOnlyRepository
    {
        private readonly CashFlowWorkerDbContext _dbContext;

        public ReportRequestRepository(CashFlowWorkerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ReportRequests?> GetById(Guid id)
        {
            return await _dbContext.ReportRequests.FindAsync(id);
        }

        public async Task UpdateStatus(Guid id, ReportStatus status)
        {
           await _dbContext.ReportRequests
                .Where(r => r.Id == id)
                .ExecuteUpdateAsync(r => r.SetProperty(rr => rr.Status, status));
        }
    }
}
