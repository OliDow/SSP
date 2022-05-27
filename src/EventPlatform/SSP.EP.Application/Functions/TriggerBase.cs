// using SSP.Common;
// using SSP.Common.Extensions;
// using SSP.Common.Messaging.Messaging;
// using SSP.EP.Application.Repositories;
// using SSP.Events;
//
// // ReSharper disable InvokeAsExtensionMethod
// namespace SSP.EP.Application.Functions;
//
// public class TriggerBase
// {
//     protected readonly IMessageContext MessageContext;
//     protected readonly IEventSchemaRepository EventSchemaRepository;
//
//     private static readonly List<Type> EventList =
//         AssemblyExtensions.FindDerivedTypes(typeof(CreateAccount).Assembly, typeof(IEvent));
//
//     protected TriggerBase(IMessageContext messageContext, IEventSchemaRepository eventSchemaRepository)
//     {
//         MessageContext = messageContext ?? throw new ArgumentNullException(nameof(messageContext));
//         EventSchemaRepository = eventSchemaRepository ?? throw new ArgumentNullException(nameof(eventSchemaRepository));
//     }
//
//     protected static Type GetEventType(string type)
//     {
//         try
//         {
//             return EventList.Single(t => t.Name == type);
//         }
//         catch (Exception e)
//         {
//             throw new Exception("Event Type not found in Type List", e);
//         }
//     }
// }