using System;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public struct GeneralSettings {

        public bool ShowGround { get; set; }
        public bool ShowShadows { get; set; }
        public bool LightsOn { get; set; }

        public bool AutoMeshQuality { get; set; }

        [SerializeField]
        private int meshQuality;
        public int MeshQuality {

            get {
                return meshQuality;
            }

            set {
                meshQuality = Mathf.Clamp(value, 0, Settings.MeshQualityValues.Length - 1);
            }
        }

        public static GeneralSettings Default() {

            return new GeneralSettings {

                AutoMeshQuality = true,
                MeshQuality = Settings.DefaultMeshQuality,
                ShowGround = true,
                ShowShadows = true,
                LightsOn = true,
            };
        }
    }
}
