using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories
{
    public interface IExpensesReadOnlyRepository
    {
        Task<List<Expense>> GetByMonth(long userId, DateOnly date);
    }
}
