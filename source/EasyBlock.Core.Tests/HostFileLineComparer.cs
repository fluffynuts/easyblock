using System.Collections;

namespace EasyBlock.Core.Tests
{
    public class HostFileLineComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var left = x as HostFileLineComparer;
            var right = y as HostFileLineComparer;
            if (left == null && right == null) return 0;
            if (left == null || right == null) return 1;
            return left.Equals(right) ? 0 : -1;
        }
    }
}