using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueAtomNameButton : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI atomNameText;

        [SerializeField]
        private Image backgroundImage;

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
