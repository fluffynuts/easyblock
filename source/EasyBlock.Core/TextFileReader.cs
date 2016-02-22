using System.IO;

namespace EasyBlock.Core
{
    public class TextFileReader: ITextFileReader
    {
        private StreamReader _reader;
        private readonly object _lock = new object();

        public TextFileReader(string path)
        {
            _reader = new StreamReader(path);
        }

        public string ReadLine()
        {
            return _reader?.ReadLine();
        }

        public void Dispose()
        {
            // TODO: find a way to prove this happens from within a test?
            lock(_lock)
            {
                _reader?.Dispose();
                _reader = null;
            }
        }
    }
}