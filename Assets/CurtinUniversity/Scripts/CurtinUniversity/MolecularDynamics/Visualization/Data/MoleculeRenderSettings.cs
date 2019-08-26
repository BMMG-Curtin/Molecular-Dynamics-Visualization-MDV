using System;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class MoleculeRenderSettings {

        // primary structure settings
        public bool ShowPrimaryStructure { get; set; }
        public bool ShowBonds { get; set; }
        public bool ShowAtoms { get; set; }
        public bool ShowStandardResidues { get; set; }
        public bool ShowNonStandardResidues { get; set; }
        public bool ShowMainChains { get; set; }

        public MolecularRepresentation Representation { get; set; }

        [SerializeField]
        private float atomScale;
        public float AtomScale {

            get {
                return atomScale;
            }

            set {

                atomScale = value;

                if (atomScale > Settings.MaxAtomScale) {
                    atomScale = Settings.MaxAtomScale;
                }

                if (atomScale < Settings.MinAtomScale) {
                    atomScale = Settings.MinAtomScale;
                }
            }
        }

        [SerializeField]
        private float bondScale;
        public float BondScale {

            get {
                return bondScale;
            }

            set {

                bondScale = value;

                if (bondScale > Settings.MaxBondScale) {
                    bondScale = Settings.MaxBondScale;
                }

                if (bondScale < Settings.MinBondScale) {
                    bondScale = Settings.MinBondScale;
                }
            }
        }

        // secondary structure settings
        public bool ShowSecondaryStructure { get; set; }
        public bool ShowHelices { get; set; }
        public bool ShowSheets { get; set; }
        public bool ShowTurns { get; set; }
        public bool SmoothNodes { get; set; }

        // box settings
        public bool ShowSimulationBox { get; set; }
        public bool CalculateBoxEveryFrame { get; set; }

        // element settings
        public HashSet<string> EnabledElements { get; set; }

        // residue settings
        public HashSet<string> EnabledResidueNames;
        public HashSet<string> CustomResidueNames;
        public HashSet<int> EnabledResidueIDs;
        public Dictionary<int, ResidueRenderSettings> CustomResidueRenderSettings;

        public static MoleculeRenderSettings Default() {

            return new MoleculeRenderSettings() {

                ShowPrimaryStructure = true,
                ShowAtoms = true,
                ShowBonds = true,
                ShowStandardResidues = true,
                ShowNonStandardResidues = true,
                ShowMainChains = false,

                Representation = MolecularRepresentation.CPK,

                ShowSecondaryStructure = false,
                ShowHelices = true,
                ShowSheets = true,
                ShowTurns = true,
                SmoothNodes = true,

                ShowSimulationBox = false,
                CalculateBoxEveryFrame = false,

                AtomScale = Settings.DefaultAtomScale,
                BondScale = Settings.DefaultBondScale,
            };
        }

        public override string ToString() {

            string output =
                "ShowPrimaryStructure: " + ShowPrimaryStructure + "\n" +
                "ShowAtoms: " + ShowAtoms + "\n" +
                "ShowBonds: " + ShowBonds + "\n" +
                "ShowStandardResidues: " + ShowStandardResidues + "\n" +
                "ShowNonStandardResidues: " + ShowNonStandardResidues + "\n" +
                "ShowMainChains: " + ShowMainChains + "\n" +
                "Representation: " + Representation + "\n" +
                "ShowSecondaryStructure: " + ShowSecondaryStructure + "\n" +
                "ShowHelices: " + ShowHelices + "\n" +
                "ShowSheets: " + ShowSheets + "\n" +
                "ShowTurns: " + ShowTurns + "\n" +
                "SmoothNodes: " + SmoothNodes + "\n" +
                "ShowSimulationBox: " + ShowSimulationBox + "\n" +
                "CalculateBoxEveryFrame: " + CalculateBoxEveryFrame + "\n" +
                "AtomScale: " + AtomScale + "\n" +
                "BondScale: " + BondScale + "\n";

            output += EnabledElements == null ? "null\n" : string.Join("", EnabledElements) + "\n";


            output += EnabledResidueNames == null ? "null\n" : string.Join("", EnabledResidueNames) + "\n";
            output += EnabledResidueIDs == null ? "null\n" : string.Join("", EnabledResidueIDs) + "\n";
            output += CustomResidueNames == null ? "null\n" : string.Join("", CustomResidueNames) + "\n";

            if (CustomResidueRenderSettings == null) {
                output += "ResidueOptions: null\n";
            }
            else {
                output += "ResidueOptions:\n";
                foreach (KeyValuePair<int, ResidueRenderSettings> item in CustomResidueRenderSettings) {
                    output += "Residue ID: " + item.Key + ":\n" + item.Value.ToString();
                }
            }

            return output;
        }
    }
}
