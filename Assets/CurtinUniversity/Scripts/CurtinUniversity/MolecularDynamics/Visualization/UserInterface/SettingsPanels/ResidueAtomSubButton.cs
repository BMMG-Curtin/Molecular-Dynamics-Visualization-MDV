using UnityEngine;
using UnityEngine.EventSystems;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class ResidueAtomSubButton : MonoBehaviour, IPointerDownHandler {

        private AtomSubButtonClickDelegate onClick;

        public void Initialise(AtomSubButtonClickDelegate onClick) {
            this.onClick = onClick;
        }

        public void OnPointerDown(PointerEventData eventData) {

            if (onClick != null) {
                onClick();
            }
        }
    }
}
