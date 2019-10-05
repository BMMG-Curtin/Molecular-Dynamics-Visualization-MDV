using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using CurtinUniversity.MolecularDynamics.Model;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public delegate void ResidueRenderSettingsUpdated();

    public enum ResidueUpdateType {
        ID,
        Name,
        All
    }

    public class ResiduesSettingsPanel : MonoBehaviour {

        [SerializeField]
        private MoleculeList molecules;

        [SerializeField]
        private TextMeshProUGUI selectedMoleculeText;

        [SerializeField]
        public ResidueNamesPanel residueNamesPanel;

        [SerializeField]
        public GameObject residueNamesPanelGO;

        [SerializeField]
        public GameObject residueIDsPanelGO;

        [SerializeField]
        public GameObject customRenderSettingsPanelGO;

        [SerializeField]
        public GameObject colourSelectPanelGO;

        [SerializeField]
        public GameObject residueUpdateRangePanelGO;

        private Dictionary<int, PrimaryStructure> primaryStructures;
        private MoleculeSettings selectedMolecule;

        private void Awake() {
            enableResidueSettingsPanels(false);
        }

        private void OnEnable() {
            UpdateSelectedMolecule();
        }

        public void UpdateSelectedMolecule() {

            if(isActiveAndEnabled) {

                selectedMolecule = molecules.GetSelected();

                if (selectedMolecule != null) {

                    selectedMoleculeText.text = "Modifying settings for molecule  - " + selectedMolecule.FileName;

                    if (primaryStructures.ContainsKey(selectedMolecule.ID)) {

                        PrimaryStructure primaryStructure = primaryStructures[selectedMolecule.ID];
                        initialiseResidueRenderSettings(primaryStructure);
                        showResidueNamesPanel(selectedMolecule.RenderSettings, primaryStructure);
                    }
                }
                else {
                    selectedMoleculeText.text = "< no molecule selected >";
                }
            }
        }

        private void OnDisable() {
            enableResidueSettingsPanels(false);
        }

        public void SetPrimaryStructure(int moleculeID, PrimaryStructure primaryStructure) {

            if (primaryStructures == null) {
                primaryStructures = new Dictionary<int, PrimaryStructure>();
            }

            if (!primaryStructures.ContainsKey(moleculeID)) {
                primaryStructures.Add(moleculeID, primaryStructure);
            }
            else {
                primaryStructures[moleculeID] = primaryStructure;
            }
        }

        private void initialiseResidueRenderSettings(PrimaryStructure primaryStructure) {

            if(primaryStructure.ResidueNames == null || primaryStructure.ResidueNames.Count == 0 || 
                primaryStructure.ResidueIDs == null || primaryStructure.ResidueIDs.Count == 0) {
                return;
            }

            // initialise the residue render settings for the molecule
            if (selectedMolecule.RenderSettings.EnabledResidueNames == null) {
                selectedMolecule.RenderSettings.EnabledResidueNames = new HashSet<string>(primaryStructure.ResidueNames);
            }

            if (selectedMolecule.RenderSettings.CustomResidueNames == null) {
                selectedMolecule.RenderSettings.CustomResidueNames = new HashSet<string>();
            }

            if (selectedMolecule.RenderSettings.EnabledResidueIDs == null) {
                selectedMolecule.RenderSettings.EnabledResidueIDs = new HashSet<int>(primaryStructure.ResidueIDs);
            }

            if (selectedMolecule.RenderSettings.CustomResidueRenderSettings == null) {
                selectedMolecule.RenderSettings.CustomResidueRenderSettings = new Dictionary<int, ResidueRenderSettings>();
            }
        }

        private void showResidueNamesPanel(MoleculeRenderSettings renderSettings, PrimaryStructure primaryStructure) {

            residueNamesPanel.Initialise(renderSettings, primaryStructure, updateMoleculeRender);

            if (primaryStructure.ResidueNames == null || primaryStructure.ResidueNames.Count == 0 ||
                primaryStructure.ResidueIDs == null || primaryStructure.ResidueIDs.Count == 0) {

                residueNamesPanelGO.gameObject.SetActive(false);
            }
            else {
                residueNamesPanelGO.gameObject.SetActive(true);
            }
        }

        private void updateMoleculeRender() {

            if (selectedMolecule.Hidden) {
                selectedMolecule.PendingRerender = true;
            }
            else {
                UserInterfaceEvents.RaiseMoleculeRenderSettingsUpdated(selectedMolecule.ID, selectedMolecule.RenderSettings, selectedMolecule.CurrentTrajectoryFrameNumber);
            }
        }

        private void enableResidueSettingsPanels(bool enable) {

            residueNamesPanelGO.gameObject.SetActive(enable);
            residueIDsPanelGO.gameObject.SetActive(enable);
            customRenderSettingsPanelGO.gameObject.SetActive(enable);
            colourSelectPanelGO.gameObject.SetActive(enable);
            residueUpdateRangePanelGO.gameObject.SetActive(enable);
        }
    }
}
