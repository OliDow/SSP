using SSP.Common;

namespace SSP.Events;

public record CreateAccount(string AccountNumber) : IEvent;