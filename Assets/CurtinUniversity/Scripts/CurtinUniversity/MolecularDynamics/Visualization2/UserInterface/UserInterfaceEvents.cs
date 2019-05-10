using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class UserInterfaceEvents {

        public delegate void LoadMolecule(string filename);
        public static event LoadMolecule OnLoadMolecule;
        public static void RaiseOnLoadMolecule(string filename) {
            if(OnLoadMolecule != null) {
                OnLoadMolecule(filename);
            }
        }

        public delegate void LoadTrajectory(string filename);
        public static event LoadTrajectory OnLoadTrajectory;
        public static void RaiseOnLoadTrajectory(string filename) {
            if (OnLoadTrajectory != null) {
                OnLoadTrajectory(filename);
            }
        }

        public delegate void SceneSettingsUpdated(SceneSettings settings);
        public static event SceneSettingsUpdated OnSceneSettingsUpdated;
        public static void RaiseOnSceneSettingsUpdated(SceneSettings settings) {
            if (OnSceneSettingsUpdated != null) {
                OnSceneSettingsUpdated(settings);
            }
        }

        public delegate void MoleculeRenderSettingsUpdated(MoleculeRenderSettings settings);
        public static event MoleculeRenderSettingsUpdated OnMoleculeRenderSettingsUpdated;
        public static void RaiseOnMoleculeRenderSettingsUpdated(MoleculeRenderSettings settings) {
            if (OnMoleculeRenderSettingsUpdated != null) {
                OnMoleculeRenderSettingsUpdated(settings);
            }
        }
    }
}
