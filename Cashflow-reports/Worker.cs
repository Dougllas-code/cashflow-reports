using CashFlow.Application.UseCases.Reports;
using CashFlow.Infra.Messaging;

namespace CashFlow_reports
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker service started at: {Time}", DateTimeOffset.Now);
            var reader = MessageChannel.Channel.Reader;
            while (await reader.WaitToReadAsync(stoppingToken))
            {
                _logger.LogInformation("Waiting for messages at: {Time}", DateTimeOffset.Now);
                while (reader.TryRead(out var message))
                {
                    _logger.LogInformation("Received message with Id: {MessageId}, UserId: {UserId} at: {Time}", message.Id, message.UserId, DateTimeOffset.Now);
                    try
                    {
                        using var scope = _serviceScopeFactory.CreateScope();
                        var processCashFlowMessageUseCase = scope.ServiceProvider.GetRequiredService<IProcessCashFlowMessageUseCase>();
                        await processCashFlowMessageUseCase.ExecuteAsync(message, stoppingToken);
                        _logger.LogInformation("Processed message with Id: {MessageId} at: {Time}", message.Id, DateTimeOffset.Now);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message with Id: {MessageId} at: {Time}", message.Id, DateTimeOffset.Now);
                    }
                }
            }
            _logger.LogInformation("Worker service stopping at: {Time}", DateTimeOffset.Now);
        }
    }
}
