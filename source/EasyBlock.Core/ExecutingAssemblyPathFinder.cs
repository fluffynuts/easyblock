using System.Reflection;

namespace EasyBlock.Core
{
    public static class ExecutingAssemblyPathFinder
    {
        public static string GetExecutingAssemblyFolder()
        {
            return Assembly.GetExecutingAssembly()
                .CodeBase
                .AsLocalPath()
                .GetFolder();
        }
    }
}