using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace Code.Network.Editor
{
    public enum RequestType
    {
        UPLOAD, UPDATE
    }
    
    public class ExcelSheetManager : EditorWindow
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;

        private CharacterSheet _characterSheet;
        private VisualElement _loadingScreen;
        
        public ToastMessage ToastMessage { get; private set; }
        public PopupPanel PopupPanel { get; private set; }
        
        public bool IsLoading
        {
            get => _loadingScreen.ClassListContains("on");
            set => _loadingScreen.EnableInClassList("on", value);
        }
        
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

            visualTreeAsset.CloneTree(root);
            _loadingScreen = root.Q<VisualElement>("LoadingScreen");
            ToastMessage = new ToastMessage(root.Q<VisualElement>("Toast"));
            PopupPanel = new PopupPanel(root.Q<TemplateContainer>("Popup"));
            
            VisualElement characterContainer = root.Q<VisualElement>("CharacterContainer");
            _characterSheet.Initialize(characterContainer, this); // 컨테이너 넣어서 초기화해준다.
        }

        public async Task<string> SendPostRequest(string url, string payload, RequestType type)
        {
            WWWForm form = new WWWForm();
            form.AddField("payload", payload);
            form.AddField("type", type.ToString().ToLower());
            
            UnityWebRequest www = UnityWebRequest.Post(url, form);
            var asyncOperation = www.SendWebRequest();
            await asyncOperation;
            return asyncOperation.isDone ? www.downloadHandler.text : "Error : Connection failed";
        }

        public async Task<string> SendGetRequest(string url)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            var asyncOperation = www.SendWebRequest();
            await asyncOperation;
            
            return asyncOperation.isDone ? www.downloadHandler.text : "Error : Connection failed";
        }
    }
}
