
using UnityEngine;
using UnityEngine.UI;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueCustomColourSelect : MonoBehaviour {

        [SerializeField]
        private GameObject ColorSelectButtonPrefab;

        [SerializeField]
        private GameObject ButtonParent;

        private SetCustomColourButtonColourDelegate setColourCallback;

        private void Start() {

            addColourSelectButton(Settings.ResidueColour1);
            addColourSelectButton(Settings.ResidueColour2);
            addColourSelectButton(Settings.ResidueColour3);
            addColourSelectButton(Settings.ResidueColour4);
            addColourSelectButton(Settings.ResidueColour5);
            addColourSelectButton(Settings.ResidueColour6);
            addColourSelectButton(Settings.ResidueColour7);
            addColourSelectButton(Settings.ResidueColour8);
            addColourSelectButton(Settings.ResidueColour9);
            addColourSelectButton(Settings.ResidueColour10);
            addColourSelectButton(Settings.ResidueColour11);
            addColourSelectButton(Settings.ResidueColour12);
            addColourSelectButton(Settings.ResidueColour13);
            addColourSelectButton(Settings.ResidueColour14);
            addColourSelectButton(Settings.ResidueColour15);
        }

        public void Initialise(SetCustomColourButtonColourDelegate setColourCallback) {
            this.setColourCallback = setColourCallback;
        }

        public void OnNoColourButton() {
            saveAndCloseColourSelectPanel(null);
        }

        private void addColourSelectButton(Color colour) {

            GameObject button = (GameObject)Instantiate(ColorSelectButtonPrefab, Vector3.zero, Quaternion.identity);

            ResidueCustomColourButton buttonScript = button.GetComponent<ResidueCustomColourButton>();
            buttonScript.SetColorCallback(saveAndCloseColourSelectPanel, colour);

            ColorBlock colors = button.GetComponent<Button>().colors;
            colors.normalColor = colour;
            colors.highlightedColor = colour;
            colors.pressedColor = colour;
            button.GetComponent<Button>().colors = colors;

            RectTransform rect = button.GetComponent<RectTransform>();
            rect.SetParent(ButtonParent.transform, false);
        }

        private void saveAndCloseColourSelectPanel(Color? colour) {

            setColourCallback(colour);
            gameObject.SetActive(false);
        }
    }
}
