using System.Threading.Tasks;
using TankCode.System;
using UnityEngine;

namespace TankCode.Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private HostSingleton hostPrefab;
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            
            LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicated)
        {
            if (isDedicated == false)
            {
                HostSingleton hostSingleton = Instantiate(hostPrefab, transform);
                hostSingleton.CreateHost();
                
                ClientSingleton clientSingleton = Instantiate(clientPrefab, transform);
                bool isAuthenticated = await clientSingleton.CreateClientAsync();
                
                if(isAuthenticated)
                {
                    clientSingleton.GameManager.ChangeScene(SceneNames.MenuScene);
                }
                else
                {
                    Debug.LogError("UGS service error on now");
                }
            }
        }
    }
}