using System;

namespace EasyBlock.Core
{
    public interface ITextFileReader: IDisposable
    {
        string ReadLine();
    }
}