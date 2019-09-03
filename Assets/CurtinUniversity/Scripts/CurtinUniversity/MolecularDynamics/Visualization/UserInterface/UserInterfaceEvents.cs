
namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class UserInterfaceEvents {

        public delegate void LoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings);
        public static event LoadMolecule OnLoadMolecule;
        public static void RaiseLoadMolecule(int moleculeID, string filename, MoleculeRenderSettings renderSettings) {
            if (OnLoadMolecule != null) {
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

        public delegate void GeneralSettingsUpdated(GeneralSettings settings);
        public static event GeneralSettingsUpdated OnGeneralSettingsUpdated;
        public static void RaiseGeneralSettingsUpdated(GeneralSettings settings) {
            if (OnGeneralSettingsUpdated != null) {
                OnGeneralSettingsUpdated(settings);
            }
        }

        public delegate void MoleculeRenderSettingsUpdated(int moleculeID, MoleculeRenderSettings settings, int? frameNumber);
        public static event MoleculeRenderSettingsUpdated OnMoleculeRenderSettingsUpdated;
        public static void RaiseMoleculeRenderSettingsUpdated(int moleculeID, MoleculeRenderSettings settings, int? frameNumber) {
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

        public delegate void EnableMoveMolecule(int moleculeID);
        public static event EnableMoveMolecule OnEnableMoveMolecule;
        public static void RaiseDisableMoveMolecule(int moleculeID) {
            if (OnEnableMoveMolecule != null) {
                OnEnableMoveMolecule(moleculeID);
            }
        }

        public delegate void DisableMoveMolecule(int moleculeID);
        public static event DisableMoveMolecule OnDisableMoveMolecule;
        public static void RaiseEnableMoveMolecule(int moleculeID) {
            if (OnDisableMoveMolecule != null) {
                OnDisableMoveMolecule(moleculeID);
            }
        }

        public delegate void SaveMoleculeSettings(MoleculeSettings settings, string saveFilePath);
        public static event SaveMoleculeSettings OnSaveMoleculeSettings;
        public static void RaiseSaveMoleculeSettings(MoleculeSettings settings, string saveFilePath) {
            if (OnSaveMoleculeSettings != null) {
                OnSaveMoleculeSettings(settings, saveFilePath);
            }
        }

        public delegate void LoadMoleculeSettings(int moleculeID, string filePath, bool loadStructure, bool loadTrajectory, bool loadRenderSettings, bool loadMoleculeTransform, bool loadCameraTransform, LoadMoleculeRenderSettingsDelegate loadMoleculeRenderSettingsCallback);
        public static event LoadMoleculeSettings OnLoadMoleculeSettings;
        public static void RaiseLoadMoleculeSettings(int moleculeID, string filePath, bool loadStructure, bool loadTrajectory, bool loadRenderSettings, bool loadMoleculeTransform, bool loadCameraTransform, LoadMoleculeRenderSettingsDelegate loadMoleculeRenderSettingsCallback) {
            if (OnLoadMoleculeSettings != null) {
                OnLoadMoleculeSettings(moleculeID, filePath, loadStructure, loadTrajectory, loadRenderSettings, loadMoleculeTransform, loadCameraTransform, loadMoleculeRenderSettingsCallback);
            }
        }
    }
}
