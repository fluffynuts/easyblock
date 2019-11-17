using System.Collections.Generic;
using System.Linq;
using EasyBlock.Core.Interfaces.HostFiles;
using NSubstitute;

namespace EasyBlock.Core.Tests.TestUtils
{
    public static class CollectionOfHostLinesExtensions
    {
        public static IEnumerable<IHostFileLine> Clone(this IEnumerable<IHostFileLine> src)
        {
            return src.Select(CreateSubstituteCopy);
        }

        private static IHostFileLine CreateSubstituteCopy(IHostFileLine arg)
        {
            var result = Substitute.For<IHostFileLine>();
            result.Data.Returns(arg.Data);
            result.HostName.Returns(arg.HostName);
            result.IPAddress.Returns(arg.IPAddress);
            result.IsComment.Returns(arg.IsComment);
            return result;
        }
    }
}