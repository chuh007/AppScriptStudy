using System.Collections.Generic;
using Code.Network;
using Defective.JSON;
using UnityEngine;

namespace Code.Characters
{
    [CreateAssetMenu(fileName = "Character List", menuName = "SO/ CharacterList", order = 0)]
    public class CharacterDataListSO : ScriptableObject, IToJsonable
    {
        public List<CharacterDataSO> characterList;
        
        public string ToJson()
        {
            JSONObject jsonObject = new JSONObject();
            JSONObject jsonArray = new JSONObject();

            foreach (var character in characterList)
            {
                JSONObject characterJson = new JSONObject(character.ToJson());
                jsonArray.Add(characterJson);
            }
            jsonObject.AddField("list", jsonArray);
            
            return jsonObject.ToString();
        }

        public CharacterDataSO FindCharacterByGuid(string guid)
            => characterList.Find(x => x.guid == guid);
    }
}