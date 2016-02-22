using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PeanutButter.Utils;

namespace EasyBlock.Core
{
    public class TextFileWriter: ITextFileWriter
    {
        private string _path;
        private List<string> _lines;

        public TextFileWriter(string path)
        {
            _path = path;
            _lines = new List<string>();
        }
        public void AppendLine(string line)
        {
            _lines.Add(line);
        }

        public void Dispose()
        {
            using (var tempFile = new AutoTempFile())
            {
                WriteLinesWithNoTrailingEmptyLineTo(tempFile.Path);
                EnsureFolderExistsFor(_path);
                File.Copy(tempFile.Path, _path, true);
            }
        }

        private void EnsureFolderExistsFor(string path)
        {
            var folder = Path.GetDirectoryName(path);
            if (Directory.Exists(folder))
                return;
            Directory.CreateDirectory(folder);
        }

        private void WriteLinesWithNoTrailingEmptyLineTo(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                _lines.Take(_lines.Count - 1)
                    .ForEach(writer.WriteLine);
                writer.Write(_lines.Last());
            }
        }
    }
}