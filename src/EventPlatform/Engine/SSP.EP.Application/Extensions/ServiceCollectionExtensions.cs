using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSP.EP.Application.Delivery;
using SSP.EP.Application.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace SSP.EP.Application.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddEventRouting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDeliveryStrategy, StandardStrategy>();
        services.AddTransient<IDeliveryStrategy, ImportantStrategy>();
        services.AddTransient<IEventConfigRepository, EventConfigRepository>();
    }
}