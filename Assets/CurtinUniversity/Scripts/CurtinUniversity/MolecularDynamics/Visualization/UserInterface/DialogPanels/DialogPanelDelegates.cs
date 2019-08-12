
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void SetButtonTextDelegate(string buttonName);
    public delegate void OnFileBrowserButtonClick(string text);
    public delegate void OnFileBrowserButtonDoubleClick(string text);
    public delegate void OnFileBrowserParentDirectoryButtonClick();
    public delegate void SetParentDirectoryDelegate();
    public delegate void OnFileBrowserOpenFileSubmit(string fileName, string filePath);
    public delegate void OnFileBrowserSaveFileSubmit(string fileName, string filePath);
}
