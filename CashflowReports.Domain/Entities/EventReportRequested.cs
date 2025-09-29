using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entities
{
    public class EventReportRequested
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public DateOnly Month { get; set; }
        public ReportType ReportType { get; set; }
    }
}
