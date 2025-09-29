using CashFlow.Application;
using CashFlow.Infra;
using CashFlow_reports;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var host = builder.Build();
host.Run();
