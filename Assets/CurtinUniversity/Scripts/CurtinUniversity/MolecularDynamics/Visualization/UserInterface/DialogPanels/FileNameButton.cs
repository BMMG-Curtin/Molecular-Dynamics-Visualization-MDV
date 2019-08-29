
using UnityEngine;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class FileNameButton : MonoBehaviour {

        public TextMeshProUGUI ButtonText;

        private OnFileBrowserButtonClick onClick;
        private OnFileBrowserButtonDoubleClick onDoubleClick;
        private string pathName;

        private float lastClickTime = 0;
        private float doubleClickTimeout = 0.5f;

        public void Initialise(string pathName, OnFileBrowserButtonClick onClick, OnFileBrowserButtonDoubleClick onDoubleClick) {

            ButtonText.text = pathName;

            this.pathName = pathName;
            this.onClick = onClick;
            this.onDoubleClick = onDoubleClick;
        }

        public void OnClick() {

            if (onDoubleClick != null) {

                if (Time.time <= lastClickTime + doubleClickTimeout) {

                    onDoubleClick(pathName);
                    lastClickTime = Time.time;
                    return;
                }

                lastClickTime = Time.time;
            }


            if (onClick != null) {
                onClick(pathName);
            }
        }
    }
}
