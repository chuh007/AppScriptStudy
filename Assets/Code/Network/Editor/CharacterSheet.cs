using System;
using System.Collections.Generic;
using System.Linq;
using Code.Characters;
using Defective.JSON;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Network.Editor
{
    public class CharacterSheet : SheetManager
    {
        public CharacterDataListSO _dataList;
        private readonly string _soPath = "Assets/08SO";
        
        private TextField _urlField;
        
        public override void Initialize(VisualElement root, ExcelSheetManager excelSheetManager)
        {
            base.Initialize(root, excelSheetManager);
            _urlField = root.parent.Q<TextField>("UrlField");
            if(_dataList == null)
                FindCharacterDataList();

            root.Q<Button>("UploadBtn").clicked += UploadToSheet;
            root.Q<Button>("DownloadBtn").clicked += DownloadFromSheet;
            root.Q<Button>("ImageUploadBtn").clicked += UploadImageToServer;
            // 다운로드는 나중에.
            
            RefreshAssets();
        }

        private void FindCharacterDataList()
        {
            string path = $"{_soPath}/Character/list.asset";
            _dataList = AssetDatabase.LoadAssetAtPath<CharacterDataListSO>(path);

            if (_dataList == null)
            {
                _dataList = ScriptableObject.CreateInstance<CharacterDataListSO>();
                AssetDatabase.CreateAsset(_dataList, path);
                Debug.Log($"CharacterDataListSO created at {path}");
            }
        }

        private void RefreshAssets()
        {
            string path = $"{_soPath}/Character";
            string[] assetGuids = AssetDatabase.FindAssets("", new[] { path });
            // 해당 경로에 있는 모든 에셋을 가져온다. 폴더도 포함

            _dataList.characterList = assetGuids.Select(
                guid =>
            {
                string dataPath = AssetDatabase.GUIDToAssetPath(guid);
                CharacterDataSO data = AssetDatabase.LoadAssetAtPath<CharacterDataSO>(dataPath);
                return data;
            }).Where(data => data != null).ToList();
            
            EditorUtility.SetDirty(_dataList);
            AssetDatabase.SaveAssets();
        }

        public override async void UploadToSheet()
        {
            _excelManager.IsLoading = true;
            string result = await _excelManager.SendPostRequest(_urlField.value, _dataList.ToJson(), RequestType.UPLOAD);
            Debug.Log(result);
            _excelManager.IsLoading = false;
            _excelManager.ToastMessage.Show("업로드 완료!", 3f);
        }

        public override async void DownloadFromSheet()
        {
            _excelManager.IsLoading = true;
            string result = await _excelManager.SendGetRequest(_urlField.value);

            try
            {
                JSONObject jsonObject = new JSONObject(result);
                int created = 0;
                int updated = 0;
                List<CharacterDataSO> needToUpdateList = new List<CharacterDataSO>();
                List<string> updatedGuid = new List<string>();
                foreach (var item in jsonObject)
                {
                    string characterName = item.GetField("characterName").stringValue;
                    float moveSpeed = item.GetField("moveSpeed").floatValue;
                    int maxHealth = item.GetField("maxHealth").intValue;
                    string guid = item.GetField("guid").stringValue;
                    
                    CharacterDataSO targetData = _dataList.FindCharacterByGuid(guid);
                    if (targetData == null)
                    {
                        created++;
                        targetData = ScriptableObject.CreateInstance<CharacterDataSO>();
                        AssetDatabase.CreateAsset(targetData, $"{_soPath}/Character/{characterName}.asset");
                        _dataList.characterList.Add(targetData);
                        if (string.IsNullOrEmpty(guid))
                        {
                            targetData.GenerateGUID();
                            needToUpdateList.Add(targetData);
                        }
                        else
                            targetData.guid = guid;
                        EditorUtility.SetDirty(_dataList);
                    }
                    else
                    {
                        updated++;
                    }

                    updatedGuid.Add(targetData.guid);
                    
                    targetData.maxHealth = maxHealth;
                    targetData.moveSpeed = moveSpeed;
                    targetData.characterName = characterName;
                    EditorUtility.SetDirty(targetData);
                    // 에셋의 이름이 변경되었으면 에셋의 파일명도 같이 변경되어야 하니까.
                    if (targetData.name != characterName)
                    {
                        string assetPath = AssetDatabase.GetAssetPath(targetData);
                        AssetDatabase.RenameAsset(assetPath, characterName);
                        // 예외처리 안할거다.
                    }
                }

                foreach (var item in _dataList.characterList.ToList())
                {
                    if (updatedGuid.Any(guid => guid == item.guid) == false)
                    {
                        bool isDelete = await _excelManager.PopupPanel.ShowPopup("파일 삭제 확인", item.characterName);

                        if (isDelete)
                        {
                            string targetPath = AssetDatabase.GetAssetPath(item);
                            AssetDatabase.DeleteAsset(targetPath);
                            _dataList.characterList.Remove(item);
                            EditorUtility.SetDirty(_dataList);
                        }
                    }
                }
                
                AssetDatabase.SaveAssets();
                //최종적으로 업데이트 몇개, 생성 몇개인지를 출력.
                string updateJson = CreateUploadJson(needToUpdateList);
                _ = _excelManager.SendPostRequest(_urlField.value, updateJson, RequestType.UPDATE);
                Debug.Log(updateJson);
                _excelManager.ToastMessage.Show($"Updated : {updated}, Created : {created}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            _excelManager.IsLoading = false;
            
        }

        private void UploadImageToServer()
        {
            JSONObject payload = new JSONObject();
            foreach (var item in _dataList.characterList)
            {
                JSONObject itemObject = new JSONObject();
                Texture2D texture = GetTextureFromSprite(item.CharacterImage);
                byte[] pngBytes = texture.EncodeToPNG();
                string base64Image = Convert.ToBase64String(pngBytes); // 바이트 스트림을 base65 스트링으로 변경해준다.
                itemObject.AddField("guid", item.guid);
                itemObject.AddField("image", base64Image);
                
                Debug.Log(base64Image);
                payload.Add(itemObject);
            }
            Debug.Log(payload.ToString());
        }

        private Texture2D GetTextureFromSprite(Sprite sprite)
        {
            Rect rect = sprite.textureRect; // 텍스쳐의 크기를 알아내고
            Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);
            Color[] pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }
        
        private string CreateUploadJson(List<CharacterDataSO> needToUpdateList)
        {
            JSONObject updateJsonObject = new JSONObject();
            JSONObject array = new JSONObject();
            foreach (var item in needToUpdateList)
            {
                JSONObject characterData = new JSONObject(item.ToJson()); // 각 아이템은 Json화 시킨다.
                array.Add(characterData);
            }
            updateJsonObject.AddField("list", array);

            return updateJsonObject.ToString(); // 최종 결과물을 json string으로 변경해서 보내준다.
        }
    }
}