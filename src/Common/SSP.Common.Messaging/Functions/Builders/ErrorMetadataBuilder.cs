namespace SSP.Common.Messaging.Functions.Builders;

internal sealed class ErrorMetadataBuilder : IErrorMetadataBuilder
{
    private const string ErrorMessagePropertyFormat = "Delivery{0}ErrorMessage";
    private const string ErrorTypePropertyFormat = "Delivery{0}ErrorType";

    public IDictionary<string, object> BuildErrorMetadata(int deliveryCount, Exception ex)
    {
        var errorMessagePropertyKey = string.Format(ErrorMessagePropertyFormat, deliveryCount);
        var errorTypePropertyKey = string.Format(ErrorTypePropertyFormat, deliveryCount);

        Dictionary<string, object> errorMetadata = new()
        {
            { errorMessagePropertyKey, ex.Message },
            { errorTypePropertyKey, ex.GetType().FullName ?? string.Empty },
            { "LastDeliveryErrorStackTrace", ex.StackTrace ?? string.Empty }
        };

        return errorMetadata;
    }
}