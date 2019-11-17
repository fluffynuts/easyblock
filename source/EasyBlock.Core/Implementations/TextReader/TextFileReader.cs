using System.IO;
using EasyBlock.Core.Interfaces.TextReader;

namespace EasyBlock.Core.Implementations.TextReader
{
    public class TextFileReader: ITextFileReader
    {
        private StreamReader _reader;
        private readonly object _lock = new object();
        private string _path;
        private bool _disposed;

        public TextFileReader(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not found", path);
            _path = path;
        }

        public string ReadLine()
        {
            if (Reader == null)
                return null;
            var result = Reader.ReadLine();
            if (result == null)
            {
                DisposeReader();
            }
            return result;
        }

        private StreamReader Reader => _disposed
                                        ? null
                                        : _reader ?? (_reader = new StreamReader(_path));

        public void Dispose()
        {
            // TODO: find a way to prove this happens from within a test?
            lock(_lock)
            {
                if (_disposed)
                    return;
                _disposed = true;
                DisposeReader();
            }
        }

        private void DisposeReader()
        {
            _reader?.Dispose();
            _reader = null;
        }
    }
}