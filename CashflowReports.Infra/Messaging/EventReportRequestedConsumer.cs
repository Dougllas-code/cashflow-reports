using CashFlow.Domain.Entities;
using CashFlow.Domain.Services;
using MassTransit;

namespace CashFlow.Infra.Messaging
{
    public class EventReportRequestedConsumer : IRabbitMqConsumerService, IConsumer<EventReportRequested>
    {
        public async Task Consume(ConsumeContext<EventReportRequested> context)
        {
            await StartConsumingAsync(context, context.CancellationToken);
        }

        public async Task StartConsumingAsync(ConsumeContext<EventReportRequested> context, CancellationToken cancellationToken)
        {
            await MessageChannel.Channel.Writer.WriteAsync(context.Message, cancellationToken);
        }
    }
}