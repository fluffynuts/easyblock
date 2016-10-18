using System;
using System.IO;

namespace EasyBlock.Core.Extensions
{
    public static class StringExtensions
    {
        public static int AsInteger(this string value)
        {
            int result;
            int.TryParse(value, out result);
            return result;
        }

        public static string AsLocalPath(this string uri)
        {
            return new Uri(uri).LocalPath;
        }

        public static string GetFolder(this string path)
        {
            return Path.GetDirectoryName(path);
        }
    }
}