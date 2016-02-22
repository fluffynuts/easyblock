using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBlock.Core
{
    public interface IHostFile
    {
        IEnumerable<IHostFileLine> Lines { get; }
    }

    public class HostFile: IHostFile
    {
        private readonly ITextFileWriter _writer;

        public IEnumerable<IHostFileLine> Lines => _lines;
        private List<IHostFileLine> _lines;

        public HostFile(ITextFileReader reader, ITextFileWriter writer)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            _writer = writer;
            LoadFrom(reader);
        }

        private void LoadFrom(ITextFileReader reader)
        {
            string line;
            _lines = new List<IHostFileLine>();
            while ((line = reader.ReadLine()) != null)
            {
                _lines.Add(new HostFileLine(line));
            }
        }
    }
}
