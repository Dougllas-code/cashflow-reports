using CashFlow.Domain.Repositories;
using CashFlow.Domain.Services;
using CashFlow.Infra.DataAccess;
using CashFlow.Infra.Messaging;
using CashFlow.Infra.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Infra
{
    public static class DependencyInjectionExtension
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            AddDbContext(services, configuration);
            AddMassTransit(services, configuration);
            AddRepositories(services);
        }

        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IRabbitMqConsumerService, EventReportRequestedConsumer>();
            services.AddScoped<IExpensesReadOnlyRepository, ExpenseRepository>();
            services.AddScoped<IReportRequestReadOnlyRepository, ReportRequestRepository>();
            services.AddScoped<IReportRequestUpdateOnlyRepository, ReportRequestRepository>();
        }

        private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection");
            var serverVersion = ServerVersion.AutoDetect(connectionString);

            services.AddDbContext<CashFlowWorkerDbContext>(config => config.UseMySql(connectionString, serverVersion));
        }

        private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetSection("Settings:RabbitMq:Host").Value!;
            var username = configuration.GetSection("Settings:RabbitMq:Username").Value!;
            var password = configuration.GetSection("Settings:RabbitMq:Password").Value!;

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumer<EventReportRequestedConsumer>();

                busConfigurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(connection, host =>
                    {
                        host.Username(username);
                        host.Password(password);
                    });

                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}
