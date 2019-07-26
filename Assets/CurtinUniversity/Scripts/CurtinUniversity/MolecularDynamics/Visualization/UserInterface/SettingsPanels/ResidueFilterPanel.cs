using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using System.Text;
using System.Text.RegularExpressions;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueFilterPanel : MonoBehaviour {

        public Toggle EnableFilter;
        public InputField FilterString;
        public Text FilterStringText;
        public Text ErrorText;

        public ResidueFilterButton filterButton;

        [HideInInspector]
        public HashSet<int> EnabledResiduesNumbers;

        //private SceneManager sceneManager;
        private const int MAX_RESIDUE_NUMBER = 10000;

        void Awake() {
            EnableFilter.isOn = false;
        }

        void Start() {

            //sceneManager = SceneManager.instance;

            SetDefaultOptions();
            EnabledResiduesNumbers = new HashSet<int>();
        }

        public void Initialise() {

            SetDefaultOptions();
            EnabledResiduesNumbers = new HashSet<int>();
        }

        public void SetDefaultOptions() {

            EnableFilter.isOn = false;
            FilterString.text = "";

            filterButton.Highlight(false);
            ErrorText.text = "";
        }

        public void OnEnable() {
            ErrorText.text = "";
        }

        public bool IsDefault() {

            if (EnableFilter.isOn != true && FilterString.text.Trim() == "") {
                return true;
            }

            return false;
        }

        public void SaveFilterEnabled() {

            FilterString.interactable = EnableFilter.isOn;

            if (EnableFilter.isOn) {
                FilterStringText.color = Color.white;
            }
            else {
                FilterStringText.color = new Color(0.2f, 0.2f, 0.2f);
            }

            filterButton.Highlight(EnableFilter.isOn);
        }

        public void FilterEnabledClick() {
            //StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public void OnEnterFilterString() {
            //sceneManager.InputManager.KeyboardUIControlEnabled = false;
            //sceneManager.InputManager.KeyboardSceneControlEnabled = false;
        }

        public void SaveFilterString() {

            //sceneManager.InputManager.KeyboardUIControlEnabled = true;
            //sceneManager.InputManager.KeyboardSceneControlEnabled = true;

            string inputText = CleanFilterString(FilterString.text);
            string[] items = inputText.Split(',');

            if (items == null || items.Length == 0) {

                FilterString.text = "";
                ErrorText.text = "No valid numbers or ranges found in input";
                return;
            }

            List<string> filterStringItems = new List<string>();
            EnabledResiduesNumbers = new HashSet<int>();
            ErrorText.text = "";

            foreach (string item in items) {

                if (item == null || item.Trim() == "") {
                    continue;
                }

                if (item.Contains("-")) {

                    string[] numbers = item.Split('-');

                    if (numbers != null && numbers.Length >= 2) {

                        int number1 = -1;
                        int number2 = -1;

                        if (int.TryParse(numbers[0], out number1) && int.TryParse(numbers[1], out number2)) {

                            if (number1 >= 0 && number1 <= MAX_RESIDUE_NUMBER && number2 >= 0 && number2 <= MAX_RESIDUE_NUMBER) {

                                if (number1 <= number2) {
                                    for (int i = number1; i <= number2; i++) {
                                        EnabledResiduesNumbers.Add(i);
                                    }
                                }
                                else {
                                    for (int i = number2; i <= number1; i++) {
                                        EnabledResiduesNumbers.Add(i);
                                    }
                                }

                                filterStringItems.Add(item);
                            }
                        }
                    }
                }
                else {

                    int residueNumber = -1;

                    if (int.TryParse(item, out residueNumber)) {

                        if (residueNumber >= 0 && residueNumber <= MAX_RESIDUE_NUMBER) {

                            filterStringItems.Add(residueNumber.ToString());
                            EnabledResiduesNumbers.Add(residueNumber);
                        }
                    }
                }
            }

            if (filterStringItems.Count > 0) {
                FilterString.text = string.Join(",", filterStringItems.ToArray());
            }
            else {
                FilterString.text = "";
                ErrorText.text = "No valid numbers or ranges found in input";
            }

            //List<string> debugInts = new List<string>();
            //foreach(int residueNumber in enabledResidues) {
            //    debugInts.Add(residueNumber.ToString());
            //}
            //Debug.Log("Filter string parsed, resulting residues: \n" + string.Join(",", debugInts.ToArray()));



            //StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public string CleanFilterString(string inputText) {

            StringBuilder cleanText = new StringBuilder();

            foreach (char character in inputText) {
                if (character != ' ' && character == ',' || character == '-' || char.IsDigit(character)) {
                    cleanText.Append(character);
                }
            }

            return cleanText.ToString();
        }
    }
}
