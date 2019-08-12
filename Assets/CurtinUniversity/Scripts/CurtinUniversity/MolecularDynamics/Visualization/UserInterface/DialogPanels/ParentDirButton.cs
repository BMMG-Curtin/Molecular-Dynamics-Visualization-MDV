using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ParentDirButton : MonoBehaviour {

        OnFileBrowserParentDirectoryButtonClick onClick;

        public void Initialise(OnFileBrowserParentDirectoryButtonClick onClick) {
            this.onClick = onClick;
        }

        public void ParentDirClick() {

            if (onClick != null) {
                onClick();
            }
        }
    }
}
