namespace SSP.Common.Messaging.Functions.Builders;

public interface IErrorMetadataBuilder
{
    IDictionary<string, object> BuildErrorMetadata(int deliveryCount, Exception exception);
}