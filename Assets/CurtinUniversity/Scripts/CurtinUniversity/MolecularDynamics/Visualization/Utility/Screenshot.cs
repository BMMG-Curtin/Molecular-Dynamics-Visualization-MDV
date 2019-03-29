using System;
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization.Utility {

    public class Screenshot : MonoBehaviour {

        public KeyCode ScreenshotKey = KeyCode.Print;

        public void Update() {

            if (Input.GetKeyDown(ScreenshotKey)) {
                
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string path = Application.streamingAssetsPath + "/Screenshots/" + "Screenshot_" + timeStamp + ".png";
                ScreenCapture.CaptureScreenshot(path);

                SceneManager.instance.GUIManager.Console.ShowMessage("Screenshot captured: " + path);
            }
        }
    }
}