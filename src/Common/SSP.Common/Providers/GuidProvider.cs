namespace SSP.Common.Providers
{
    internal sealed class GuidProvider : IGuidProvider
    {
        public Guid NewGuid() => Guid.NewGuid();

        public Guid Parse(string input) => Guid.Parse(input);
    }
}