using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Code.Test
{
    public class SimpleGoogleDownloader : MonoBehaviour
    {
        [SerializeField, TextArea] private string URL;

        [ContextMenu("load_from_google_drive")]
        private void Load()
        {
            StartCoroutine(DownLoadTSV());
        }

        private IEnumerator DownLoadTSV()
        {
            // Get, Post, Put, Delete
            UnityWebRequest www = UnityWebRequest.Get(URL); // 아직은 요청 발사가 안된 상태.
            yield return www.SendWebRequest(); // 이때 요청이 들어가고 응답을 기다린다.
            
            string data = www.downloadHandler.text;

            string[][] lines = data.Split("\n").Select(line => line.Split("\t").ToArray()).ToArray();
            
            StringBuilder builder = new StringBuilder();
            
            for (int i = 0; i < lines.Length; i++)
            {
                builder.Clear();
                for (int j = 0; j < lines[i].Length; j++)
                {
                    builder.Append(lines[i][j]);
                    if (j < lines[i].Length - 1)
                    {
                        builder.Append(", ");
                    }
                }
                Debug.Log($"Line {i}: {builder}");
            }
        }
    }
}
