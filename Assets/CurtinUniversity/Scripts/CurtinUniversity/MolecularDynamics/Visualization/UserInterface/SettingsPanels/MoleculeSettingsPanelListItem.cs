using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void OnMoleculeSettingsPanelListItemClick(int moleculeID);
    public delegate void OnMoleculeSettingsPanelListItemDoubleClick(int moleculeID);

    // we need to use a pointerdownhandler instead of an event trigger on the gameobject as the event trigger consumes the mouse wheel scroll behaviour
    public class MoleculeSettingsPanelListItem : MonoBehaviour, IPointerDownHandler {

        [SerializeField]
        private TextMeshProUGUI moleculeIDText;

        [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField]
        private Image listItemBackground;

        [SerializeField]
        private Color32 normalColor;

        [SerializeField]
        private Color32 highlightedColor;

        private int moleculeID;
        private int displayID;

        private OnMoleculeSettingsPanelListItemClick onClick;
        private OnMoleculeSettingsPanelListItemDoubleClick onDoubleClick;

        private float lastClickTime = 0;
        private float doubleClickTimeout = 0.5f;

        public void Initialise(int moleculeID, string name, OnMoleculeSettingsPanelListItemClick onClick, OnMoleculeSettingsPanelListItemDoubleClick onDoubleClick) {

            this.moleculeID = moleculeID;
            this.onClick = onClick;
            this.onDoubleClick = onDoubleClick;

            moleculeIDText.text = "";
            nameText.text = name;

            SetHighlighted(false);
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

        public void OnPointerDown(PointerEventData eventData) {

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

        public void SetHighlighted(bool setHighlighted) {

            if(setHighlighted) {
                listItemBackground.color = highlightedColor;
            }
            else {
                listItemBackground.color = normalColor;
            }
        }
    }
}
