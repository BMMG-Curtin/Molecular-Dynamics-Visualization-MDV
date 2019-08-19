using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class AtomNameButton : MonoBehaviour {

        [SerializeField]
        private Text atomNameText;

        [SerializeField]
        private Image backgroundImage;

        [SerializeField]
        private Toggle Enabled;

        [SerializeField]
        private Toggle CPK;

        [SerializeField]
        private Toggle VDW;

        private AtomRenderSettings atomOptions;

        public void Initialise(AtomRenderSettings options) {

            UpdateAtomOptions(options);
        }

        public void UpdateAtomOptions(AtomRenderSettings options) {

            this.atomOptions = options;

            atomNameText.text = options.AtomName.ToString();
        }
    }
}
