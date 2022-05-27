using SSP.Common.Messaging.Functions.Builders;

namespace SSP.Common.Messaging.Functions;

internal class ServiceBusTriggerServiceProvider : IServiceBusTriggerServiceProvider
{
    private readonly IErrorMetadataBuilder _errorMetadataBuilder;

    public ServiceBusTriggerServiceProvider(IErrorMetadataBuilder errorMetadataBuilder)
    {
        _errorMetadataBuilder = errorMetadataBuilder ?? throw new ArgumentNullException(nameof(errorMetadataBuilder));
    }

    public IErrorMetadataBuilder ErrorMetadataBuilder => _errorMetadataBuilder;
}