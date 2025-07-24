using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace TankCode.Networking
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton _instance;

        public static HostSingleton Instance
        {
            get
            {
                if(_instance != null) return _instance;

                _instance = FindFirstObjectByType<HostSingleton>();

                Debug.Assert(_instance != null, "No host Singleton");
                return _instance;
            }
        }
        
        public HostGameManager GameManager { get; private set; }
        
        public void CreateHost()
        {
            GameManager = new HostGameManager();
        }

        public string GetFirstIPAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in ipEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            
            return null;
        }
    }
}