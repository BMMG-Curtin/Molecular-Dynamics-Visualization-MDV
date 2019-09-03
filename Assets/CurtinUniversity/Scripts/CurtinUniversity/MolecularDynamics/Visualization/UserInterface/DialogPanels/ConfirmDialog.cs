
using UnityEngine;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ConfirmDialog : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI messageText;

        private OnConfirmDialogSubmit onSubmit;
        private object data;

        public void Initialise(string message, OnConfirmDialogSubmit onSubmit, object data = null) {

            messageText.text = message;
            this.onSubmit = onSubmit;
            this.data = data;
        }

        public void OnYesButton() {

            onSubmit(true, data);
            gameObject.SetActive(false);
        }

        public void OnNoButton() {

            onSubmit(false, data);
            gameObject.SetActive(false);
        }
    }
}
