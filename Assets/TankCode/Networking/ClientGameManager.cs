using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankCode.Networking
{
    public class ClientGameManager
    {
        private JoinAllocation _joinAllocation;
        
        public async Task<bool> InitManagerAsync()
        {
            await UnityServices.InitializeAsync();

            UGSAuthState authState = await UGSAuthWrapper.DoAuthAsync();

            if (authState == UGSAuthState.Authenticated)
            {
                return true;
            }
            
            return false;
        }

        public async Task<bool> StartClientWithJoinCode(string joinCode)
        {
            try
            {
                _joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(_joinAllocation.ToRelayServerData("dtls"));
                
                return NetworkManager.Singleton.StartClient();
            }
            catch (Exception ex)
            {
                 Debug.LogException(ex);
                 return false;
            }
        }
        
        public void ChangeScene(string sceneName)
            => SceneManager.LoadScene(sceneName);
    }
}