
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class VisualisationSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI helpText;

        private string initialHelpText;
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
        public Toggle UseFileSimulationBoxToggle;
        public Toggle CalculateBoxEveryFrameToggle;

        public Text AtomScale;
        public Text BondScale;

        public bool SettingsChanged { get; set; }

        private void Awake() {

            initialHelpText = helpText.text;
            SettingsChanged = false;
        }

        public void OnEnable() {

            selectedMolecule = molecules.GetSelected();

            if (selectedMolecule != null) {

                helpText.text = initialHelpText + " - " + selectedMolecule.Name;
                EnableSettings(true);
                LoadSettings();
            }
            else {

                helpText.text = "< no molecule selected >";
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

            // TODO
            // scale options
            //AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
            //BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");

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
            EnablePrimaryStructureToggle.isOn = selectedMolecule.RenderSettings.EnablePrimaryStructure;
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
            EnableSecondaryStructureToggle.isOn = selectedMolecule.RenderSettings.EnableSecondaryStructure;
            ShowHelicesToggle.isOn = selectedMolecule.RenderSettings.ShowHelices;
            ShowSheetsToggle.isOn = selectedMolecule.RenderSettings.ShowSheets;
            ShowTurnsToggle.isOn = selectedMolecule.RenderSettings.ShowTurns;

            // TODO
            // scale options
            //AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
            //BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");

            // other options
            EnableSimlationBoxToggle.isOn = selectedMolecule.RenderSettings.ShowSimulationBox;
            CalculateBoxEveryFrameToggle.isOn = selectedMolecule.RenderSettings.CalculateBoxEveryFrame;
        }

        public void SaveSettings() {

            bool visualisationUpdateRequired = false;

            // primary structure options
            if (selectedMolecule.RenderSettings.EnablePrimaryStructure != EnablePrimaryStructureToggle.isOn) {
                selectedMolecule.RenderSettings.EnablePrimaryStructure = EnablePrimaryStructureToggle.isOn;
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
            if (selectedMolecule.RenderSettings.EnableSecondaryStructure != EnableSecondaryStructureToggle.isOn) {
                selectedMolecule.RenderSettings.EnableSecondaryStructure = EnableSecondaryStructureToggle.isOn;
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
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings);
            }
        }

        //public void InreaseAtomScale() {
        //    sceneManager.StructureView.PrimaryStructureView.IncreaseAtomScale();
        //    AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
        //    StartCoroutine(sceneManager.ReloadModelView(true, false));
        //}

        //public void DecreaseAtomScale() {
        //    sceneManager.StructureView.PrimaryStructureView.DecreaseAtomScale();
        //    AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
        //    StartCoroutine(sceneManager.ReloadModelView(true, false));
        //}

        //public void InreaseBondScale() {
        //    sceneManager.StructureView.PrimaryStructureView.IncreaseBondScale();
        //    BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");
        //    StartCoroutine(sceneManager.ReloadModelView(true, false));
        //}

        //public void DecreaseBondScale() {
        //    sceneManager.StructureView.PrimaryStructureView.DecreaseBondScale();
        //    BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");
        //    StartCoroutine(sceneManager.ReloadModelView(true, false));
        //}
    }
}

