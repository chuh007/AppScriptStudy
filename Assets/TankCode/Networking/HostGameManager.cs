using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TankCode.Networking
{
    public class HostGameManager
    {
        private Allocation _relayAllocation;
        private const int _maxConnectionCount = 10;
        private string _joinCode;
        public async Task<bool> StartHostAsync()
        {
            try
            {
                _relayAllocation = await RelayService.Instance.CreateAllocationAsync(_maxConnectionCount);
                _joinCode = await RelayService.Instance.GetJoinCodeAsync(_relayAllocation.AllocationId);
                Debug.Log(_joinCode);
                
                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                transport.SetRelayServerData(_relayAllocation.ToRelayServerData("dtls"));
                
                return NetworkManager.Singleton.StartHost();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        public void ChangeNetworkScene(string sceneName)
            => NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}