using System;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public struct MoleculeRenderSettings {

        public bool EnablePrimaryStructure { get; set; }
        public bool ShowBonds { get; set; }
        public bool ShowAtoms { get; set; }
        public bool ShowStandardResidues { get; set; }
        public bool ShowNonStandardResidues { get; set; }
        public bool ShowMainChains { get; set; }
        public bool ShowSimulationBox { get; set; }

        public MolecularRepresentation Representation { get; set; }

        public bool EnableSecondaryStructure { get; set; }
        public bool ShowHelices { get; set; }
        public bool ShowSheets { get; set; }
        public bool ShowTurns { get; set; }

        public bool EnableSimlationBoxToggle;
        public bool UseFileSimulationBoxToggle;
        public bool CalculateBoxEveryFrameToggle;

        public float ModelScale;
        public float AtomScale;
        public float BondScale;

        // need to do element settings and residue settings
        public bool HasHiddenElements { get; set; }
        public HashSet<string> EnabledElements { get; set; }
        public List<Color32> ResidueColours { get; set; }

        public static MoleculeRenderSettings Default() {

            return new MoleculeRenderSettings() {

                EnablePrimaryStructure = true,
                ShowAtoms = true,
                ShowBonds = true,
                ShowStandardResidues = true,
                ShowNonStandardResidues = true,
                ShowMainChains = true,
                ShowSimulationBox = false,

                Representation = MolecularRepresentation.CPK,

                EnableSecondaryStructure = false,
                ShowHelices = true,
                ShowSheets = true,
                ShowTurns = true,

                ModelScale = Settings.DefaultModelScale,
                AtomScale = Settings.DefaultAtomScale,
                BondScale = Settings.DefaultBondScale,

                HasHiddenElements = false,
            };
        }
    }
}
