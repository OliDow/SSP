using SSP.Common;

namespace SSP.Events;

public record CreateContact(string FirstName) : IEvent;