﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;
using System.Linq;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// This is mostly just a duplicate of the LoadFileDialog class with a few small changes to allow for selecting a save file
    /// </summary>
    public class SaveFileDialog : MonoBehaviour {

        [SerializeField]
        ConfirmDialog ConfirmDialog;

        [SerializeField]
        private GameObject FileNameButtonPrefab;

        [SerializeField]
        private GameObject DirNameButtonPrefab;

        [SerializeField]
        private GameObject ParentDirButtonPrefab;

        [SerializeField]
        private GameObject DriveNameButtonPrefab;

        [SerializeField]
        private GameObject DirectoryContent;

        [SerializeField]
        private ScrollRect ScrollView;

        [SerializeField]
        private TextMeshProUGUI CurrentDirectoryText;

        [SerializeField]
        private TMP_InputField FileNameInputField;

        private string currentFilePath;
        private OnFileBrowserSaveFileSubmit onSubmit;
        private List<string> validExtensions;

        private int currentDirectoryObjectCount = 0;
        private int scrollStepCount = 5;

        private string playerPrefsCurrentFilePathKey = @"CurrentFilePath";

        // used for saving between confirmation dialog
        private string confirmFilePath;

        public void Initialise(List<string> validFileExtensions, OnFileBrowserSaveFileSubmit onSubmit) {

            Debug.Log("Initialising File Browser");

            this.validExtensions = validFileExtensions;
            this.onSubmit = onSubmit;

            currentFilePath = "";
            CurrentDirectoryText.text = currentFilePath;

            confirmFilePath = "";

            bool setPath = false;

            if (PlayerPrefs.HasKey(playerPrefsCurrentFilePathKey)) {

                string filePath = PlayerPrefs.GetString(playerPrefsCurrentFilePathKey);
                if (filePath != "" && Directory.Exists(filePath)) {
                    SetRootDirectory(filePath);
                    setPath = true;
                }
            }

            if (!setPath) {

                string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                if (homeFolder != null && homeFolder.Trim() != "") {

                    SetRootDirectory(homeFolder);
                    setPath = true;
                }
            }

            if (!setPath) {
                displayDrives();
            }
        }

        public void SetFileName(string fileName) {
            FileNameInputField.text= fileName;
        }

        public void SubmitFileName(string fileName) {

            FileNameInputField.text = fileName;
            OnSubmitButton();
        }

        public void SetChildDirectory(string childDirectoryName) {

            if (currentFilePath != null && currentFilePath != "" && !currentFilePath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                currentFilePath += Path.DirectorySeparatorChar;
            }

            currentFilePath += childDirectoryName;
            CurrentDirectoryText.text = currentFilePath + Path.DirectorySeparatorChar;
            clearDirectoryView();
            displayCurrentDirectory();

            ScrollDirectoryToTop();

            PlayerPrefs.SetString(playerPrefsCurrentFilePathKey, currentFilePath);
        }

        public void SetRootDirectory(string rootDirectoryName) {

            currentFilePath = rootDirectoryName;
            CurrentDirectoryText.text = currentFilePath;
            clearDirectoryView();
            displayCurrentDirectory();

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
                CurrentDirectoryText.text = currentFilePath;
                clearDirectoryView();
                displayDrives();
            }
            else {

                currentFilePath = parentInfo.FullName;
                CurrentDirectoryText.text = currentFilePath;
                if (!currentFilePath.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                    CurrentDirectoryText.text += Path.DirectorySeparatorChar;
                }
                clearDirectoryView();
                displayCurrentDirectory();
            }

            ScrollDirectoryToTop();

            PlayerPrefs.SetString(playerPrefsCurrentFilePathKey, currentFilePath);
        }

        public void OnSubmitButton() {

            string fileName = FileNameInputField.text;
            string filePath = currentFilePath;

            if (fileName == null || fileName.Trim() == "" || filePath == null || filePath.Trim() == "") {
                return;
            }

            string fullPath = (currentFilePath + Path.DirectorySeparatorChar + fileName).Trim();

            if(File.Exists(fullPath)) {

                confirmFilePath = fullPath;

                ConfirmDialog.gameObject.SetActive(true);
                ConfirmDialog.Initialise("File already exists\nWould you like to overwrite the file?", onConfirmSave);
            }
            else {

                onSubmit(fullPath);
                gameObject.SetActive(false);
            }
        }

        private void onConfirmSave(bool confirmed, object data = null) {

            if (confirmed) {

                onSubmit(confirmFilePath);
                gameObject.SetActive(false);
            }
        }

        public void OnCloseDialog() {
            gameObject.SetActive(false);
        }

        public void ScrollDirectoryToTop() {
            ScrollView.verticalNormalizedPosition = 1;
        }

        public void ScrollDirectoryUp() {

            float scrollAmount = (1.0f / currentDirectoryObjectCount) * scrollStepCount;
            ScrollView.verticalNormalizedPosition += scrollAmount;
            if (ScrollView.verticalNormalizedPosition > 1) {
                ScrollView.verticalNormalizedPosition = 1;
            }
        }

        public void ScrollDirectoryDown() {

            float scrollAmount = (1.0f / currentDirectoryObjectCount) * scrollStepCount;
            ScrollView.verticalNormalizedPosition -= scrollAmount;
            if (ScrollView.verticalNormalizedPosition < 0) {
                ScrollView.verticalNormalizedPosition = 0;
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

        private void clearDirectoryView() {

            foreach (Transform child in DirectoryContent.transform) {
                Destroy(child.gameObject);
            }
        }

        private void displayDrives() {

            // DriveInfo doesn't work with Unity C#
            // DriveInfo[] info = DriveInfo.GetDrives();

            clearDirectoryView();
            string[] drives = Directory.GetLogicalDrives();

            currentDirectoryObjectCount = 0;

            foreach (string drive in drives) {

                GameObject driveNameButton = Instantiate(DriveNameButtonPrefab);
                driveNameButton.transform.SetParent(DirectoryContent.transform);
                driveNameButton.GetComponent<FileNameButton>().Initialise(drive, SetRootDirectory, null);
                resetTransform(driveNameButton);
                currentDirectoryObjectCount++;
            }
        }

        private void displayCurrentDirectory() {

            currentDirectoryObjectCount = 0;

            // always put an up directory button to allow back out in case of issues displaying directory
            {
                GameObject upDirButton = Instantiate(ParentDirButtonPrefab);
                upDirButton.transform.SetParent(DirectoryContent.transform);
                upDirButton.GetComponent<ParentDirButton>().Initialise(SetParentDirectory);
                resetTransform(upDirButton);
                currentDirectoryObjectCount++;
            }

            try {

                DirectoryInfo dinfo = new DirectoryInfo(currentFilePath);
                DirectoryInfo[] directories = dinfo.GetDirectories();

                foreach (DirectoryInfo directory in directories) {

                    if ((directory.Attributes & FileAttributes.Hidden) == 0) {

                        GameObject dirNameButton = Instantiate(DirNameButtonPrefab);
                        dirNameButton.transform.SetParent(DirectoryContent.transform);
                        dirNameButton.GetComponent<FileNameButton>().Initialise(directory.Name, SetChildDirectory, null);
                        resetTransform(dirNameButton);
                        currentDirectoryObjectCount++;
                    }
                }

                FileInfo[] files;
                if (validExtensions != null) {
                    files = dinfo.GetFiles().Where(f => validExtensions.Contains(f.Extension.ToLower())).ToArray();
                }
                else {
                    files = dinfo.GetFiles();
                }

                foreach (FileInfo file in files) {

                    GameObject fileNameButton = Instantiate(FileNameButtonPrefab);
                    fileNameButton.transform.SetParent(DirectoryContent.transform);
                    fileNameButton.GetComponent<FileNameButton>().Initialise(file.Name, SetFileName, SubmitFileName);
                    resetTransform(fileNameButton);
                    currentDirectoryObjectCount++;
                }
            }
            catch (Exception ex) {
                // doesn't matter what exception. We always want to fail gracefully.
                //SceneManager.GUIManager.ShowConsoleMessage("Error loading directory [" + ex.Message + "]");
                Debug.Log("Error loading directory [" + ex.Message + "]");
            }
        }

        private void resetTransform(GameObject go) {

            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localRotation = Quaternion.identity;
        }
    }
}
