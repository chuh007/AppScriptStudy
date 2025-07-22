using UnityEngine.UIElements;

namespace Code.Network.Editor
{
    public class ToastMessage
    {
        private VisualElement _root;
        private Label _messageLabel;

        public string Message
        {
            get => _messageLabel.text;
            set => _messageLabel.text = value;
        }

        public bool IsVisible
        {
            get => _root.ClassListContains("on");
            set => _root.EnableInClassList("on", value);
        }
        
        public ToastMessage(VisualElement toastRoot)
        {
            _root = toastRoot;
            _messageLabel = _root.Q<Label>("ToastMessage");
        }

        public void Show(string message, float duration = 2f)
        {
            Message = message;
            IsVisible = true;
            _root.schedule.Execute(() =>
            {
                IsVisible = false;
            }).StartingIn((int)duration * 1000);
        }
    }
}