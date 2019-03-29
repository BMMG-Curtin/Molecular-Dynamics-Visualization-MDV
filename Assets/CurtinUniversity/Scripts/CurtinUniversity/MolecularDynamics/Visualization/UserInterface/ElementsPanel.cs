using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Model;
using CurtinUniversity.MolecularDynamics.Visualization.Utility;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ElementsPanel : MonoBehaviour {

        public GameObject ElementPanel;
        public GameObject ButtonEnabledPrefab; // for elements in model. Interactive
        public GameObject ButtonDisabledPrefab; // for elements not in model. Non interactive

        [HideInInspector]
        public HashSet<string> EnabledElements { get { return enabledElements; } }

        private SceneManager sceneManager;

        private HashSet<string> modelElements;
        private HashSet<string> enabledElements;

        void Start() {

            sceneManager = SceneManager.instance;

            if (modelElements == null) {
                modelElements = new HashSet<string>();
                Initialise();
            }
        }

        public void SetModelElements(HashSet<string> elements) {
            modelElements = elements;
            Initialise();
        }

        public bool HasHiddenElements {
            get {
                if (modelElements.Count > enabledElements.Count) {
                    return true;
                }
                return false;
            }
        }

        public void Initialise() {

            Cleanup.DestroyGameObjects(ElementPanel);

            if (modelElements != null) {
                enabledElements = new HashSet<string>(modelElements);
            }
            else {
                enabledElements = new HashSet<string>();
            }

            for (int i = 0; i < MolecularConstants.ElementNamesByAtomicNumber.Count; i++) {

                GameObject button;
                string symbol = MolecularConstants.ElementNamesByAtomicNumber[i][0];

                if (modelElements.Contains(symbol.ToUpper())) {

                    button = (GameObject)Instantiate(ButtonEnabledPrefab, Vector3.zero, Quaternion.identity);
                    button.GetComponent<Image>().color = new Color(1, 1, 1);

                    ElementButton buttonScript = button.GetComponent<ElementButton>();
                    buttonScript.SetCallback(new SetElementDelegate(SetElement));
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

        public void SetElement(string elementName, bool enabled) {

            if (enabledElements.Contains(elementName)) {
                if (!enabled) {
                    enabledElements.Remove(elementName);
                }
            }
            else {
                if (enabled) {
                    enabledElements.Add(elementName);
                }
            }

            StartCoroutine(sceneManager.ReloadModelView(true, false));
        }
    }
}
