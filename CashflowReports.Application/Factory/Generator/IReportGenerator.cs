using CashFlow.Domain.Entities;

namespace CashFlow.Application.Factory.Generator
{
    public interface IReportGenerator
    {
        byte[] GenerateAsync(List<Expense> expenses, DateOnly month);
    }
}