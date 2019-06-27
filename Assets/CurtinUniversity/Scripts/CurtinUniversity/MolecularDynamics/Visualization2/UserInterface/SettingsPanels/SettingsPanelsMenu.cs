using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class SettingsPanelsMenu : MonoBehaviour {

        public GameObject ApplicationPanel;
        public GameObject MoleculesPanel;
        public GameObject VisualisationPanel;
        public GameObject ElementsPanel;
        public GameObject ResiduesPanel;
        public GameObject PhysicsPanel;

        public GameObject ApplicationPanelButton;
        public GameObject MoleculesPanelButton;
        public GameObject VisualisationPanelButton;
        public GameObject ElementsPanelButton;
        public GameObject ResiduesPanelButton;
        public GameObject PhysicsPanelButton;

        private Color enabledColor = new Color(96f / 255f, 39f / 255f, 13f / 255f, 100f);
        private Color disabledColor = new Color(30f / 255f, 15f / 255f, 15f / 255f, 100f);

        void Start() {
            ShowApplicationPanel();
        }

        public void ShowApplicationPanel() {

            disablePanels();
            ApplicationPanelButton.GetComponent<Image>().color = enabledColor;
            ApplicationPanel.SetActive(true);
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

        public void ShowPhysicsPanel() {

            disablePanels();
            PhysicsPanelButton.GetComponent<Image>().color = enabledColor;
            PhysicsPanel.SetActive(true);
        }

        public void ResetAndHideMenu() {

            disablePanels();
            ShowApplicationPanel();
            gameObject.SetActive(false);
        }

        private void disablePanels() {

            ApplicationPanelButton.GetComponent<Image>().color = disabledColor;
            MoleculesPanelButton.GetComponent<Image>().color = disabledColor;
            VisualisationPanelButton.GetComponent<Image>().color = disabledColor;
            ElementsPanelButton.GetComponent<Image>().color = disabledColor;
            ResiduesPanelButton.GetComponent<Image>().color = disabledColor;
            PhysicsPanelButton.GetComponent<Image>().color = disabledColor;

            ApplicationPanel.SetActive(false);
            MoleculesPanel.SetActive(false);
            VisualisationPanel.SetActive(false);
            ElementsPanel.SetActive(false);
            ResiduesPanel.SetActive(false);
            PhysicsPanel.SetActive(false);
        }
    }
}

