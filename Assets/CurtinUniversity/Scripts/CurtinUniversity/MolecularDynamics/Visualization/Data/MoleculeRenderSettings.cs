using System;
using System.Collections.Generic;
using System.Linq;

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

        public MoleculeRenderSettings Clone() {

            MoleculeRenderSettings renderSettings = new MoleculeRenderSettings();

            renderSettings.ShowPrimaryStructure = this.ShowPrimaryStructure;
            renderSettings.ShowBonds = this.ShowBonds;
            renderSettings.ShowAtoms = this.ShowAtoms;
            renderSettings.ShowStandardResidues = this.ShowStandardResidues;
            renderSettings.ShowNonStandardResidues = this.ShowNonStandardResidues;
            renderSettings.ShowMainChains = this.ShowMainChains;
            renderSettings.Representation = this.Representation;

            renderSettings.AtomScale = this.AtomScale;
            renderSettings.BondScale = this.BondScale;

            renderSettings.ShowSecondaryStructure = this.ShowSecondaryStructure;
            renderSettings.ShowHelices = this.ShowHelices;
            renderSettings.ShowSheets = this.ShowSheets;
            renderSettings.ShowTurns = this.ShowTurns;
            renderSettings.SmoothNodes = this.SmoothNodes;

            renderSettings.ShowSimulationBox = this.ShowSimulationBox;
            renderSettings.CalculateBoxEveryFrame = this.CalculateBoxEveryFrame;

            renderSettings.EnabledElements = this.EnabledElements == null ? null : new HashSet<string>(this.EnabledElements);
            renderSettings.EnabledResidueNames = this.EnabledResidueNames == null ? null : new HashSet<string>(this.EnabledResidueNames);
            renderSettings.CustomResidueNames = this.CustomResidueNames == null ? null : new HashSet<string>(this.CustomResidueNames);
            renderSettings.EnabledResidueIDs = this.EnabledResidueIDs == null ? null : new HashSet<int>(this.EnabledResidueIDs);

            if (this.CustomResidueRenderSettings == null) {
                renderSettings.CustomResidueRenderSettings = null;
            }
            else {

                renderSettings.CustomResidueRenderSettings = new Dictionary<int, ResidueRenderSettings>();

                foreach (KeyValuePair<int, ResidueRenderSettings> item in this.CustomResidueRenderSettings) {
                    renderSettings.CustomResidueRenderSettings.Add(item.Key, item.Value.Clone());
                }
            }

            return renderSettings;
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

            output += EnabledElements == null ? "null\n" : string.Join("", EnabledElements.ToArray()) + "\n";
            output += EnabledResidueNames == null ? "null\n" : string.Join("", EnabledResidueNames.ToArray()) + "\n";
            output += EnabledResidueIDs == null ? "null\n" : string.Join("", Array.ConvertAll(EnabledResidueIDs.ToArray(), element => element.ToString())) + "\n";
            output += CustomResidueNames == null ? "null\n" : string.Join("", CustomResidueNames.ToArray()) + "\n";

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
