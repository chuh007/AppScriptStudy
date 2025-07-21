using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace Code.Network.Editor
{
    public class ExcelSheetManager : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;

        private CharacterSheet _characterSheet;

        private void OnEnable()
        {
            if (_characterSheet == null)
                _characterSheet = new CharacterSheet();
        }

        [MenuItem("Tools/ExcelSheetManager")]
        public static void ShowWindow()
        {
            ExcelSheetManager wnd = GetWindow<ExcelSheetManager>();
            wnd.titleContent = new GUIContent("ExcelSheetManager");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
        
            VisualElement treeAsset = visualTreeAsset.Instantiate();
            root.Add(treeAsset);
            VisualElement loading = root.Q<VisualElement>("LoadingScreen");
            loading.visible = false;
            VisualElement characterContainer = root.Q<VisualElement>("CharacterContainer");
            _characterSheet.Initialize(characterContainer, this); // 컨테이너 넣어서 초기화해준다.
        }

        public async Task<string> SendPostRequest(string url, string payload)
        {
            WWWForm form = new WWWForm();
            form.AddField("payload", payload);
            
            VisualElement root = rootVisualElement;
            VisualElement loading = root.Q<VisualElement>("LoadingScreen");
            loading.visible = true;
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            var asyncOperation = www.SendWebRequest();
            await asyncOperation;
            loading.visible = false;
            return asyncOperation.isDone ? www.downloadHandler.text : "Error : Connection failed";
        }
    }
}
