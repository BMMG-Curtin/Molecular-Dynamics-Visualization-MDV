using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Visualization;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class MoleculeRenderSettings {

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

        // need to do element settings and residue settings
        public bool HasHiddenElements { get; set; }
        public HashSet<string> EnabledElements { get; set; }
        public List<Color32> ResidueColours { get; set; }

        public MoleculeRenderSettings() {
            Reset();
        }

        public void Reset() {

            EnablePrimaryStructure = true;
            ShowAtoms = true;
            ShowBonds = true;
            ShowStandardResidues = true;
            ShowNonStandardResidues = true;
            ShowMainChains = true;
            ShowSimulationBox = false;

            Representation = MolecularRepresentation.CPK;

            EnablePrimaryStructure = false;
            ShowHelices = true;
            ShowSheets = true;
            ShowTurns = true;

            HasHiddenElements = false;
        }
    }
}
