
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class VisualisationSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        private MoleculeSettings selectedMolecule;

        public Toggle EnablePrimaryStructureToggle;
        public Toggle ShowAtomsToggle;
        public Toggle ShowBondsToggle;
        public Toggle ShowStandardResiduesToggle;
        public Toggle ShowNonStandardResiduesToggle;
        public Toggle ShowMainChainsToggle;

        public Toggle EnableSecondaryStructureToggle;
        public Toggle ShowHelicesToggle;
        public Toggle ShowSheetsToggle;
        public Toggle ShowTurnsToggle;

        public Toggle ShowCPKToggle;
        public Toggle ShowVDWToggle;

        public Toggle EnableSimlationBoxToggle;
        public Toggle CalculateBoxEveryFrameToggle;

        public Text AtomScale;
        public Text BondScale;
        private float scaleIncrementAmount = 0.1f;

        public bool SettingsChanged { get; set; }

        private void Awake() {
            SettingsChanged = false;
        }

        public void OnEnable() {

            selectedMolecule = molecules.GetSelected();

            if (selectedMolecule != null) {

                selectedMoleculeText.text = "Modifying settings for molecule  - " + selectedMolecule.Name;
                EnableSettings(true);
                LoadSettings();
            }
            else {

                selectedMoleculeText.text = "< no molecule selected >";
                EnableSettings(false);
                ClearSettings();
            }
        }

        public void ClearSettings() {

            // primary structure options
            EnablePrimaryStructureToggle.isOn = false;
            ShowAtomsToggle.isOn = false;
            ShowBondsToggle.isOn = false;
            ShowStandardResiduesToggle.isOn = false;
            ShowNonStandardResiduesToggle.isOn = false;
            ShowMainChainsToggle.isOn = false;

            // representation options
            ShowCPKToggle.isOn = false;
            ShowVDWToggle.isOn = false;

            // secondary structure options
            EnableSecondaryStructureToggle.isOn = false;
            ShowHelicesToggle.isOn = false;
            ShowSheetsToggle.isOn = false;
            ShowTurnsToggle.isOn = false;

            // scale options
            AtomScale.text = "";
            BondScale.text = "";

            // other options
            EnableSimlationBoxToggle.isOn = false;
            CalculateBoxEveryFrameToggle.isOn = false;

        }

        public void EnableSettings(bool enable) {

            EnablePrimaryStructureToggle.interactable = enable;
            ShowAtomsToggle.interactable = enable;
            ShowBondsToggle.interactable = enable;
            ShowStandardResiduesToggle.interactable = enable;
            ShowNonStandardResiduesToggle.interactable = enable;
            ShowMainChainsToggle.interactable = enable;

            ShowCPKToggle.interactable = enable;
            ShowVDWToggle.interactable = enable;

            EnableSecondaryStructureToggle.interactable = enable;
            ShowHelicesToggle.interactable = enable;
            ShowSheetsToggle.interactable = enable;
            ShowTurnsToggle.interactable = enable;

            EnableSimlationBoxToggle.interactable = enable;
            CalculateBoxEveryFrameToggle.interactable = enable;
        }

        public void LoadSettings() {

            // primary structure options
            EnablePrimaryStructureToggle.isOn = selectedMolecule.RenderSettings.ShowPrimaryStructure;
            ShowAtomsToggle.isOn = selectedMolecule.RenderSettings.ShowAtoms;
            ShowBondsToggle.isOn = selectedMolecule.RenderSettings.ShowBonds;
            ShowStandardResiduesToggle.isOn = selectedMolecule.RenderSettings.ShowStandardResidues;
            ShowNonStandardResiduesToggle.isOn = selectedMolecule.RenderSettings.ShowNonStandardResidues;
            ShowMainChainsToggle.isOn = selectedMolecule.RenderSettings.ShowMainChains;

            // representation options
            ShowCPKToggle.isOn = false;
            ShowVDWToggle.isOn = false;
            switch (selectedMolecule.RenderSettings.Representation) {
                case MolecularRepresentation.CPK:
                    ShowCPKToggle.isOn = true;
                    break;
                case MolecularRepresentation.VDW:
                    ShowVDWToggle.isOn = true;
                    break;
            }

            // secondary structure options
            EnableSecondaryStructureToggle.isOn = selectedMolecule.RenderSettings.ShowSecondaryStructure;
            ShowHelicesToggle.isOn = selectedMolecule.RenderSettings.ShowHelices;
            ShowSheetsToggle.isOn = selectedMolecule.RenderSettings.ShowSheets;
            ShowTurnsToggle.isOn = selectedMolecule.RenderSettings.ShowTurns;

            // scale options
            AtomScale.text = selectedMolecule.RenderSettings.AtomScale.ToString("F1");
            BondScale.text = selectedMolecule.RenderSettings.BondScale.ToString("F1");

            // other options
            EnableSimlationBoxToggle.isOn = selectedMolecule.RenderSettings.ShowSimulationBox;
            CalculateBoxEveryFrameToggle.isOn = selectedMolecule.RenderSettings.CalculateBoxEveryFrame;
        }

        public void SaveSettings() {

            if (selectedMolecule == null) {
                return;
            }

            bool visualisationUpdateRequired = false;

            // primary structure options
            if (selectedMolecule.RenderSettings.ShowPrimaryStructure != EnablePrimaryStructureToggle.isOn) {
                selectedMolecule.RenderSettings.ShowPrimaryStructure = EnablePrimaryStructureToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowAtoms != ShowAtomsToggle.isOn) {
                selectedMolecule.RenderSettings.ShowAtoms = ShowAtomsToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowBonds != ShowBondsToggle.isOn) {
                selectedMolecule.RenderSettings.ShowBonds = ShowBondsToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowStandardResidues != ShowStandardResiduesToggle.isOn) {
                selectedMolecule.RenderSettings.ShowStandardResidues = ShowStandardResiduesToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowNonStandardResidues != ShowNonStandardResiduesToggle.isOn) {
                selectedMolecule.RenderSettings.ShowNonStandardResidues = ShowNonStandardResiduesToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowMainChains != ShowMainChainsToggle.isOn) {
                selectedMolecule.RenderSettings.ShowMainChains = ShowMainChainsToggle.isOn;
                visualisationUpdateRequired = true;
            }

            // representation options
            if (ShowCPKToggle.isOn && selectedMolecule.RenderSettings.Representation != MolecularRepresentation.CPK) {
                selectedMolecule.RenderSettings.Representation = MolecularRepresentation.CPK;
                visualisationUpdateRequired = true;
            }
            else if (ShowVDWToggle.isOn && selectedMolecule.RenderSettings.Representation != MolecularRepresentation.VDW) {
                selectedMolecule.RenderSettings.Representation = MolecularRepresentation.VDW;
                visualisationUpdateRequired = true;
            }

            // secondary structure options
            if (selectedMolecule.RenderSettings.ShowSecondaryStructure != EnableSecondaryStructureToggle.isOn) {
                selectedMolecule.RenderSettings.ShowSecondaryStructure = EnableSecondaryStructureToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowHelices != ShowHelicesToggle.isOn) {
                selectedMolecule.RenderSettings.ShowHelices = ShowHelicesToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.ShowSheets != ShowSheetsToggle.isOn) {
                selectedMolecule.RenderSettings.ShowSheets = ShowSheetsToggle.isOn;
                visualisationUpdateRequired = true;
            }

            // other options
            if (selectedMolecule.RenderSettings.ShowSimulationBox != EnableSimlationBoxToggle.isOn) {
                selectedMolecule.RenderSettings.ShowSimulationBox = EnableSimlationBoxToggle.isOn;
                visualisationUpdateRequired = true;
            }

            if (selectedMolecule.RenderSettings.CalculateBoxEveryFrame != CalculateBoxEveryFrameToggle.isOn) {
                selectedMolecule.RenderSettings.CalculateBoxEveryFrame = CalculateBoxEveryFrameToggle.isOn;
                visualisationUpdateRequired = true;
            }

            // recreate structures if necessary
            if (visualisationUpdateRequired) {

                if (selectedMolecule.Hidden) {
                    selectedMolecule.PendingRerender = true;
                }
                else {
                    UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
                }
            }
        }

        public void InreaseAtomScale() {

            selectedMolecule.RenderSettings.AtomScale += scaleIncrementAmount;
            AtomScale.text = selectedMolecule.RenderSettings.AtomScale.ToString("F1");
            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
        }

        public void DecreaseAtomScale() {

            selectedMolecule.RenderSettings.AtomScale -= scaleIncrementAmount;
            AtomScale.text = selectedMolecule.RenderSettings.AtomScale.ToString("F1");
            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
        }

        public void InreaseBondScale() {

            selectedMolecule.RenderSettings.BondScale += scaleIncrementAmount;
            BondScale.text = selectedMolecule.RenderSettings.BondScale.ToString("F1");
            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
        }

        public void DecreaseBondScale() {

            selectedMolecule.RenderSettings.BondScale -= scaleIncrementAmount;
            BondScale.text = selectedMolecule.RenderSettings.BondScale.ToString("F1");
            UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
        }
    }
}

