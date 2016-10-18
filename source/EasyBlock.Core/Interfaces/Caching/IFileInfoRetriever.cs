using System.IO;

namespace EasyBlock.Core.Interfaces.Caching
{
    public interface IFileInfoRetriever
    {
        FileInfo GetFileInfoFor(string path);
    }
}
