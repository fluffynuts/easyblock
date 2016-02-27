using System;

namespace EasyBlock.Core
{
    public interface ITextFileWriter: IDisposable
    {
        void AppendLine(string line);
        void Persist();
    }
}
