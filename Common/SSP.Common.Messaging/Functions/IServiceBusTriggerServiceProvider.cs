using SSP.Common.Messaging.Functions.Builders;

namespace SSP.Common.Messaging.Functions;

public interface IServiceBusTriggerServiceProvider
{
    IErrorMetadataBuilder ErrorMetadataBuilder { get; }
}