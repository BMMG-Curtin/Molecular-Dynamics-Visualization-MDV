using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization.Utility {

    public static class Cleanup {

        public static void DestroyGameObjects(GameObject parent) {

            if (parent != null) {
                foreach (Transform child in parent.transform) {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        public static void ForeceGC() {

            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}

