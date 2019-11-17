using System.Collections;
using EasyBlock.Core.Interfaces.HostFiles;

namespace EasyBlock.Core.Tests
{
    public class HostFileLineComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var left = x as IHostFileLine;
            var right = y as IHostFileLine;
            if (left == null && right == null) return 0;
            if (left == null || right == null) return 1;
            var areEqual = left.IsPrimary == right.IsPrimary &&
                            left.IsComment == right.IsComment &&
                            left.Data == right.Data &&
                            left.HostName == right.HostName &&
                            left.IPAddress == right.IPAddress;
            return areEqual ? 1 : 0;
        }
    }
}