using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Linq;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public struct MouseRaycastHit {
        public Graphic uiGraphic;
        public Vector3 hitPosition;
    };

    /// <summary>
    /// This class extends graphic raycaster to raycast from a world space mouse pointer
    /// </summary>
    public class MouseInputRaycaster : GraphicRaycaster {

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private GameObject mousePointer;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> results) {

            Ray ray = new Ray(eventCamera.transform.position, (mousePointer.transform.position - eventCamera.transform.position));
            IList<Graphic> canvasGraphics = (IList<Graphic>)GraphicRegistry.GetGraphicsForCanvas(canvas);
            SortedList<int, MouseRaycastHit> hitGraphics = new SortedList<int, MouseRaycastHit>();

            // iterate through all canvas graphics
            for (int i = 0; i < canvasGraphics.Count; i++) {

                // exclude mouse pointer
                if (mousePointer == canvasGraphics[i].gameObject) {
                    continue;
                }

                // check if pointer is over graphic
                Vector3[] worldCorners = new Vector3[4];
                canvasGraphics[i].rectTransform.GetWorldCorners(worldCorners);
                float rayDistance;
                if (!new Plane(worldCorners[0], worldCorners[1], worldCorners[2]).Raycast(ray, out rayDistance)) {
                    continue;
                }

                // pointer is over graphic so get the intersect point and add to sort list
                Vector3 rectIntersect = ray.GetPoint(rayDistance);
                Vector3 rectBottom = worldCorners[3] - worldCorners[0];
                Vector3 rectLeft = worldCorners[1] - worldCorners[0];

                if (Vector3.Dot(rectIntersect - worldCorners[0], rectBottom) < rectBottom.sqrMagnitude && 
                    Vector3.Dot(rectIntersect - worldCorners[0], rectLeft) < rectLeft.sqrMagnitude && 
                    Vector3.Dot(rectIntersect - worldCorners[0], rectBottom) >= 0 && 
                    Vector3.Dot(rectIntersect - worldCorners[0], rectLeft) >= 0) {

                    Vector3 intersectPosition = worldCorners[0] + Vector3.Dot(rectIntersect - worldCorners[0], rectLeft) * rectLeft / rectLeft.sqrMagnitude + Vector3.Dot(rectIntersect - worldCorners[0], rectBottom) * rectBottom / rectBottom.sqrMagnitude;

                    if (canvasGraphics[i].Raycast(eventCamera.WorldToScreenPoint(intersectPosition), eventCamera)) {

                        hitGraphics.Add(
                            canvasGraphics[i].depth,
                            new MouseRaycastHit {
                                uiGraphic = canvasGraphics[i],
                                hitPosition = intersectPosition
                            }
                        );
                    }
                }
            }

            // output raycast hits sorted by depth
            foreach (KeyValuePair<int, MouseRaycastHit> hitGraphic in hitGraphics.Reverse()) {

                results.Add(
                    new RaycastResult {
                        gameObject = hitGraphic.Value.uiGraphic.gameObject,
                        module = this,
                        distance = Vector3.Distance(ray.origin, hitGraphic.Value.hitPosition),
                        index = results.Count,
                        depth = hitGraphic.Value.uiGraphic.depth,
                        worldPosition = hitGraphic.Value.hitPosition
                    }
                );
            }
        }
    }
}
