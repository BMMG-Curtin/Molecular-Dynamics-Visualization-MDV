using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class SettingsPanelsMenu : MonoBehaviour {

        [SerializeField]
        private GameObject MoleculesPanel;

        [SerializeField]
        private GameObject VisualisationPanel;

        [SerializeField]
        private GameObject ElementsPanel;

        [SerializeField]
        private GameObject ResiduesPanel;

        [SerializeField]
        private GameObject InteractionsPanel;

        [SerializeField]
        private GameObject OtherPanel;

        [SerializeField]
        private GameObject MoleculesPanelButton;

        [SerializeField]
        private GameObject VisualisationPanelButton;

        [SerializeField]
        private GameObject ElementsPanelButton;

        [SerializeField]
        private GameObject ResiduesPanelButton;

        [SerializeField]
        private GameObject InteractionsPanelButton;

        [SerializeField]
        private GameObject OtherPanelButton;

        private Color enabledColor = new Color(96f / 255f, 39f / 255f, 13f / 255f, 100f);
        private Color disabledColor = new Color(30f / 255f, 15f / 255f, 15f / 255f, 100f);

        void Start() {
            ShowMoleculesPanel();
        }

        public void ShowMoleculesPanel() {

            disablePanels();
            MoleculesPanelButton.GetComponent<Image>().color = enabledColor;
            MoleculesPanel.SetActive(true);
        }

        public void ShowVisualisationPanel() {

            disablePanels();
            VisualisationPanelButton.GetComponent<Image>().color = enabledColor;
            VisualisationPanel.SetActive(true);
        }

        public void ShowElementsPanel() {

            disablePanels();
            ElementsPanelButton.GetComponent<Image>().color = enabledColor;
            ElementsPanel.SetActive(true);
        }

        public void ShowResiduesPanel() {

            disablePanels();
            ResiduesPanelButton.GetComponent<Image>().color = enabledColor;
            ResiduesPanel.SetActive(true);
        }

        public void ShowInteractionsPanel() {

            disablePanels();
            InteractionsPanelButton.GetComponent<Image>().color = enabledColor;
            InteractionsPanel.SetActive(true);
        }

        public void ShowOtherPanel() {

            disablePanels();
            OtherPanelButton.GetComponent<Image>().color = enabledColor;
            OtherPanel.SetActive(true);
        }

        public void ResetAndHideMenu() {

            disablePanels();
            ShowOtherPanel();
            gameObject.SetActive(false);
        }

        private void disablePanels() {

            MoleculesPanelButton.GetComponent<Image>().color = disabledColor;
            VisualisationPanelButton.GetComponent<Image>().color = disabledColor;
            ElementsPanelButton.GetComponent<Image>().color = disabledColor;
            ResiduesPanelButton.GetComponent<Image>().color = disabledColor;
            InteractionsPanelButton.GetComponent<Image>().color = disabledColor;
            OtherPanelButton.GetComponent<Image>().color = disabledColor;

            MoleculesPanel.SetActive(false);
            VisualisationPanel.SetActive(false);
            ElementsPanel.SetActive(false);
            ResiduesPanel.SetActive(false);
            InteractionsPanel.SetActive(false);
            OtherPanel.SetActive(false);
        }
    }
}

