using PeanutButter.ServiceShell;

namespace EasyBlock.Win32Service
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Shell.RunMain<EasyBlockService>(args);
        }
    }
}
