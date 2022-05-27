using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSP.Common.Messaging.EventHub;
using SSP.Common.Messaging.Functions;
using SSP.Common.Messaging.Functions.Builders;
using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Provider;
using SSP.Common.Messaging.Repository;
using System.Diagnostics.CodeAnalysis;

namespace SSP.Common.Messaging.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static void AddPocEventHub(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEventHubClient, EventEventHubClient>();
    }

    public static void AddEventProvider<T>(this IServiceCollection services)
        where T : IEvent =>
        services.AddTransient<IEventProvider<T>, EventProvider<T>>();

    public static void AddPocMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTransient<IBusClient, BusClient>()
            .AddScoped<IMessageContext, MessageContext>()
            .AddTransient<IEventSchemaRepository, EventSchemaRepository>()
            .AddTransient<IErrorMetadataBuilder, ErrorMetadataBuilder>();

        var messageBusConnectionString = configuration["MessageBusConnectionString"];

        if (messageBusConnectionString != null)
        {
            services.AddAzureClients(builder => { builder.AddServiceBusClient(messageBusConnectionString); });
        }
    }
}