using UnityEngine.UIElements;

namespace Code.Network.Editor
{
    public abstract class SheetManager
    {
        protected ExcelSheetManager _excelManager;

        public virtual void Initialize(VisualElement root, ExcelSheetManager excelManager)
        {
            _excelManager = excelManager;
        }

        public abstract void UploadToSheet();

        public abstract void DownloadFromSheet();
    }
}