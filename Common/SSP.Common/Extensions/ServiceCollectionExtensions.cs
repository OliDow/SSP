using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using SSP.Common.Providers;
using SSP.Common.Telemetry;
using System.Diagnostics.CodeAnalysis;

namespace SSP.Common.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddTelemetry(this IServiceCollection services, string cloudRoleName)
        => services
            .AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer(cloudRoleName))
            .AddApplicationInsightsTelemetry();

    public static IServiceCollection AddCommonProviders(this IServiceCollection services)
        => services
            .AddSingleton<IDateTimeProvider, DateTimeProvider>()
            .AddSingleton<IGuidProvider, GuidProvider>();
}