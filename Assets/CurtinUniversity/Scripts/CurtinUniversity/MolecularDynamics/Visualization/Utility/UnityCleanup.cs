using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public static class UnityCleanup {

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

