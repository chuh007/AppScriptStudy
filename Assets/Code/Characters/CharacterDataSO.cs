using Code.Network;
using Defective.JSON;
using UnityEditor;
using UnityEngine;

namespace Code.Characters
{
    [CreateAssetMenu(fileName = "Character data", menuName = "SO/CharacterData", order = 0)]
    public class CharacterDataSO : ScriptableObject, IToJsonable
    {
        public float moveSpeed;
        public string characterName;
        public int maxHealth;
        public string guid;
        public Sprite CharacterImage;
        
        public string ToJson()
        {
            JSONObject jsonObject = new JSONObject();
            jsonObject.AddField("moveSpeed", moveSpeed);
            jsonObject.AddField("characterName", characterName);
            jsonObject.AddField("maxHealth", maxHealth);
            jsonObject.AddField("guid", guid);
            
            return jsonObject.ToString(); // Json 스트링으로 만들어서 리턴해준다.
        }

        private void OnValidate()
        {
            if(string.IsNullOrEmpty(guid))
                GenerateGUID();
        }

        private void GenerateGUID()
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);
            GUID assetGuid = AssetDatabase.GUIDFromAssetPath(path);
            guid = assetGuid.ToString();
#endif
        }
    }
}