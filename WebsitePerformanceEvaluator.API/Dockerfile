FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WebsitePerformanceEvaluator.API/WebsitePerformanceEvaluator.API.csproj", "WebsitePerformanceEvaluator.API/"]
COPY ["WebsitePerformanceEvaluator.Crawler/WebsitePerformanceEvaluator.Crawler.csproj", "WebsitePerformanceEvaluator.Crawler/"]
COPY ["WebsitePerformanceEvaluator.Domain/WebsitePerformanceEvaluator.Domain.csproj", "WebsitePerformanceEvaluator.Domain/"]
COPY ["WebsitePerformanceEvaluator.Application/WebsitePerformanceEvaluator.Application.csproj", "WebsitePerformanceEvaluator.Application/"]
COPY ["WebsitePerformanceEvaluator.Persistence/WebsitePerformanceEvaluator.Persistence.csproj", "WebsitePerformanceEvaluator.Persistence/"]
COPY ["WebsitePerformanceEvaluator.InfrastructureIoC/WebsitePerformanceEvaluator.InfrastructureIoC.csproj", "WebsitePerformanceEvaluator.InfrastructureIoC/"]
RUN dotnet restore "WebsitePerformanceEvaluator.API/WebsitePerformanceEvaluator.API.csproj"
COPY . .
WORKDIR "/src/WebsitePerformanceEvaluator.API"
RUN dotnet build "WebsitePerformanceEvaluator.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebsitePerformanceEvaluator.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebsitePerformanceEvaluator.API.dll"]
