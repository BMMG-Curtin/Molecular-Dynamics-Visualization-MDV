using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ParentDirButton : MonoBehaviour {

        SetParentDirectoryDelegate parentDirCallback;

        public void SetCallback(SetParentDirectoryDelegate callback) {
            parentDirCallback = callback;
        }

        public void ParentDirClick() {

            if (parentDirCallback != null) {
                parentDirCallback();
            }
        }
    }
}
