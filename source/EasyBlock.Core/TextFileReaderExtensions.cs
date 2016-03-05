using System.Collections.Generic;

namespace EasyBlock.Core
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