using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSP.Common.Messaging.EventHub;
using SSP.Common.Messaging.Functions;
using SSP.Common.Messaging.Functions.Builders;
using SSP.Common.Messaging.Messaging;
using System.Diagnostics.CodeAnalysis;

namespace SSP.Common.Messaging.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddPocEventHub(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEventHubClient, EventEventHubClient>();
    }

    public static void AddPocMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBusConnectionString = configuration["MessageBusConnectionString"];
        services
            .AddTransient<IBusClient, BusClient>()
            .AddScoped<IMessageContext, MessageContext>()
            .AddTransient<IServiceBusTriggerServiceProvider, ServiceBusTriggerServiceProvider>()
            .AddTransient<IErrorMetadataBuilder, ErrorMetadataBuilder>()
            .AddAzureClients(builder => { builder.AddServiceBusClient(messageBusConnectionString); });
    }
}