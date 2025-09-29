using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infra.DataAccess
{
    public class CashFlowWorkerDbContext: DbContext
    {
        public CashFlowWorkerDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Expense> Expenses { get; set; }
    }
}
