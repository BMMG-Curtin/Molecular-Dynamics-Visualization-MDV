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

        private float moleculeMovementSpeed;
        public float MoleculeInputSensitivity {
            get {
                return moleculeMovementSpeed;
            }
            set {
                moleculeMovementSpeed = Mathf.Clamp(value, 0f, 1f);
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

        public bool SpaceNavigatorCameraControlEnabled { get; set; }
        public bool SpaceNavigatorMoleculeControlEnabled { get; set; }

        public static GeneralSettings Default() {

            return new GeneralSettings {

                ShowGround = false,
                ShowShadows = false,
                MainLightsOn = true,
                FillLightsOn = false,
                AmbientLightsOn = true,
                LightIntensity = (float)(Settings.DefaultLightIntensity - Settings.MinLightIntensity) / (float)(Settings.MaxLightIntensity - Settings.MinLightIntensity),
                AutoMeshQuality = true,
                MeshQuality = Settings.DefaultMeshQuality,
                AutoRotateSpeed = (float)(Settings.DefaultAutoRotateSpeed - Settings.MinAutoRotateSpeed) / (float)(Settings.MaxAutoRotateSpeed - Settings.MinAutoRotateSpeed),
                MoleculeInputSensitivity = (float)(Settings.DefaultMoleculeMovementSpeed - Settings.MinMoleculeMovementSpeed) / (float)(Settings.MaxMoleculeMovementSpeed - Settings.MinMoleculeMovementSpeed),
                SpaceNavigatorCameraControlEnabled = true,
                SpaceNavigatorMoleculeControlEnabled = true,
            };
        }

        public override string ToString() {

            return
                "ShowGround: " + ShowGround + "\n" +
                "ShowShadows: " + ShowShadows + "\n" +
                "MainLightsOn: " + MainLightsOn + "\n" +
                "FillLightsOn: " + FillLightsOn + "\n" +
                "AmbientLightsOn: " + AmbientLightsOn + "\n" +
                "LightIntensity: " + LightIntensity + "\n" +
                "AutoMeshQuality: " + AutoMeshQuality + "\n" +
                "MeshQuality: " + MeshQuality + "\n" +
                "SpaceNavigatorCameraControlEnabled: " + SpaceNavigatorCameraControlEnabled + "\n" +
                "SpaceNavigatorMoleculeControlEnabled: " + SpaceNavigatorMoleculeControlEnabled;
        }
    }
}
