using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using SSP.Common.Messaging;
using SSP.Common.Messaging.Functions;
using SSP.Common.Messaging.Messaging;
using SSP.Common.Messaging.Provider;
using SSP.Common.Messaging.Repository;
using SSP.Common.Providers;
using SSP.Events;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SSP.EP.FunctionApp;

public class EventIngester : HttpTriggerBase
{
    private readonly IEventProvider<CreateAccount> _eventProvider;
    private readonly EventHubProducerClient _eventHubProducer;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public EventIngester(
        IMessageContext messageContext,
        IEventSchemaRepository eventSchemaRepository,
        IGuidProvider guidProvider,
        IEventProvider<CreateAccount> eventProvider, // need to wrap this to prevent the need to specify a type when injecting
        IConfiguration configuration)
        : base(messageContext, eventSchemaRepository, guidProvider)
    {
        _eventProvider = eventProvider ?? throw new ArgumentNullException(nameof(eventProvider));
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

        // Serialise payload to list of events (single point of failure cufrrenly)
        var messages = JsonSerializer.Deserialize<List<IngestEvent>>(await req.ReadAsStringAsync());

        // atomically deserialise each event using it's type
        using var eventBatch = await _eventHubProducer.CreateBatchAsync(cancellationToken);
        // todo fix- hacky null check
        foreach (var message in messages ?? new List<IngestEvent>())
        {
            try
            {
                var type = _eventProvider.GetEventType(message.MessageType);
                var messageString = Convert.ToString(message.Data) ?? throw new Exception();

                // Check message deserialise, do something with failures
                var ev = JsonSerializer.Deserialize(messageString, type, SerializerOptions) as IEvent;

                var newEvent = new EventData(Encoding.UTF8.GetBytes(messageString));
                newEvent.Properties.Add(Constants.MessageTypePropertyName, type.Name);
                newEvent.CorrelationId = message.CorrelationId;

                eventBatch.TryAdd(newEvent);

                // todo Check schema registry
                await EventSchemaRepository.DoEventRepositoryStuff();
            }
            catch (Exception e)
            {
                // log error atomic transaction
                Console.WriteLine(e.ToString());
            }
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


// versio