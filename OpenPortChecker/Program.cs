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
            Console.Write(portChecker.OpenTCPString());
            Console.ReadLine();
            Console.Write(portChecker.UsedTCPString());
            Console.ReadLine();
        }
    }
}
