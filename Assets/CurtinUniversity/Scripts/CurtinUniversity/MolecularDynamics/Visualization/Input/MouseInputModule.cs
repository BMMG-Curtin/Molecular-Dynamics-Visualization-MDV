using CurtinUniversity.MolecularDynamics.Visualization;

namespace UnityEngine.EventSystems {

    public class MouseInputModule : PointerInputModule {

        [SerializeField]
        private MouseInputRaycaster graphicRaycaster;

        public override void Process() {

            if (eventSystem.currentSelectedGameObject != null) {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
            }

            PointerEventData eventData;
            GetPointerData(kMouseLeftId, out eventData, true);
            eventData.Reset();
            eventData.scrollDelta = Input.mouseScrollDelta;

            if (graphicRaycaster) {
                graphicRaycaster.Raycast(eventData, m_RaycastResultCache);
                eventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
                m_RaycastResultCache.Clear();
            }

            MouseState mouseState = new MouseState();

            mouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), eventData);

            MouseButtonEventData buttonData = mouseState.GetButtonState(PointerEventData.InputButton.Left).eventData;
            PointerEventData pointerEvent = buttonData.buttonData;
            GameObject currentGO = pointerEvent.pointerCurrentRaycast.gameObject;

            // mouse pointer down
            if (buttonData.PressedThisFrame()) {

                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentGO, pointerEvent);

                GameObject pressedGO = ExecuteEvents.ExecuteHierarchy(currentGO, pointerEvent, ExecuteEvents.pointerDownHandler);

                if (pressedGO == null) {
                    pressedGO = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentGO);
                }

                pointerEvent.clickCount = 1;
                pointerEvent.pointerPress = pressedGO;
                pointerEvent.rawPointerPress = currentGO;
                pointerEvent.clickTime = Time.unscaledTime;
            }

            // mouse pointer up
            if (buttonData.ReleasedThisFrame()) {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentGO);
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick) {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }

                if (currentGO != pointerEvent.pointerEnter) {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentGO);
                }
            }

            // scroll wheel
            if (!Mathf.Approximately(buttonData.buttonData.scrollDelta.sqrMagnitude, 0.0f)) {
                GameObject scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(buttonData.buttonData.pointerCurrentRaycast.gameObject);
                ExecuteEvents.ExecuteHierarchy(scrollHandler, buttonData.buttonData, ExecuteEvents.scrollHandler);
            }
        }
    }
}
