using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Visualization.Utility;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void SetElementDelegate(string elementName, bool enabled);

    public class ElementsSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        public GameObject ElementPanel;
        public GameObject ButtonEnabledPrefab; // for elements in model. Interactive
        public GameObject ButtonDisabledPrefab; // for elements not in model. Non interactive

        private Dictionary<int, HashSet<string>> modelElements;

        private MoleculeSettings selectedMolecule;

        private void Awake() {
            if (modelElements == null) {
                modelElements = new Dictionary<int, HashSet<string>>();
            }
        }

        public void OnEnable() {

            selectedMolecule = molecules.GetSelected();
            initialise();

            if (selectedMolecule != null) {
                selectedMoleculeText.text = "Modifying settings for molecule  - " + selectedMolecule.Name;
            }
            else {
                selectedMoleculeText.text = "< no molecule selected >";
            }
        }

        public void SetModelElements(int moleculeID, HashSet<string> elements) {

            if (modelElements == null) {
                modelElements = new Dictionary<int, HashSet<string>>();
            }

            modelElements.Add(moleculeID, elements);
        }

        private void initialise() {

            Cleanup.DestroyGameObjects(ElementPanel);

            if (selectedMolecule == null) {
                return;
            }

            if (selectedMolecule.RenderSettings.EnabledElements == null) {

                if (modelElements.ContainsKey(selectedMolecule.ID)) {
                    selectedMolecule.RenderSettings.EnabledElements = new HashSet<string>(modelElements[selectedMolecule.ID]);
                }
                else {
                    selectedMolecule.RenderSettings.EnabledElements = new HashSet<string>();
                }
            }

            HashSet<string> displayElements = modelElements[selectedMolecule.ID];

            for (int i = 0; i < MolecularConstants.ElementNamesByAtomicNumber.Count; i++) {

                GameObject button;
                string symbol = MolecularConstants.ElementNamesByAtomicNumber[i][0];

                if (displayElements.Contains(symbol.ToUpper())) {

                    button = (GameObject)Instantiate(ButtonEnabledPrefab, Vector3.zero, Quaternion.identity);
                    button.GetComponent<Image>().color = new Color(1, 1, 1);

                    ElementButton buttonScript = button.GetComponent<ElementButton>();

                    if (selectedMolecule.RenderSettings.EnabledElements.Contains(symbol.ToUpper())) {
                        buttonScript.Initialise(true, new SetElementDelegate(setElement));
                    }
                    else { 
                        buttonScript.Initialise(false, new SetElementDelegate(setElement));
                    }
                    buttonScript.ElementName = symbol;
                }
                else {
                    button = (GameObject)Instantiate(ButtonDisabledPrefab, Vector3.zero, Quaternion.identity);
                }

                RectTransform rect = button.GetComponent<RectTransform>();

                Text text = button.GetComponentInChildren<Text>();
                if (text != null) {
                    if (symbol != null) {
                        text.text = symbol;
                    }
                    else {
                        text.text = "unknown";
                    }
                }

                rect.SetParent(ElementPanel.GetComponent<RectTransform>());

                Vector2 position = MolecularConstants.ElementsChartPosition[i];

                float x = position.x * 50f;
                float y = position.y * -50f;

                rect.localPosition = new Vector3(x, y, 0);
                rect.localRotation = Quaternion.Euler(0, 0, 0);
                rect.localScale = Vector3.one;
            }
        }

        private void setElement(string elementName, bool enabled) {

            if (selectedMolecule == null || selectedMolecule.RenderSettings.EnabledElements == null) {
                return;
            }

            if (selectedMolecule.RenderSettings.EnabledElements.Contains(elementName)) {
                if (!enabled) {
                    selectedMolecule.RenderSettings.EnabledElements.Remove(elementName);
                }
            }
            else {
                if (enabled) {
                    selectedMolecule.RenderSettings.EnabledElements.Add(elementName);
                }
            }

            if (selectedMolecule.Hidden) {
                selectedMolecule.PendingRerender = true;
            }
            else {
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
            }
        }
    }
}
