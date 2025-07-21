using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Code
{
    public class SpriteDataManager : MonoBehaviour
    {
        [SerializeField, TextArea] private string URL;
        [SerializeField] private string userName;
        [SerializeField] private float userScore;
        [SerializeField] private bool hasLicense; // 정처기 여부

        #region Get request
        
        [ContextMenu("Get request")]
        private void GetRequest()
        {
            StartCoroutine(DownloadSprite());
        }

        private IEnumerator DownloadSprite()
        {
            UnityWebRequest www = UnityWebRequest.Get(URL);
            yield return www.SendWebRequest();
            
            string data = www.downloadHandler.text;
            
            Debug.Log(data);
        }
        
        #endregion

        #region POST request

        [ContextMenu("Post request")]
        private void PostRequest()
        {
            StartCoroutine(SendPostData());
        }

        private IEnumerator SendPostData()
        {
            // 폼은 직렬화된 데이터가 날아가는거라서 무조건 스트링이다.
            WWWForm form = new WWWForm();
            form.AddField("userName", userName);
            form.AddField("userScore", userScore.ToString());
            form.AddField("hasLicense", hasLicense ? "true" : "false");
            
            UnityWebRequest www = UnityWebRequest.Post(URL, form);
            yield return www.SendWebRequest();
            
            Debug.Log(www.downloadHandler.text);
        }

        #endregion
        
    }
}