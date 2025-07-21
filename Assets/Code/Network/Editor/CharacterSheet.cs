using System.Linq;
using Code.Characters;
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
            string result = await _excelManager.SendPostRequest(_urlField.value, _dataList.ToJson());
            Debug.Log(result);
        }

        public override void DownloadFromSheet()
        {
            
        }
    }
}