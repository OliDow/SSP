using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using SSP.Common.Extensions;
using SSP.Common.Messaging.Extensions;
using SSP.EP.Application.Extensions;
using SSP.EP.FunctionApp;
using SSP.Events;
using System.Reflection;

#pragma warning disable CS8603

[assembly: FunctionsStartup(typeof(Startup))]

namespace SSP.EP.FunctionApp;

public class Startup : FunctionsStartup
{
    private static string ExecutingAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddTelemetry(ExecutingAssemblyName);
        builder.Services.AddCommonProviders();

        var configuration = builder.GetContext().Configuration;
        builder.Services.AddPocMessaging(configuration);
        builder.Services.AddPocEventHub(configuration);
        builder.Services.AddEventProvider<CreateAccount>();
        builder.Services.AddEventRouting(configuration);
    }
}