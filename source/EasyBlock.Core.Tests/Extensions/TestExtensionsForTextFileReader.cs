using System.Collections.Generic;
using System.Linq;
using EasyBlock.Core.Interfaces.TextReader;
using NSubstitute;

namespace EasyBlock.Core.Tests.Extensions
{
    public static class TestExtensionsForTextFileReader
    {
        public static void SetData(this ITextFileReader reader, params string[] lines)
        {
            SetDataActual(reader, lines);
        }

        private static void SetDataActual(ITextFileReader reader, IEnumerable<string> lines)
        {
            var queue = new Queue<string>(lines);
            reader.ReadLine().Returns(ci =>
            {
                if (!queue.Any())
                    return null;
                return queue.Dequeue();
            });
        } 

    }
}