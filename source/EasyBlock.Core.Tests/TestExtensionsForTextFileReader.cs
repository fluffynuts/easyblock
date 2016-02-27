using System.Collections.Generic;
using System.Linq;
using NSubstitute;

namespace EasyBlock.Core.Tests
{
    public static class TestExtensionsForTextFileReader
    {
        public static void SetData(this ITextFileReader reader, params string[] lines)
        {
            SetDataActual(reader, lines);
        }

        public static void SetData(this ITextFileReader reader, IEnumerable<string> lines)
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