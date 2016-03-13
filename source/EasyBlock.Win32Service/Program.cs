using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using EasyBlock.Core;
using PeanutButter.ServiceShell;

namespace EasyBlock.Win32Service
{
    public class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class EasyBlockService: Shell
    {
        private IWindsorContainer _container;

        public EasyBlockService()
        {
            _container = WindsorBootstrapper.Bootstrap();
            DisplayName = "Easy Block AdBlocker";
            ServiceName = "EasyBlock";
        }

    }
}
