namespace EasyBlock.Core.Interfaces.Caching
{
    public interface ICacheFilenameGenerator
    {
        string GenerateFor(string sourceUrl);
    }
}