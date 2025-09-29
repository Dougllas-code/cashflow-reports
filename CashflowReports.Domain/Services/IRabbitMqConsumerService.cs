using CashFlow.Domain.Entities;
using MassTransit;

namespace CashFlow.Domain.Services
{
    public interface IRabbitMqConsumerService
    {
        Task StartConsumingAsync(ConsumeContext<EventReportRequested> context, CancellationToken cancellationToken);
    }
}
