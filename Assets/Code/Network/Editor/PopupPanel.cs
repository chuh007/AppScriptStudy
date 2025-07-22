using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Network.Editor
{
    public class PopupPanel
    {
        private VisualElement _root;
        
        private Label _titleLabel;
        private Label _contentLabel;
        private AwaitableCompletionSource<bool> _deleteConfirmSource;
        
        public string Title
        {
            get => _titleLabel.text;
            set => _titleLabel.text = value;
        }

        public string Content
        {
            get => _contentLabel.text;
            set => _contentLabel.text = value;
        }

        public bool IsShow
        {
            get => _root.ClassListContains("on");
            set => _root.EnableInClassList("on", value);
        }
        
        public PopupPanel(VisualElement popupRoot)
        {
            _root = popupRoot;
            _titleLabel = _root.Q<Label>("TitleLabel");
            _contentLabel = _root.Q<Label>("ContentLabel");
            
            _deleteConfirmSource = new AwaitableCompletionSource<bool>();

            _root.Q<Button>("KeepBtn").clicked += HandleKeep;
            _root.Q<Button>("DeleteBtn").clicked += HandleDelete;
        }

        public async Task<bool> ShowPopup(string title, string content)
        {
            Title = title;
            Content = content;
            IsShow = true;
            
            _deleteConfirmSource.Reset();
            bool result = await _deleteConfirmSource.Awaitable;
            IsShow = false;
            
            return result;
        }
        
        private void HandleKeep()
        {
            _deleteConfirmSource.SetResult(false);
        }
        
        private void HandleDelete()
        {
            _deleteConfirmSource.SetResult(true);
        }
    }
}