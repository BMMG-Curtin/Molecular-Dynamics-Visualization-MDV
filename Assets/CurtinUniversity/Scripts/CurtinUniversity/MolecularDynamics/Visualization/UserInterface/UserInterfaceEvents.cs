
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

        public delegate void ResetMoleculeTransform(int moleculeID);
        public static event ResetMoleculeTransform OnResetMoleculeTransform;
        public static void RaiseResetMoleculeTransform(int moleculeID) {
            if (OnResetMoleculeTransform != null) {
                OnResetMoleculeTransform(moleculeID);
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

        public delegate void MoleculeSelected(int moleculeID, bool selected);
        public static event MoleculeSelected OnMoleculeSelected;
        public static void RaiseMoleculeSelected(int moleculeID, bool selected) {
            if (OnMoleculeSelected != null) {
                OnMoleculeSelected(moleculeID, selected);
            }
        }

        public delegate void MoveCameraToMolecule(int moleculeID);
        public static event MoveCameraToMolecule OnMoveCameraToMolecule;
        public static void RaiseMoveCameraToMolecule(int moleculeID) {
            if (OnMoveCameraToMolecule != null) {
                OnMoveCameraToMolecule(moleculeID);
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

        public delegate void StartMonitoringMoleculeInteractions(int molecule1ID, int molecule2ID, MolecularInteractionSettings interactionSettings, MoleculeRenderSettings molecule1Settings, MoleculeRenderSettings molecule2Settings);
        public static event StartMonitoringMoleculeInteractions OnStartMonitoringMoleculeInteractions;
        public static void RaiseStartMonitoringMoleculeInteractions(int molecule1ID, int molecule2ID, MolecularInteractionSettings interactionSettings, MoleculeRenderSettings molecule1Settings, MoleculeRenderSettings molecule2Settings) {
            if(OnStartMonitoringMoleculeInteractions != null) {
                OnStartMonitoringMoleculeInteractions(molecule1ID, molecule2ID, interactionSettings, molecule1Settings, molecule2Settings);
            }
        }

        public delegate void StopMonitoringMoleculeInteractions();
        public static event StopMonitoringMoleculeInteractions OnStopMonitoringMoleculeInteractions;
        public static void RaiseStopMonitoringMoleculeInteractions() {
            if (OnStopMonitoringMoleculeInteractions != null) {
                OnStopMonitoringMoleculeInteractions();
            }
        }

        public delegate void UpdateMolecularInteractionSettings(MolecularInteractionSettings settings);
        public static event UpdateMolecularInteractionSettings OnUpdateMolecularInteractionSettings;
        public static void RaiseMolecularInteractionSettingsUpdated(MolecularInteractionSettings settings) {
            if (OnUpdateMolecularInteractionSettings != null) {
                OnUpdateMolecularInteractionSettings(settings);
            }
        }
    }
}
