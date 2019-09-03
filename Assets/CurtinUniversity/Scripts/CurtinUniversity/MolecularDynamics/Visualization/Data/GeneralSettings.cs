using System;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public struct GeneralSettings {

        public bool ShowGround { get; set; }
        public bool ShowShadows { get; set; }
        public bool MainLightsOn { get; set; }
        public bool FillLightsOn { get; set; }
        public bool AmbientLightsOn { get; set; }

        private float intensity;
        public float LightIntensity {
            get {
                return intensity;
            }
            set {
                intensity = Mathf.Clamp(value, 0f, 1f);
            }
        }

        private float autoRotateSpeed;
        public float AutoRotateSpeed {
            get {
                return autoRotateSpeed;
            }
            set {
                autoRotateSpeed = Mathf.Clamp(value, 0f, 1f);
            }
        }

        public bool AutoMeshQuality { get; set; }

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

                ShowGround = true,
                ShowShadows = true,
                MainLightsOn = true,
                FillLightsOn = false,
                AmbientLightsOn = true,
                LightIntensity = Settings.DefaultLightIntensity,
                AutoMeshQuality = true,
                MeshQuality = Settings.DefaultMeshQuality,
            };
        }
    }
}
