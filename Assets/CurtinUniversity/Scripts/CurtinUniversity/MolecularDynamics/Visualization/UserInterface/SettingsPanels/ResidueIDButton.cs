//using UnityEngine;
//using UnityEngine.UI;

//namespace CurtinUniversity.MolecularDynamics.Visualization {

//    public class ResidueIDButton : MonoBehaviour {

//        [SerializeField]
//        private Text buttonIDText;

//        public Color32 EnabledColour;
//        public Color32 HighlightedColour;
//        public Color32 DisabledColour;

//        private ColorBlock buttonColours;
//        private ResidueRenderSettings residueOptions;

//        private SaveResidueButtonOptionsDelegate saveOptionsCallback;
//        private OpenResidueDisplayOptionsDelegate openDisplayOptionsCallback;

//        void Start() {
//            buttonColours = GetComponent<Button>().colors;
//        }

//        public void Initialise(ResidueRenderSettings options, SaveResidueButtonOptionsDelegate saveOptionsCallback, OpenResidueDisplayOptionsDelegate openDisplayCallback) {

//            this.saveOptionsCallback = saveOptionsCallback;
//            openDisplayOptionsCallback = openDisplayCallback;

//            UpdateResidueOptions(options);
//        }

//        public void UpdateResidueOptions(ResidueRenderSettings options) {

//            this.residueOptions = options;

//            buttonIDText.text = options.ResidueID.ToString();
//            updateButtonColors();
//        }

//        public void ResidueClick() {

//            if (InputManager.Instance.ShiftPressed) {

//                openDisplayOptionsCallback?.Invoke(ResidueUpdateType.ID, residueOptions.ResidueID);
//            }
//            else {

//                if (saveOptionsCallback != null) {

//                    updateButtonColors();
//                    saveOptionsCallback(ResidueUpdateType.ID, residueOptions, false, true);
//                }
//            }
//        }

//        private void updateButtonColors() {

//            if (residueOptions.Enabled) {
//                if (residueOptions.IsDefault()) {
//                    setButtonColours(EnabledColour);
//                }
//                else {
//                    setButtonColours(HighlightedColour);
//                }
//            }
//            else {
//                setButtonColours(DisabledColour);
//            }
//        }

//        private void setButtonColours(Color32 color) {

//            buttonColours.normalColor = color;
//            buttonColours.highlightedColor = color;
//            buttonColours.pressedColor = color;
//            buttonColours.colorMultiplier = 1f;
//            GetComponent<Button>().colors = buttonColours;
//        }
//    }
//}
