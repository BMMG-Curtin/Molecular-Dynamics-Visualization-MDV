
using UnityEngine;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ConfirmDialog : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI messageText;

        private OnConfirmDialogSubmit onSubmit;

        public void Initialise(string message, OnConfirmDialogSubmit onSubmit) {

            messageText.text = message;
            this.onSubmit = onSubmit;
        }

        public void OnYesButton() {

            onSubmit(true);
            gameObject.SetActive(false);
        }

        public void OnNoButton() {

            onSubmit(false);
            gameObject.SetActive(false);
        }
    }
}
