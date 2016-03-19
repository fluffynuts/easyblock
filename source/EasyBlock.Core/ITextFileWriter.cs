using System;

namespace EasyBlock.Core
{
    public interface ITextFileWriter
    {
        void AppendLine(string line);
        void Persist();
    }
}
