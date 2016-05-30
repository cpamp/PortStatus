using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using System.Net;

namespace PortStatus
{
    /// <summary>
    /// Check status (used/unused) of ports
    /// </summary>
    public class PortConnections
    {
        /// <summary>
        /// Smallest port number
        /// </summary>
        private const int MIN_PORT = 0;
        /// <summary>
        /// Largest port number
        /// </summary>
        private const int MAX_PORT = 65535;

        /// <summary>
        /// Port search starting point
        /// </summary>
        private int startPort;
        /// <summary>
        /// startPort accessors
        /// </summary>
        int StartPort
        {
            get { return startPort; }
            set { startPort = value; }
        }

        /// <summary>
        /// Port search ending point
        /// </summary>
        private int endPort;
        /// <summary>
        /// endPort accessors
        /// </summary>
        int EndPort
        {
            get { return endPort; }
            set { endPort = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PortConnections() : this(MIN_PORT, MAX_PORT)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">Port number to begin search.</param>
        public PortConnections(int start) : this(start, MAX_PORT)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">Port number to begin search.</param>
        /// <param name="end">Port number to end search.</param>
        public PortConnections(int start, int end)
        {
            startPort = start >= MIN_PORT || start < MAX_PORT ? start : MIN_PORT;
            endPort = end <= MAX_PORT || end > MIN_PORT ? end : MAX_PORT;
        }

        /// <summary>
        /// Get open ports within starting and ending port numbers.
        /// </summary>
        /// <param name="ports">Collection of used ports.</param>
        /// <returns>Open ports within starting and ending port numbers.</returns>
        private IEnumerable<int> checkPorts (IEnumerable<int> ports)
        {
            List<int> result = new List<int>();
            HashSet<int> hs = new HashSet<int>(ports);

            for (int i = startPort; i <= endPort; i++)
            {
                if (!hs.Contains(i)) { result.Add(i); }
            }

            return result;
        }

        /// <summary>
        /// Get ports which have TCP connections.
        /// </summary>
        /// <returns>Ports with TCP connections.</returns>
        private IEnumerable<int> usedTCP()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpEndPoints = properties.GetActiveTcpConnections();

            return tcpEndPoints.OrderBy(x => x.LocalEndPoint.Port)
                .Select(x => x.LocalEndPoint.Port)
                .Distinct();
        }

        /// <summary>
        /// Gets ports which have TCP listeners
        /// </summary>
        /// <returns>Ports with TCP listeners.</returns>
        private IEnumerable<int> listenTCP()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();

            return tcpEndPoints.OrderBy(x => x.Port)
                .Select(x => x.Port)
                .Distinct();
        }

        /// <summary>
        /// Gets ports which have UDP listeners
        /// </summary>
        /// <returns>Ports with UDP listeners.</returns>
        private IEnumerable<int> listenUDP()
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();

            return udpEndPoints.OrderBy(x => x.Port)
                .Select(x => x.Port)
                .Distinct();
        }

        /// <summary>
        /// Get ports without TCP connections.
        /// </summary>
        /// <returns>Ports without TCP connections.</returns>
        public IEnumerable<int> GetOpenTCP()
        {
            return checkPorts(usedTCP());
        }

        /// <summary>
        /// Gets ports without TCP listeners.
        /// </summary>
        /// <returns>Ports without TCP listeners.</returns>
        public IEnumerable<int> GetListenTCP()
        {
            return checkPorts(listenTCP());
        }

        /// <summary>
        /// Gets ports without UDP listeners.
        /// </summary>
        /// <returns>Ports without UDP listeners</returns>
        public IEnumerable<int> GetListenUDP()
        {
            return checkPorts(listenUDP());
        }

        /// <summary>
        /// Get all ports with TCP/UDP listeners and active TCP connections.
        /// </summary>
        /// <returns>Ports with TCP/UDP listeners and active TCP connections.</returns>
        public IEnumerable<int> GetAllUsedPorts()
        {
            return usedTCP().Union(listenTCP()).Union(listenUDP()).OrderBy(x => x);
        }

        /// <summary>
        /// Get all ports without TCP/UDP listeners and active TCP connections.
        /// </summary>
        /// <returns>Ports without TCP/UDP listeners and active TCP connections.</returns>
        public IEnumerable<int> GetAllOpenPorts()
        {
            return checkPorts(usedTCP().Union(listenTCP()).Union(listenUDP())).OrderBy(x => x);
        }

        /// <summary>
        /// Converts array of ports to string.
        /// </summary>
        /// <param name="ports">Array of ports to convert.</param>
        /// <param name="header">Message to display above port collection.</param>
        /// <returns>String of ports.</returns>
        private string portsToString(IEnumerable<int> ports, string header)
        {
            StringBuilder result = new StringBuilder();
            int[] arrPorts = ports.ToArray();

            result.Append(header);

            for (int i = 0; i < arrPorts.Length; i++)
            {
                result.Append(string.Format("{0}: Port {1}\n", i + 1, arrPorts[i]));
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets a string of ports without TCP connections.
        /// </summary>
        /// <returns>String of ports without TCP connections.</returns>
        public string OpenTCPString()
        {
            return portsToString(GetOpenTCP(), "Open TCP Ports:\n");
        }

        /// <summary>
        /// Gets string of ports with TCP connections.
        /// </summary>
        /// <returns>String of ports with TCP connections.</returns>
        public string UsedTCPString()
        {
            return portsToString(usedTCP(), "Used TCP Ports:\n");
        }

        /// <summary>
        /// Gets string of ports with TCP listeners.
        /// </summary>
        /// <returns>String of ports with TCP listeners.</returns>
        public string ListenTCPString()
        {
            return portsToString(listenTCP(), "Listener TCP Ports:\n");
        }

        /// <summary>
        /// Gets string of ports with UDP listeners.
        /// </summary>
        /// <returns>String of ports with UDP listeners.</returns>
        public string ListenUDPString()
        {
            return portsToString(listenUDP(), "Listener UDP Ports:\n");
        }

        /// <summary>
        /// Gets a string of all open ports.
        /// </summary>
        /// <returns>String of open ports.</returns>
        public string OpenPortsString()
        {
            return portsToString(GetAllOpenPorts(), "All open Ports:\n");
        }

        /// <summary>
        /// Gets a string of all used ports.
        /// </summary>
        /// <returns>String of all used ports.</returns>
        public string UsedPortsString()
        {
            return portsToString(GetAllUsedPorts(), "All used Ports:\n");
        }
    }
}
