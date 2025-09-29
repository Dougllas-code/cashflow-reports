using CashFlow.Domain.Entities;
using System.Threading.Channels;

namespace CashFlow.Infra.Messaging
{
    public static class MessageChannel
    {
        public static Channel<EventReportRequested> Channel { get; } = System.Threading.Channels.Channel.CreateUnbounded<EventReportRequested>();
    }
}