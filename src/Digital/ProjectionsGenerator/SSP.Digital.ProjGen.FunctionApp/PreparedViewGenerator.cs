using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using SSP.Common.Messaging.Functions;
using SSP.Common.Messaging.Functions.Builders;
using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Repository;

namespace SSP.Digital.ProjGen.FunctionApp;

public class PreparedViewGenerator : ServiceBusTriggerBase
{
    public PreparedViewGenerator(IMessageContext messageContext, IErrorMetadataBuilder errorMetadataBuilder,
        IEventSchemaRepository eventSchemaRepository)
        : base(messageContext, errorMetadataBuilder, eventSchemaRepository) { }

    [FunctionName("PreparedViewGenerator")]
    public Task RunAsync(
        [ServiceBusTrigger("%OutEventTopicName%", "%OutEventSubscriptionName%", Connection = "MessageBusConnectionString")]
        ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, CancellationToken cancellationToken) =>
        ProcessMessageAsync(message, messageActions, cancellationToken);
}