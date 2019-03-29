using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class FileNameButton : MonoBehaviour {

        SetButtonTextDelegate fileNameCallback;

        public void SetCallback(SetButtonTextDelegate callback) {
            fileNameCallback = callback;
        }

        public void FileNameClick() {

            if (fileNameCallback != null) {
                string buttonText = transform.Find(@"Text").GetComponent<Text>().text;
                fileNameCallback(buttonText);
            }
        }
    }
}
