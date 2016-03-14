using System.IO;

namespace EasyBlock.Core
{
    public class StarterConfigGenerator
    {
        public void CreateConfig()
        {
            var iniPath = Path.Combine(ExecutingAssemblyPathFinder.GetExecutingAssemblyFolder(), Constants.CONFIG_FILE);
        }
    }
}
