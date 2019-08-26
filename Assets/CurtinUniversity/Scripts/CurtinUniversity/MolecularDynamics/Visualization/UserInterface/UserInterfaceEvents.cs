using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class UserInterfaceEvents {

        public delegate void LoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings);
        public static event LoadMolecule OnLoadMolecule;
        public static void RaiseLoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings) {
            OnLoadMolecule?.Invoke(moleculeID, filename, renderSettings);
        }

        public delegate void LoadTrajectory(int moleculeID, string filename);
        public static event LoadTrajectory OnLoadTrajectory;
        public static void RaiseLoadTrajectory(int moleculeID, string filename) {
            OnLoadTrajectory?.Invoke(moleculeID, filename);
        }

        public delegate void RemoveMolecule(int moleculeID);
        public static event RemoveMolecule OnRemoveMolecule;
        public static void RaiseRemoveMolecule(int moleculeID) {
            OnRemoveMolecule?.Invoke(moleculeID);
        }

        public delegate void GeneralSettingsUpdated(GeneralSettings settings);
        public static event GeneralSettingsUpdated OnGeneralSettingsUpdated;
        public static void RaiseGeneralSettingsUpdated(GeneralSettings settings) {
            OnGeneralSettingsUpdated?.Invoke(settings);
        }

        public delegate void MoleculeRenderSettingsUpdated(int moleculeID, MoleculeRenderSettings settings, int? frameNumber);
        public static event MoleculeRenderSettingsUpdated OnMoleculeRenderSettingsUpdated;
        public static void RaiseMoleculeRenderSettingsUpdated(int moleculeID, MoleculeRenderSettings settings, int? frameNumber) {
            OnMoleculeRenderSettingsUpdated?.Invoke(moleculeID, settings, frameNumber);
        }

        public delegate void ShowMolecule(int moleculeID);
        public static event ShowMolecule OnShowMolecule;
        public static void RaiseShowMolecule(int moleculeID) {
            OnShowMolecule?.Invoke(moleculeID);
        }

        public delegate void HideMolecule(int moleculeID);
        public static event HideMolecule OnHideMolecule;
        public static void RaiseHideMolecule(int moleculeID) {
            OnHideMolecule?.Invoke(moleculeID);
        }

        public delegate void EnableMoveMolecule(int moleculeID);
        public static event EnableMoveMolecule OnEnableMoveMolecule;
        public static void RaiseDisableMoveMolecule(int moleculeID) {
            OnEnableMoveMolecule?.Invoke(moleculeID);
        }

        public delegate void DisableMoveMolecule(int moleculeID);
        public static event DisableMoveMolecule OnDisableMoveMolecule;
        public static void RaiseEnableMoveMolecule(int moleculeID) {
            OnDisableMoveMolecule?.Invoke(moleculeID);
        }
    }
}
