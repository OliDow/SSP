using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using CloudNative.CloudEvents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using SSP.Common;
using SSP.Common.Messaging.Messaging;
using SSP.Common.Providers;
using SSP.EP.Application.Functions;
using SSP.EP.Application.Repositories;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SSP.EP.FunctionApp;

public class EventIngester : HttpTriggerBase
{
    private readonly EventHubProducerClient _eventHubProducer;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public EventIngester(IMessageContext messageContext, IEventSchemaRepository eventSchemaRepository,
        IGuidProvider guidProvider, IConfiguration configuration)
        : base(messageContext, eventSchemaRepository, guidProvider)
    {
        var connectionString = configuration["EventHubEventsSend"];
        var eventHubName = configuration["EventHubName"];

        _eventHubProducer = new EventHubProducerClient(connectionString, eventHubName);
    }

    [FunctionName("EventIngester")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
        HttpRequest req, CancellationToken cancellationToken)
    {
        // Would be better if this was called implicitly
        SetCorrelationFromHttpRequestHeader(req);

        // Serialise payload to list of events
        var messages = JsonSerializer.Deserialize<List<IngestEvent>>(await req.ReadAsStringAsync());

        // atomically deserialise each event using it's type
        using var eventBatch = await _eventHubProducer.CreateBatchAsync(cancellationToken);
        foreach (var message in messages ?? new List<IngestEvent>()) // todo fix- hacky null check
        {
            var type = GetEventType(message.MessageType);
            var messageString = message.Data.ToString();
            // Check message deserialise, do something with failures
            var ev = (IEvent)JsonSerializer.Deserialize(messageString, type, SerializerOptions);

            var newEvent = new EventData(Encoding.UTF8.GetBytes(messageString));
            newEvent.Properties.Add(Constants.MessageTypePropertyName, type.Name);
            newEvent.CorrelationId = message.CorrelationId;

            eventBatch.TryAdd(newEvent);

            // todo Check schema registry
            await EventSchemaRepository.DoEventRepositoryStuff();
        }

        // send
        try
        {
            await _eventHubProducer.SendAsync(eventBatch, cancellationToken);
        }
        finally
        {
            await _eventHubProducer.DisposeAsync();
        }

        return new OkObjectResult((MessageContext.CorrelationId, MessageContext.MessageId));
    }

    // Below can be moved, here for a quick POC
    private record IngestEvent(string MessageType, object Data, string CorrelationId);
}