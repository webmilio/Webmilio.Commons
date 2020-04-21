using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Webmilio.Commons.Networking
{
    public static class NetworkUtilities
    {
        // Source: https://www.c-sharpcorner.com/forums/how-to-get-local-machine-ip-address-in-net-core-22
        public static UnicastIPAddressInformation GetLocalAddress()
        {
            UnicastIPAddressInformation result = default;

            foreach (var inter in NetworkInterface.GetAllNetworkInterfaces().Where(i => i.OperationalStatus == OperationalStatus.Up))
            {
                var properties = inter.GetIPProperties();

                if (properties.GatewayAddresses.Count == 0)
                    continue;


                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily == AddressFamily.InterNetworkV6 || IPAddress.IsLoopback(address.Address))
                        continue;


                    if (!address.IsDnsEligible)
                    {
                        if (result == default)
                            result = address;

                        continue;
                    }



                    // DHCP IP is best IP.
                    if (address.PrefixOrigin == PrefixOrigin.Dhcp) 
                        return address;
                    
                    if (result != default || !result.IsDnsEligible)
                        result = address;


                    return result;
                }

                return result;
            }

            return default;
        }
    }
}