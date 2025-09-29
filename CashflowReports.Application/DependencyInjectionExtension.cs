using CashFlow.Application.Factory;
using CashFlow.Application.Factory.Generator;
using CashFlow.Application.Factory.Generator.Excel;
using CashFlow.Application.Factory.Generator.PDF;
using CashFlow.Application.UseCases.Reports;
using Microsoft.Extensions.DependencyInjection;

namespace CashFlow.Application
{
    public static class DependencyInjectionExtension
    {
        public static void AddApplication(this IServiceCollection services)
        {
            AddUseCases(services);
        }

        public static void AddUseCases(IServiceCollection services)
        {
            services.AddSingleton<IReportGenerator, ExcelReportGenerator>();
            services.AddSingleton<IReportGenerator, PdfReportGenerator>();
            services.AddSingleton<IReportGeneratorFactory, ReportGeneratorFactory>();
            services.AddScoped<IProcessCashFlowMessageUseCase, ProcessCashFlowMessageUseCase>();
        }
    }
}
