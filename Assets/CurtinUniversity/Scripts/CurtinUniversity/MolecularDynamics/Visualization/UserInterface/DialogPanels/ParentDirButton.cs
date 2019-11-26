
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    /// <summary>
    /// ParentDirButton is used in the load and save file dialogs
    /// </summary>
    public class ParentDirButton : MonoBehaviour {

        private OnFileBrowserParentDirectoryButtonClick onClick;

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
