using System;
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class Screenshot : MonoBehaviour {

        [SerializeField]
        private KeyCode screenshotKey = KeyCode.Print;

        [SerializeField]
        private KeyCode altScreenshotKey = KeyCode.SysReq;

        [SerializeField]
        private SceneManager sceneManager;

        public void Update() {

            if (Input.GetKeyDown(screenshotKey) || Input.GetKeyDown(altScreenshotKey)) {
                
                string timeStamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                string path = Application.streamingAssetsPath + "/Screenshots/" + "Screenshot_" + timeStamp + ".png";
                ScreenCapture.CaptureScreenshot(path);

                sceneManager.ShowConsoleMessage("Screenshot captured: " + path, false);
            }
        }
    }
}