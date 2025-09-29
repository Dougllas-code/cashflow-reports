using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Infra.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infra.Repositories
{
    internal class ExpenseRepository : IExpensesReadOnlyRepository
    {
        private readonly CashFlowWorkerDbContext _dbContext;

        public ExpenseRepository(CashFlowWorkerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Expense>> GetByMonth(long userId, DateOnly date)
        {
            var startDate = new DateTime(year: date.Year, month: date.Month, day: 1).Date;

            var daysInMonth = DateTime.DaysInMonth(year: date.Year, month: date.Month);
            var endDate = new DateTime(year: date.Year, month: date.Month, day: daysInMonth, hour: 23, minute: 59, second: 59);

            return await _dbContext
                .Expenses
                .AsNoTracking()
                .Where(e => e.UserId == userId && e.Date >= startDate && e.Date <= endDate)
                .OrderBy(e => e.Date)
                .ThenBy(e => e.Title)
                .ToListAsync();
        }
    }
}
