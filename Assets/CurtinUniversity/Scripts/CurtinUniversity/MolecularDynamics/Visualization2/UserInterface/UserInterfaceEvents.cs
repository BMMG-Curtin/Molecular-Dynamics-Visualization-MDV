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

        public delegate void MoleculeRenderSettingsUpdated(int moleculeID, MoleculeRenderSettings settings, int? frameNumber = null);
        public static event MoleculeRenderSettingsUpdated OnMoleculeRenderSettingsUpdated;
        public static void RaiseMoleculeRenderSettingsUpdated(int moleculeID, MoleculeRenderSettings settings, int? frameNumber = null) {
            if (OnMoleculeRenderSettingsUpdated != null) {
                OnMoleculeRenderSettingsUpdated(moleculeID, settings, frameNumber);
            }
        }

        public delegate void ShowMolecule(int moleculeID);
        public static event ShowMolecule OnShowMolecule;
        public static void RaiseShowMolecule(int moleculeID) {
            if (OnShowMolecule != null) {
                OnShowMolecule(moleculeID);
            }
        }

        public delegate void HideMolecule(int moleculeID);
        public static event HideMolecule OnHideMolecule;
        public static void RaiseHideMolecule(int moleculeID) {
            if (OnHideMolecule != null) {
                OnHideMolecule(moleculeID);
            }
        }
    }
}
