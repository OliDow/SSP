using SSP.Common;
using SSP.Common.Messaging;

namespace SSP.Events;

public record CreateContact(string FirstName) : IEvent;