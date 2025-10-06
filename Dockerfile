# Use uma imagem base oficial do .NET para workers
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copie tudo para o container
COPY . .

# Restaure as dependências
RUN dotnet restore "CashflowReports.sln"

# Compile e publique a aplicação
RUN dotnet publish "Cashflow-reports/CashFlow.Worker.csproj" -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CashFlow.Worker.dll"]
