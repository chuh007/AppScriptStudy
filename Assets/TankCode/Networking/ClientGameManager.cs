using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

namespace TankCode.Networking
{
    public class ClientGameManager
    {
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
        
        public void ChangeScene(string sceneName)
            => SceneManager.LoadScene(sceneName);
    }
}