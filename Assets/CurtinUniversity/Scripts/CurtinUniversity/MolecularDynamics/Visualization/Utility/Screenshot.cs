using System;
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Screenshot : MonoBehaviour {

        public KeyCode ScreenshotKey = KeyCode.Print;
        public UserInterface userInterface;

        public void Update() {

            if (Input.GetKeyDown(ScreenshotKey)) {
                
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string path = Application.streamingAssetsPath + "/Screenshots/" + "Screenshot_" + timeStamp + ".png";
                ScreenCapture.CaptureScreenshot(path);

                userInterface.ShowConsoleMessage("Screenshot captured: " + path);
            }
        }
    }
}