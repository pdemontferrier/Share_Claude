using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;

namespace CommonResources.Utilities
{
    public static class DeviceInfo
    {
        public static string DeviceID => Environment.MachineName;

        public static string DeviceIP
        {
            get
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // Filtrer : interface active, Ethernet ou Wi-Fi, non virtuelle
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                        !ni.Description.ToLower().Contains("virtual") &&
                        !ni.Description.ToLower().Contains("pseudo"))
                    {
                        var ipProps = ni.GetIPProperties();

                        // Doit avoir une passerelle (connexion internet probable)
                        if (ipProps.GatewayAddresses.Any(g => g.Address.AddressFamily == AddressFamily.InterNetwork))
                        {
                            foreach (var ip in ipProps.UnicastAddresses)
                            {
                                // Récupère la première IPv4 non APIPA
                                if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                                    !ip.Address.ToString().StartsWith("169.254"))
                                {
                                    return ip.Address.ToString();
                                }
                            }
                        }
                    }
                }

                // Aucun réseau actif trouvé
                return "127.0.0.1";
            }
        }

        /*
        public static string DeviceIP
        {
            get
            {
                string localIP = string.Empty;
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet || ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                localIP = ip.Address.ToString();
                            }
                        }
                    }
                }
                return localIP;
            }
        }
        */

        public static string DeviceUser
        {
            get
            {
                var name = WindowsIdentity.GetCurrent().Name;
                return name.Contains('\\') ? name.Split('\\').Last() : name;
            }
        }
    }
}
