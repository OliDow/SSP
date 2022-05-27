namespace SSP.Common.Providers
{
    public interface IGuidProvider
    {
        Guid NewGuid();

        Guid Parse(string input);
    }
}