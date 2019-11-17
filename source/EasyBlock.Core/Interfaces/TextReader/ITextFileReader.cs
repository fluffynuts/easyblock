using System;

namespace EasyBlock.Core.Interfaces.TextReader
{
    public interface ITextFileReader: IDisposable
    {
        string ReadLine();
    }
}