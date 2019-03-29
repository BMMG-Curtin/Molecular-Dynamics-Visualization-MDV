using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class VisualisationPanel : MonoBehaviour {

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
        public Toggle ToggleSmoothRibbons;

        public Toggle ShowCPKToggle;
        public Toggle ShowVDWToggle;

        public Toggle EnableSimlationBoxToggle;
        public Toggle UseFileSimulationBoxToggle;
        public Toggle CalculateBoxEveryFrameToggle;

        public Toggle ShowGroundToggle;
        public Toggle ShowShadowsToggle;
        public Toggle ShowLightsToggle;

        public Text ModelScale;
        public Text AtomScale;
        public Text BondScale;

        public Toggle ToggleStructuresToggle;
        public Toggle RotateModelToggle;
        public Text FrameAnimationSpeed;

        public bool SettingsChanged { get; set; }

        private SceneManager sceneManager;

        void Start() {
            SettingsChanged = false;
        }

        void OnEnable() {

            sceneManager = SceneManager.instance;

            if (Settings.Loaded) { // first enable can fire before scenemanager load
                LoadSettings();
            }
        }

        public void LoadSettings() {

            // external setting changes can call this while panel is not visible. Not need to update panel if this is the case
            if (!enabled) {
                return;
            }

            // primary structure options
            EnablePrimaryStructureToggle.isOn = Settings.EnablePrimaryStructure;
            ShowAtomsToggle.isOn = Settings.ShowAtoms;
            ShowBondsToggle.isOn = Settings.ShowBonds;
            ShowStandardResiduesToggle.isOn = Settings.ShowStandardResidues;
            ShowNonStandardResiduesToggle.isOn = Settings.ShowNonStandardResidues;
            ShowMainChainsToggle.isOn = Settings.ShowMainChains;

            // representation options
            ShowCPKToggle.isOn = false;
            ShowVDWToggle.isOn = false;
            switch (Settings.Representation) {
                case MolecularRepresentation.CPK:
                    ShowCPKToggle.isOn = true;
                    break;
                case MolecularRepresentation.VDW:
                    ShowVDWToggle.isOn = true;
                    break;
            }

            // secondary structure options
            EnableSecondaryStructureToggle.isOn = Settings.EnableSecondaryStructure;
            ShowHelicesToggle.isOn = Settings.ShowHelices;
            ShowSheetsToggle.isOn = Settings.ShowSheets;
            ShowTurnsToggle.isOn = Settings.ShowTurns;

            // animation options
            RotateModelToggle.isOn = Settings.ModelRotate;
            FrameAnimationSpeed.text = sceneManager.StructureView.AnimationSpeed.ToString();

            // scene options
            ShowGroundToggle.isOn = Settings.ShowGround;
            ShowShadowsToggle.isOn = Settings.ShowShadows;
            ShowLightsToggle.isOn = Settings.ShowLights;

            // scale options
            ModelScale.text = sceneManager.Model.Scale.ToString("F1");
            AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
            BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");

            // other options
            ToggleStructuresToggle.isOn = Settings.ToggleStructures;
            ToggleSmoothRibbons.isOn = Settings.SmoothRibbons;
            EnableSimlationBoxToggle.isOn = Settings.ShowSimulationBox;
            UseFileSimulationBoxToggle.isOn = Settings.UseFileSimulationBox;
            CalculateBoxEveryFrameToggle.isOn = Settings.CalculateBoxEveryFrame;
        }

        public void SaveSettings() {

            SettingsChanged = true; // the only time this method is called is when there has been a settings update.

            bool primaryStructureReloadRequired = false;
            bool secondaryStructureReloadRequired = false;

            // primary structure options
            if (Settings.EnablePrimaryStructure != EnablePrimaryStructureToggle.isOn) {
                Settings.EnablePrimaryStructure = EnablePrimaryStructureToggle.isOn;
                primaryStructureReloadRequired = true;

                if (Settings.ToggleStructures) {
                    if (Settings.EnablePrimaryStructure && Settings.EnableSecondaryStructure) {
                        EnableSecondaryStructureToggle.isOn = false;
                        Settings.EnableSecondaryStructure = false;
                        secondaryStructureReloadRequired = true;
                    }
                }
            }

            if (Settings.ShowAtoms != ShowAtomsToggle.isOn) {
                Settings.ShowAtoms = ShowAtomsToggle.isOn;
                primaryStructureReloadRequired = true;
            }

            if (Settings.ShowBonds != ShowBondsToggle.isOn) {
                Settings.ShowBonds = ShowBondsToggle.isOn;
                primaryStructureReloadRequired = true;
            }

            if (Settings.ShowStandardResidues != ShowStandardResiduesToggle.isOn) {
                Settings.ShowStandardResidues = ShowStandardResiduesToggle.isOn;
                primaryStructureReloadRequired = true;
            }

            if (Settings.ShowNonStandardResidues != ShowNonStandardResiduesToggle.isOn) {
                Settings.ShowNonStandardResidues = ShowNonStandardResiduesToggle.isOn;
                primaryStructureReloadRequired = true;
            }

            if (Settings.ShowMainChains != ShowMainChainsToggle.isOn) {
                Settings.ShowMainChains = ShowMainChainsToggle.isOn;
                primaryStructureReloadRequired = true;
            }

            // representation options
            if (ShowCPKToggle.isOn && Settings.Representation != MolecularRepresentation.CPK) {
                Settings.Representation = MolecularRepresentation.CPK;
                primaryStructureReloadRequired = true;
            }
            else if (ShowVDWToggle.isOn && Settings.Representation != MolecularRepresentation.VDW) {
                Settings.Representation = MolecularRepresentation.VDW;
                primaryStructureReloadRequired = true;
            }

            // secondary structure options
            if (Settings.EnableSecondaryStructure != EnableSecondaryStructureToggle.isOn) {
                Settings.EnableSecondaryStructure = EnableSecondaryStructureToggle.isOn;
                secondaryStructureReloadRequired = true;

                if (Settings.ToggleStructures) {
                    if (Settings.EnablePrimaryStructure && Settings.EnableSecondaryStructure) {
                        EnablePrimaryStructureToggle.isOn = false;
                        Settings.EnablePrimaryStructure = false;
                        primaryStructureReloadRequired = true;
                    }
                }
            }

            if (Settings.ShowHelices != ShowHelicesToggle.isOn) {
                Settings.ShowHelices = ShowHelicesToggle.isOn;
                secondaryStructureReloadRequired = true;
            }

            if (Settings.ShowSheets != ShowSheetsToggle.isOn) {
                Settings.ShowSheets = ShowSheetsToggle.isOn;
                secondaryStructureReloadRequired = true;
            }

            // animation options
            if (RotateModelToggle.isOn != Settings.ModelRotate) {
                Settings.ModelRotate = RotateModelToggle.isOn;
            }

            // scene options
            if (Settings.ShowGround != ShowGroundToggle.isOn) {
                Settings.ShowGround = ShowGroundToggle.isOn;
                sceneManager.Ground.SetActive(Settings.ShowGround);
            }

            if (Settings.ShowShadows != ShowShadowsToggle.isOn) {
                Settings.ShowShadows = ShowShadowsToggle.isOn;
                sceneManager.Lighting.EnableShadows(Settings.ShowShadows);
            }

            if (Settings.ShowLights != ShowLightsToggle.isOn) {
                Settings.ShowLights = ShowLightsToggle.isOn;
                sceneManager.Lighting.EnableLighting(Settings.ShowLights);
            }


            // other options
            if (Settings.ShowSimulationBox != EnableSimlationBoxToggle.isOn) {
                Settings.ShowSimulationBox = EnableSimlationBoxToggle.isOn;
                sceneManager.MolecularModelBox.SetActive(Settings.ShowSimulationBox);
                sceneManager.ModelBox.Show(Settings.ShowSimulationBox);
            }

            if (Settings.UseFileSimulationBox != UseFileSimulationBoxToggle.isOn) {
                Settings.UseFileSimulationBox = UseFileSimulationBoxToggle.isOn;
                sceneManager.UpdateModelBox();
            }

            if (Settings.CalculateBoxEveryFrame != CalculateBoxEveryFrameToggle.isOn) {
                Settings.CalculateBoxEveryFrame = CalculateBoxEveryFrameToggle.isOn;
                sceneManager.UpdateModelBox();
                primaryStructureReloadRequired = true;
            }

            if (Settings.ToggleStructures != ToggleStructuresToggle.isOn) {
                Settings.ToggleStructures = ToggleStructuresToggle.isOn;
            }

            if (Settings.SmoothRibbons != ToggleSmoothRibbons.isOn) {
                Settings.SmoothRibbons = ToggleSmoothRibbons.isOn;
                secondaryStructureReloadRequired = true;
            }

            // reload scene structures if necessary
            if (primaryStructureReloadRequired || secondaryStructureReloadRequired) {
                StartCoroutine(sceneManager.ReloadModelView(primaryStructureReloadRequired, secondaryStructureReloadRequired));
            }
        }

        public void InreaseModelScale() {
            sceneManager.Model.IncreaseScale();
            ModelScale.text = sceneManager.Model.Scale.ToString("F1");
        }

        public void DecreaseModelScale() {
            sceneManager.Model.DecreaseScale();
            ModelScale.text = sceneManager.Model.Scale.ToString("F1");
        }

        public void InreaseAtomScale() {
            sceneManager.StructureView.PrimaryStructureView.IncreaseAtomScale();
            AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
            StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public void DecreaseAtomScale() {
            sceneManager.StructureView.PrimaryStructureView.DecreaseAtomScale();
            AtomScale.text = sceneManager.StructureView.PrimaryStructureView.AtomScale.ToString("F1");
            StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public void InreaseBondScale() {
            sceneManager.StructureView.PrimaryStructureView.IncreaseBondScale();
            BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");
            StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public void DecreaseBondScale() {
            sceneManager.StructureView.PrimaryStructureView.DecreaseBondScale();
            BondScale.text = sceneManager.StructureView.PrimaryStructureView.BondScale.ToString("F1");
            StartCoroutine(sceneManager.ReloadModelView(true, false));
        }

        public void InreaseFrameAnimationSpeed() {
            SceneManager.instance.StructureView.AnimationSpeed += 1;
            FrameAnimationSpeed.text = SceneManager.instance.StructureView.AnimationSpeed.ToString();
        }

        public void DecreaseFrameAnimationSpeed() {
            SceneManager.instance.StructureView.AnimationSpeed -= 1;
            FrameAnimationSpeed.text = SceneManager.instance.StructureView.AnimationSpeed.ToString();
        }
    }
}
