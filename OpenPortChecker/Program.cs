using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PortStatus;

namespace OpenPortChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            PortConnections portChecker = new PortStatus.PortConnections();
            Console.Write(portChecker.OpenPortsString());
            Console.ReadLine();
            Console.Write(portChecker.UsedPortsString());
            Console.ReadLine();
        }
    }
}
