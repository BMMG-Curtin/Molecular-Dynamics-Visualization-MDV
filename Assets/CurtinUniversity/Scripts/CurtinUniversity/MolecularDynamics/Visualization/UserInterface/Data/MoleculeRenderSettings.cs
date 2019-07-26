using System;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public struct MoleculeRenderSettings {

        // primary structure settings
        public bool ShowPrimaryStructure { get; set; }
        public bool ShowBonds { get; set; }
        public bool ShowAtoms { get; set; }
        public bool ShowStandardResidues { get; set; }
        public bool ShowNonStandardResidues { get; set; }
        public bool ShowMainChains { get; set; }

        public MolecularRepresentation Representation { get; set; }

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

        // box settings
        public bool ShowSimulationBox { get; set; }
        public bool CalculateBoxEveryFrame { get; set; }

        // element settings
        public HashSet<string> EnabledElements { get; set; }

        // residue settings
        public bool FilterResiduesByNumber;
        public HashSet<int> EnabledResidueNumbers;

        public HashSet<string> EnabledResidueNames;
        public HashSet<string> CustomDisplayResidues;
        public Dictionary<string, Visualization.ResidueDisplayOptions> ResidueOptions;

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

                ShowSimulationBox = true,
                CalculateBoxEveryFrame = false,

                AtomScale = Settings.DefaultAtomScale,
                BondScale = Settings.DefaultBondScale,
            };
        }

        public override bool Equals(object obj) {

            if (obj.GetType() == typeof(MoleculeRenderSettings)) {
                return Equals((MoleculeRenderSettings)obj);
            }

            return false;
        }

        public bool Equals(MoleculeRenderSettings otherSettings) {

            return
                EqualPrimaryStructureSettings(otherSettings) &&
                EqualSecondaryStructureSettings(otherSettings) &&
                EqualBoxSettings(otherSettings) &&
                EqualElementSettings(otherSettings) &&
                EqualResidueSettings(otherSettings);
        }


        public bool EqualPrimaryStructureSettings(MoleculeRenderSettings otherSettings) {

            return
                otherSettings.ShowPrimaryStructure == ShowPrimaryStructure &&
                otherSettings.ShowBonds == ShowBonds &&
                otherSettings.ShowAtoms == ShowAtoms &&
                otherSettings.ShowStandardResidues == ShowStandardResidues &&
                otherSettings.ShowNonStandardResidues == ShowNonStandardResidues &&
                otherSettings.ShowMainChains == ShowMainChains &&
                otherSettings.Representation == Representation && 
                otherSettings.AtomScale == AtomScale &&
                otherSettings.BondScale == BondScale;
        }

        public bool EqualSecondaryStructureSettings(MoleculeRenderSettings otherSettings) {

            return
                otherSettings.ShowSecondaryStructure == ShowSecondaryStructure &&
                otherSettings.ShowHelices == ShowHelices &&
                otherSettings.ShowSheets == ShowSheets &&
                otherSettings.ShowTurns == ShowTurns;
        }

        public bool EqualBoxSettings(MoleculeRenderSettings otherSettings) {

            return
                otherSettings.ShowSimulationBox == ShowSimulationBox &&
                otherSettings.CalculateBoxEveryFrame == CalculateBoxEveryFrame;
        }

        public bool EqualElementSettings(MoleculeRenderSettings otherSettings) {
            return otherSettings.EnabledElements == EnabledElements;
        }

        public bool EqualResidueSettings(MoleculeRenderSettings otherSettings) {

            return
                otherSettings.FilterResiduesByNumber == FilterResiduesByNumber &&
                otherSettings.EnabledResidueNumbers == EnabledResidueNumbers &&
                otherSettings.EnabledResidueNames == EnabledResidueNames &&
                otherSettings.CustomDisplayResidues == CustomDisplayResidues &&
                otherSettings.ResidueOptions == ResidueOptions;
        }
    }
}
