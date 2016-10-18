using System;

namespace EasyBlock.Core.Interfaces.IO.TextReader
{
    public interface ITextFileReader: IDisposable
    {
        string ReadLine();
    }
}