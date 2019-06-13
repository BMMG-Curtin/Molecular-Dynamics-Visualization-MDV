using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class UserInterfaceEvents {

        public delegate void LoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings);
        public static event LoadMolecule OnLoadMolecule;
        public static void RaiseLoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings) {
            if(OnLoadMolecule != null) {
                OnLoadMolecule(moleculeID, filename, renderSettings);
            }
        }

        public delegate void LoadTrajectory(int moleculeID, string filename);
        public static event LoadTrajectory OnLoadTrajectory;
        public static void RaiseLoadTrajectory(int moleculeID, string filename) {
            if (OnLoadTrajectory != null) {
                OnLoadTrajectory(moleculeID, filename);
            }
        }

        public delegate void RemoveMolecule(int moleculeID);
        public static event RemoveMolecule OnRemoveMolecule;
        public static void RaiseRemoveMolecule(int moleculeID) {
            if (OnRemoveMolecule != null) {
                OnRemoveMolecule(moleculeID);
            }
        }

        public delegate void SceneSettingsUpdated(SceneSettings settings);
        public static event SceneSettingsUpdated OnSceneSettingsUpdated;
        public static void RaiseSceneSettingsUpdated(SceneSettings settings) {
            if (OnSceneSettingsUpdated != null) {
                OnSceneSettingsUpdated(settings);
            }
        }

        public delegate void MoleculeSettingsUpdated(MoleculeSettings settings);
        public static event MoleculeSettingsUpdated OnMoleculeSettingsUpdated;
        public static void RaiseMoleculeSettingsUpdated(MoleculeSettings settings) {
            if (OnMoleculeSettingsUpdated != null) {
                OnMoleculeSettingsUpdated(settings);
            }
        }

        public delegate void ShowMolecule(int moleculeID, bool show);
        public static event ShowMolecule OnShowMolecule;
        public static void RaiseShowMolecule(int moleculeID, bool show) {
            if (OnShowMolecule != null) {
                OnShowMolecule(moleculeID, show);
            }
        }
    }
}
