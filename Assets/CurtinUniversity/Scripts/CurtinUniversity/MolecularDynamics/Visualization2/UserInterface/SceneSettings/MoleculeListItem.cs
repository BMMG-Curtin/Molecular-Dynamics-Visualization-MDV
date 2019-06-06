
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public delegate void OnMoleculeListItemClick(int moleculeID);
    public delegate void OnMoleculeListItemDoubleClick(int moleculeID);

    public class MoleculeListItem : MonoBehaviour {

        [SerializeField]
        private TextMeshProUGUI moleculeIDText;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Button listItemButton;

        [SerializeField]
        private Color32 normalColor;

        [SerializeField]
        private Color32 highlightedColor;

        private int moleculeID;
        private int displayID;
        private string displayName;

        private OnMoleculeListItemClick onClick;
        private OnMoleculeListItemDoubleClick onDoubleClick;

        private float lastClickTime = 0;
        private float doubleClickTimeout = 0.5f;

        private ColorBlock normalColorBlock;
        private ColorBlock highlightedColorBlock;

        public void Initialise(int moleculeID, string name, OnMoleculeListItemClick onClick, OnMoleculeListItemDoubleClick onDoubleClick) {

            this.moleculeID = moleculeID;
            this.displayName = name;
            this.onClick = onClick;
            this.onDoubleClick = onDoubleClick;

            moleculeIDText.text = "";
            nameText.text = name;

            normalColorBlock.normalColor = normalColor;
            normalColorBlock.highlightedColor = normalColor;
            normalColorBlock.pressedColor = normalColor;
            normalColorBlock.disabledColor = normalColor;

            highlightedColorBlock.normalColor = highlightedColor;
            highlightedColorBlock.highlightedColor = highlightedColor;
            highlightedColorBlock.pressedColor = highlightedColor;
            highlightedColorBlock.disabledColor = highlightedColor;
        }

        public int DisplayID {

            get {
                return displayID;
            }

            set {
                displayID = value;
                moleculeIDText.text = displayID.ToString();
            }
        }

        public void OnClick() {

            if (onDoubleClick != null) {

                if (Time.time <= lastClickTime + doubleClickTimeout) {

                    onDoubleClick(moleculeID);
                    lastClickTime = Time.time;
                    return;
                }

                lastClickTime = Time.time;
            }


            if (onClick != null) {
                onClick(moleculeID);
            }
        }

        public void SetHighlighted(bool highlighted) {

            if(highlighted) {
                listItemButton.colors = highlightedColorBlock;
            }
            else {
                listItemButton.colors = normalColorBlock;
            }
        }
    }
}
