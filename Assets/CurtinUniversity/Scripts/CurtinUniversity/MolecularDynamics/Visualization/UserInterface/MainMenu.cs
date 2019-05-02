using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MainMenu : MonoBehaviour {

        public GameObject SettingsPanel;
        public GameObject MoleculesPanel;
        public GameObject VisualisationPanel;
        public GameObject PhysicsPanel;

        public GameObject SettingsPanelButton;
        public GameObject MoleculesPanelButton;
        public GameObject VisualisationPanelButton;
        public GameObject PhysicsPanelButton;

        private Color enabledColor = new Color(96f / 255f, 39f / 255f, 13f / 255f, 100f);
        private Color disabledColor = new Color(30f / 255f, 15f / 255f, 15f / 255f, 100f);

        void Start() {
            ShowSettingsPanel();
        }

        public void ShowSettingsPanel() {

            disablePanels();
            SettingsPanelButton.GetComponent<Image>().color = enabledColor;
            SettingsPanel.SetActive(true);
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

        public void ShowPhysicsPanel() {

            disablePanels();
            PhysicsPanelButton.GetComponent<Image>().color = enabledColor;
            PhysicsPanel.SetActive(true);
        }

        public void ResetAndHideMenu() {

            disablePanels();
            ShowSettingsPanel();
            gameObject.SetActive(false);
        }

        private void disablePanels() {

            SettingsPanelButton.GetComponent<Image>().color = disabledColor;
            MoleculesPanelButton.GetComponent<Image>().color = disabledColor;
            VisualisationPanelButton.GetComponent<Image>().color = disabledColor;
            PhysicsPanelButton.GetComponent<Image>().color = disabledColor;

            SettingsPanel.SetActive(false);
            MoleculesPanel.SetActive(false);
            VisualisationPanel.SetActive(false);
            PhysicsPanel.SetActive(false);
        }
    }
}

