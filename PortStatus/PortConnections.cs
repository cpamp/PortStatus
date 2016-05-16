using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

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
        public PortConnections()
        {
            startPort = MIN_PORT;
            EndPort = MAX_PORT;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">Port number to begin search.</param>
        public PortConnections(int start)
        {
            startPort = start >= MIN_PORT || start < MAX_PORT ? start : MIN_PORT;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">Port number to begin search.</param>
        /// <param name="end">Port number to end search.</param>
        public PortConnections(int start, int end) : this(start)
        {
            endPort = end <= MAX_PORT || end > MIN_PORT ? end : MAX_PORT;
        }

        /// <summary>
        /// Get ports within starting and ending port numbers.
        /// </summary>
        /// <param name="ports">Ports to check.</param>
        /// <returns>Ports within starting and ending port numbers.</returns>
        private IEnumerable<int> checkPorts (IEnumerable<int> ports)
        {
            List<int> result = new List<int>();

            for (int i = startPort; i <= endPort; i++)
            {
                if (!ports.Contains(i)) { result.Add(i); }
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

            return tcpEndPoints.OrderBy(x => x.LocalEndPoint.Port).Select(x => x.LocalEndPoint.Port).ToList<int>();
        }

        /// <summary>
        /// Get ports without TCP connections.
        /// </summary>
        /// <returns>Ports without TCP connections.</returns>
        public IEnumerable<int> GetOpenTCP()
        {
            List<int> result = new List<int>();

            return checkPorts(usedTCP());
        }

        /// <summary>
        /// Converts array of ports to string.
        /// </summary>
        /// <param name="ports">Array of ports to convert.</param>
        /// <returns>String of ports.</returns>
        private string portsToString(int[] ports)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < ports.Length; i++)
            {
                result.Append(string.Format("{0}: Port {1}\n", i + 1, ports[i]));
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets a string of ports without TCP connections.
        /// </summary>
        /// <returns>String of ports without TCP connections.</returns>
        public string OpenTCPString()
        {
            StringBuilder result = new StringBuilder();
            int[] ports = GetOpenTCP().ToArray();

            result.Append("Open TCP Ports:\n");
            result.Append(portsToString(ports));

            return result.ToString();
        }

        /// <summary>
        /// Gets string of ports with TCP connections.
        /// </summary>
        /// <returns>String of ports with TCP connections.</returns>
        public string UsedTCPString()
        {
            StringBuilder result = new StringBuilder();

            result.Append("Used TCP Ports:\n");
            result.Append(portsToString(usedTCP().ToArray()));

            return result.ToString();
        }
    }
}
