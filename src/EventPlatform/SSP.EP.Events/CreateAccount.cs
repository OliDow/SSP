using SSP.Common.Messaging;

namespace SSP.Events;

public record CreateAccount(string AccountNumber) : IEvent;