using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class UserInterfaceEvents {

        public delegate void LoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings);
        public static event LoadMolecule OnLoadMolecule;
        public static void RaiseOnLoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings) {
            if(OnLoadMolecule != null) {
                OnLoadMolecule(moleculeID, filename, renderSettings);
            }
        }

        public delegate void LoadTrajectory(int moleculeID, string filename);
        public static event LoadTrajectory OnLoadTrajectory;
        public static void RaiseOnLoadTrajectory(int moleculeID, string filename) {
            if (OnLoadTrajectory != null) {
                OnLoadTrajectory(moleculeID, filename);
            }
        }

        public delegate void RemoveMolecule(int moleculeID);
        public static event RemoveMolecule OnRemoveMolecule;
        public static void RaiseOnRemoveMolecule(int moleculeID) {
            if (OnRemoveMolecule != null) {
                OnRemoveMolecule(moleculeID);
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
