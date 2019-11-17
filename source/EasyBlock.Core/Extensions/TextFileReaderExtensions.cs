using System.Collections.Generic;
using EasyBlock.Core.Interfaces.TextReader;

namespace EasyBlock.Core.Extensions
{
    public static class TextFileReaderExtensions
    {
        public static IEnumerable<string> EnumerateLines(this ITextFileReader reader)
        {
            string line;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        } 

    }
}