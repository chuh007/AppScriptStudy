using System;
using System.Text.RegularExpressions;
using TankCode.Networking;
using TankCode.System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TankCode.UI
{
    public class IpConnectUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipInputField;
        [SerializeField] private TMP_InputField postInputField;

        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;
        
        private void Start()
        {
            if(NetworkManager.Singleton == null) return;
            
            postInputField.text = "7777";
            string firstIP = HostSingleton.Instance.GetFirstIPAddress();
            ipInputField.text = string.IsNullOrEmpty(firstIP) ? string.Empty : firstIP;
            
            hostBtn.onClick.AddListener(HandleHostBtnClick);
            clientBtn.onClick.AddListener(HandleClientBtnClick);
        }
        
        private void HandleHostBtnClick()
        {
            if (CheckInputValidation() == false)
            {
                Debug.Log("올바르지 않은 IP와 Port주소입니다.");
                return;
            }
            
            SetUpTransport();

            if (NetworkManager.Singleton.StartHost())
            {
                NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GameScene, LoadSceneMode.Single);
            }
            else
            {
                Debug.LogError("호스트 생성 실패");
                NetworkManager.Singleton.Shutdown();
            }
        }


        private void HandleClientBtnClick()
        {
            if (CheckInputValidation() == false)
            {
                Debug.Log("올바르지 않은 IP와 Port주소입니다.");
                return;
            }
            
            SetUpTransport();

            if (NetworkManager.Singleton.StartClient())
            {
                return;
            }
            
            NetworkManager.Singleton.Shutdown();
        }
        
        private void SetUpTransport()
        {
            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            ushort portNumber = ushort.Parse(postInputField.text);
            transport.SetConnectionData(ipInputField.text, portNumber);
        }
        
        private bool CheckInputValidation()
        {
            string ip = ipInputField.text;
            string post = postInputField.text;

            Regex ipReg = new Regex(@"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$");
            Regex postReg = new Regex(@"[0-9]{3,5}");
            
            Match ipMatch = ipReg.Match(ip);
            Match postMatch = postReg.Match(post);
            
            return ipMatch.Success && postMatch.Success;
        }
    }
}