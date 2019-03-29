using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class FileBrowser : MonoBehaviour {

        public GameObject fileNameButtonPrefab;
        public GameObject dirNameButtonPrefab;
        public GameObject parentDirButtonPrefab;
        public GameObject driveNameButtonPrefab;

        public GameObject directoryContent;
        public ScrollRect scrollView;
        public Text currentDirectoryText;
        public Text fileNameField;

        public InputField startFrameInput;
        public InputField frameCountInput;
        public InputField frameFrequencyInput;

        private SceneManager sceneManager;

        private string currentFilePath;
        private int currentDirectoryObjectCount = 0;
        private int scrollStepCount = 5;
        private string defaultFileName = "Select a file...";

        private string playerPrefsCurrentFilePathKey = @"CurrentFilePath";

        private string tempFilePath = @"D:\Molecule Examples\";

        void Start() {

            sceneManager = SceneManager.instance;

            currentFilePath = "";
            currentDirectoryText.text = currentFilePath;
            displayDrives();
            SetFileName(defaultFileName);

            if (PlayerPrefs.HasKey(playerPrefsCurrentFilePathKey)) {
                string filePath = PlayerPrefs.GetString(playerPrefsCurrentFilePathKey);
                if (filePath != "" && Directory.Exists(filePath)) {
                    SetRootDirectory(filePath);
                }
            }

            startFrameInput.text = Settings.DefaultTrajectoryStartFrame.ToString();
            frameCountInput.text = Settings.DefaultTrajectoryFrameCount.ToString();
            frameFrequencyInput.text = Settings.DefaultTrajectoryFrameFrequency.ToString();
        }



        public void SetFileName(string fileName) {
            fileNameField.text = fileName;
        }

        public void SetChildDirectory(string childDirectoryName) {

            if (currentFilePath != null && currentFilePath != "" && !currentFilePath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                currentFilePath += Path.DirectorySeparatorChar;
            }

            currentFilePath += childDirectoryName;
            currentDirectoryText.text = currentFilePath + Path.DirectorySeparatorChar;
            clearDirectoryView();
            displayCurrentDirectory();
            SetFileName(defaultFileName);

            ScrollDirectoryToTop();

            PlayerPrefs.SetString(playerPrefsCurrentFilePathKey, currentFilePath);
        }

        public void SetRootDirectory(string rootDirectoryName) {

            currentFilePath = rootDirectoryName;
            currentDirectoryText.text = currentFilePath;
            clearDirectoryView();
            displayCurrentDirectory();
            SetFileName(defaultFileName);

            ScrollDirectoryToTop();

            PlayerPrefs.SetString(playerPrefsCurrentFilePathKey, currentFilePath);
        }

        public void SetParentDirectory() {

            DirectoryInfo parentInfo = null;

            if (currentFilePath != null && currentFilePath != "") {

                try {
                    string tempFilePath = currentFilePath.TrimEnd(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                    parentInfo = Directory.GetParent(tempFilePath);
                }
                catch (NullReferenceException) {
                    // do nothing
                }
            }

            if (parentInfo == null) {

                currentFilePath = "";
                currentDirectoryText.text = currentFilePath;
                SetFileName(defaultFileName);
                clearDirectoryView();
                displayDrives();
            }
            else {

                SetFileName(defaultFileName);
                currentFilePath = parentInfo.FullName;
                currentDirectoryText.text = currentFilePath;
                if (!currentFilePath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                    currentDirectoryText.text += Path.DirectorySeparatorChar;
                }
                clearDirectoryView();
                displayCurrentDirectory();
            }

            ScrollDirectoryToTop();

            PlayerPrefs.SetString(playerPrefsCurrentFilePathKey, currentFilePath);
        }

        public void LoadButtonSelected() {

            string fileName = fileNameField.text;
            string filePath = currentFilePath;

            if (fileName == null || fileName.Trim() == "" || filePath == null || filePath.Trim() == "") {
                return;
            }

            string fullPath = currentFilePath + Path.DirectorySeparatorChar + fileName;

            if(fullPath.EndsWith(Settings.GromacsFileExtension)) {

                int startFrame = Settings.DefaultTrajectoryStartFrame;
                int frameCount = Settings.DefaultTrajectoryFrameCount;
                int frameFrequency = Settings.DefaultTrajectoryFrameFrequency;

                Int32.TryParse(startFrameInput.text, out startFrame);
                Int32.TryParse(frameCountInput.text, out frameCount);
                Int32.TryParse(frameFrequencyInput.text, out frameFrequency);

                StartCoroutine(sceneManager.LoadGromacsFile(fullPath, startFrame, frameCount, frameFrequency));
                return;
            }

            foreach (string ext in Settings.StructureFileExtensions) {
                if (fullPath.EndsWith(ext)) {
                    StartCoroutine(sceneManager.LoadStructure(fullPath));
                    return;
                }
            }

            foreach (string ext in Settings.TrajectoryFileExtensions) {

                if (fullPath.EndsWith(ext)) {

                    int startFrame = Settings.DefaultTrajectoryStartFrame;
                    int frameCount = Settings.DefaultTrajectoryFrameCount;
                    int frameFrequency = Settings.DefaultTrajectoryFrameFrequency;

                    Int32.TryParse(startFrameInput.text, out startFrame);
                    Int32.TryParse(frameCountInput.text, out frameCount);
                    Int32.TryParse(frameFrequencyInput.text, out frameFrequency);

                    StartCoroutine(sceneManager.LoadTrajectory(fullPath, startFrame, frameCount, frameFrequency));
                    return;
                }
            }

            foreach (string ext in Settings.ColourFileExtensions) {

                if (fullPath.EndsWith(ext)) {

                    int startFrame = Settings.DefaultTrajectoryStartFrame;
                    int frameCount = Settings.DefaultTrajectoryFrameCount;
                    int frameFrequency = Settings.DefaultTrajectoryFrameFrequency;

                    Int32.TryParse(startFrameInput.text, out startFrame);
                    Int32.TryParse(frameCountInput.text, out frameCount);
                    Int32.TryParse(frameFrequencyInput.text, out frameFrequency);

                    StartCoroutine(sceneManager.LoadTrajectoryColour(fullPath, startFrame, frameCount, frameFrequency));
                }
            }
        }

        public void ScrollDirectoryToTop() {
            scrollView.verticalNormalizedPosition = 1;
        }

        public void ScrollDirectoryUp() {

            //Debug.Log("scroll up selected");
            float scrollAmount = (1.0f / currentDirectoryObjectCount) * scrollStepCount;
            scrollView.verticalNormalizedPosition += scrollAmount;
            if (scrollView.verticalNormalizedPosition > 1) {
                scrollView.verticalNormalizedPosition = 1;
            }
        }

        public void ScrollDirectoryDown() {

            //Debug.Log("scroll down selected");
            float scrollAmount = (1.0f / currentDirectoryObjectCount) * scrollStepCount;
            scrollView.verticalNormalizedPosition -= scrollAmount;
            if (scrollView.verticalNormalizedPosition < 0) {
                scrollView.verticalNormalizedPosition = 0;
            }
        }

        public void ReloadDirectory() {

            clearDirectoryView();

            if (currentFilePath == "") {
                displayDrives();
            }
            else {
                displayCurrentDirectory();
            }

            ScrollDirectoryToTop();
        }

        public void FrameStartStartEdit() {
            sceneManager.InputManager.KeyboardUIControlEnabled = false;
            sceneManager.InputManager.KeyboardSceneControlEnabled = false;
        }

        public void FrameStartEndEdit() {

            sceneManager.InputManager.KeyboardUIControlEnabled = true;
            sceneManager.InputManager.KeyboardSceneControlEnabled = true;

            int startFrame = 0;
            if (Int32.TryParse(startFrameInput.text, out startFrame)) {
                if (startFrame < Settings.MinTrajectoryStartFrame) {
                    startFrame = Settings.MinTrajectoryStartFrame;
                    sceneManager.GUIManager.ShowConsoleMessage("Start frame less than minimum allowed. Setting to minimum.");
                }
                else if (startFrame > Settings.MaxTrajectoryStartFrame) {
                    startFrame = Settings.MaxTrajectoryStartFrame;
                    sceneManager.GUIManager.ShowConsoleMessage("Start frame greater than maximum allowed. Setting to maximum.");
                }
            }
            else {
                startFrame = Settings.DefaultTrajectoryStartFrame;
                sceneManager.GUIManager.ShowConsoleMessage("Start frame is not a integer. Setting to default.");
            }

            startFrameInput.text = startFrame.ToString();
        }

        public void FrameCountStartEdit() {
            sceneManager.InputManager.KeyboardUIControlEnabled = false;
            sceneManager.InputManager.KeyboardSceneControlEnabled = false;
        }

        public void FrameCountEndEdit() {

            sceneManager.InputManager.KeyboardUIControlEnabled = true;
            sceneManager.InputManager.KeyboardSceneControlEnabled = true;

            int frameCount = 0;
            if (Int32.TryParse(frameCountInput.text, out frameCount)) {
                if (frameCount < Settings.MinTrajectoryFrameCount) {
                    frameCount = Settings.MinTrajectoryFrameCount;
                    sceneManager.GUIManager.ShowConsoleMessage("Frame count less than minimum allowed. Setting to minimum.");
                }
                else if (frameCount > Settings.MaxTrajectoryFrameCount) {
                    frameCount = Settings.MaxTrajectoryFrameCount;
                    sceneManager.GUIManager.ShowConsoleMessage("Frame count greater than maximum allowed. Setting to maximum.");
                }
            }
            else {
                frameCount = Settings.DefaultTrajectoryFrameCount;
                sceneManager.GUIManager.ShowConsoleMessage("Frame count is not a integer. Setting to default.");
            }

            frameCountInput.text = frameCount.ToString();
        }

        public void FrameFrequencyStartEdit() {
            sceneManager.InputManager.KeyboardUIControlEnabled = false;
            sceneManager.InputManager.KeyboardSceneControlEnabled = false;
        }

        public void FrameFrequencyEndEdit() {

            sceneManager.InputManager.KeyboardUIControlEnabled = true;
            sceneManager.InputManager.KeyboardSceneControlEnabled = true;

            int frameFrequency = 0;
            if (Int32.TryParse(frameFrequencyInput.text, out frameFrequency)) {
                if (frameFrequency < Settings.MinTrajectoryFrameFrequency) {
                    frameFrequency = Settings.MinTrajectoryFrameFrequency;
                    sceneManager.GUIManager.ShowConsoleMessage("Frame frequency less than minimum allowed. Setting to minimum.");
                }
                else if (frameFrequency > Settings.MaxTrajectoryFrameFrequency) {
                    frameFrequency = Settings.MaxTrajectoryFrameFrequency;
                    sceneManager.GUIManager.ShowConsoleMessage("Frame frequency greater than maximum allowed. Setting to maximum.");
                }
            }
            else {
                frameFrequency = Settings.DefaultTrajectoryFrameFrequency;
                sceneManager.GUIManager.ShowConsoleMessage("Frame frequency is not a integer. Setting to default.");
            }

            frameFrequencyInput.text = frameFrequency.ToString();
        }

        private void clearDirectoryView() {

            foreach (Transform child in directoryContent.transform) {
                Destroy(child.gameObject);
            }
        }

        private void displayDrives() {

            // DriveInfo doesn't work with Unity C#
            // DriveInfo[] info = DriveInfo.GetDrives();

            string[] drives = Directory.GetLogicalDrives();

            currentDirectoryObjectCount = 0;

            foreach (string drive in drives) {

                GameObject driveNameButton = Instantiate(driveNameButtonPrefab);
                Text buttonText = driveNameButton.transform.Find("Text").GetComponent<Text>();
                buttonText.text = drive;
                driveNameButton.transform.SetParent(directoryContent.transform);
                resetTransform(driveNameButton);

                FileNameButton buttonScript = driveNameButton.GetComponent<FileNameButton>();
                SetButtonTextDelegate callback = new SetButtonTextDelegate(SetRootDirectory);
                buttonScript.SetCallback(callback);

                currentDirectoryObjectCount++;
            }
        }

        private void displayCurrentDirectory() {

            currentDirectoryObjectCount = 0;

            // always put an up directory button to allow back out in case of issues displaying directory
            {
                GameObject upDirButton = Instantiate(parentDirButtonPrefab);
                upDirButton.transform.SetParent(directoryContent.transform);
                resetTransform(upDirButton);

                ParentDirButton buttonScript = upDirButton.GetComponent<ParentDirButton>();
                SetParentDirectoryDelegate callback = new SetParentDirectoryDelegate(SetParentDirectory);
                buttonScript.SetCallback(callback);

                currentDirectoryObjectCount++;
            }

            try {

                DirectoryInfo dinfo = new DirectoryInfo(currentFilePath);
                DirectoryInfo[] directories = dinfo.GetDirectories();

                foreach (DirectoryInfo directory in directories) {

                    if ((directory.Attributes & FileAttributes.Hidden) == 0) {

                        GameObject dirNameButton = Instantiate(dirNameButtonPrefab);
                        Text buttonText = dirNameButton.transform.Find("Text").GetComponent<Text>();
                        buttonText.text = directory.Name;
                        dirNameButton.transform.SetParent(directoryContent.transform);
                        resetTransform(dirNameButton);

                        FileNameButton buttonScript = dirNameButton.GetComponent<FileNameButton>();
                        SetButtonTextDelegate callback = new SetButtonTextDelegate(SetChildDirectory);
                        buttonScript.SetCallback(callback);

                        currentDirectoryObjectCount++;
                    }
                }

                FileInfo[] files = dinfo.GetFiles().Where(f => Settings.ValidExtensions.Contains(f.Extension.ToLower())).ToArray();

                foreach (FileInfo file in files) {

                    GameObject fileNameButton = Instantiate(fileNameButtonPrefab);
                    Text buttonText = fileNameButton.transform.Find("Text").GetComponent<Text>();
                    buttonText.text = file.Name;
                    fileNameButton.transform.SetParent(directoryContent.transform);
                    resetTransform(fileNameButton);

                    FileNameButton buttonScript = fileNameButton.GetComponent<FileNameButton>();
                    SetButtonTextDelegate callback = new SetButtonTextDelegate(SetFileName);
                    buttonScript.SetCallback(callback);

                    currentDirectoryObjectCount++;
                }

            }
            catch (Exception ex) {
                // doesn't matter what exception. We always want to fail gracefully.
                sceneManager.GUIManager.ShowConsoleMessage("Error loading directory [" + ex.Message + "]");
            }
        }

        private void resetTransform(GameObject go) {

            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localRotation = Quaternion.identity;
        }
    }
}
