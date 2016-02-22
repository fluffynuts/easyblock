using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBlock.Core
{
    public interface IMergedHostFile
    {
        IEnumerable<IHostFileLine> StaticLines { get; }
        IEnumerable<IHostFileLine> MergedLines { get; }

        void LoadFrom(ITextFileReader reader);
    }

    public class MergedHostFile: IMergedHostFile
    {
        public IEnumerable<IHostFileLine> StaticLines => _staticLines;
        private readonly List<IHostFileLine> _staticLines = new List<IHostFileLine>();

        public IEnumerable<IHostFileLine> MergedLines => _mergedLines;
        private readonly List<IHostFileLine> _mergedLines = new List<IHostFileLine>();

        public void LoadFrom(ITextFileReader reader)
        {
        }
    }
}
