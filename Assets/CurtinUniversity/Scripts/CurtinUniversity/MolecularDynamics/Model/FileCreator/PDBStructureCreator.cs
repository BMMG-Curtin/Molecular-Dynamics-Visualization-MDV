using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    /// <summary>
    /// This class writes an PDB file based on the supplied primary structure. 
    /// It maps the frame coordinates instead of structure coordinates if supplied.
    /// The purpose of the class is to facilitate stride analysis of non PDB structure files. 
    /// It is tightly coupled to the FileParser atom and residue index inputs to generate a file 
    /// that will match the secondary strcuture residues to the primary structure residues when 
    /// processing the stride output.
    /// </summary>
    public class PDBStructureCreator {

        private PrimaryStructure structure;
        private PrimaryStructureFrame frame = null;

        public PDBStructureCreator(PrimaryStructure structure) {
            this.structure = structure;
        }

        public PDBStructureCreator(PrimaryStructure structure, PrimaryStructureFrame frame) {
            this.structure = structure;
            this.frame = frame;
        }

        public void CreatePDBFile(string filePath, bool mainChainOnly = false, bool omitHetatoms = false, bool omitHydrogen = false) {

            using (StreamWriter writer = new StreamWriter(filePath)) {

                //string testRecord = TestLine(); // for debugging output records
                //writer.WriteLine(testRecord);

                string crystRecord = CrystRecord();
                writer.WriteLine(crystRecord);

                int atomIndex = 1;

                foreach (Chain chain in structure.Chains()) {

                    if(omitHetatoms && chain.ResidueType == StandardResidue.None) {
                        continue;
                    }

                    Atom lastAtom = null;

                    IEnumerable<Residue> residues = null;
                    if (mainChainOnly) {
                        residues = chain.MainChainResidues;
                    }
                    else {
                        residues = chain.Residues;
                    }

                    foreach (Residue residue in residues) { 

                        foreach (Atom atom in residue.Atoms.Values) {

                            if(omitHydrogen && atom.Element == Element.H) {
                                continue;
                            }

                            bool hetAtom = atom.ResidueType != StandardResidue.None ? false : true;

                            Vector3 position;

                            // if no frame number use the primary structure coordinates.
                            if (frame == null) {
                                position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                            }
                            else {
                                position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                            }

                            string atomRecord = AtomRecord(
                                hetAtom,
                                atomIndex,
                                atom.Name,
                                atom.ResidueName,
                                chain.ID.ToString()[0],
                                atom.ResidueIndex.ToString(),
                                position.x * 10,
                                position.y * 10,
                                position.z * 10,
                                atom.Element.ToString()
                            );

                            writer.WriteLine(atomRecord);
                            atomIndex++;
                            lastAtom = atom;
                        }
                    }

                    if (lastAtom != null) {
                        string terRecord = TerRecord(
                            atomIndex,
                            lastAtom.ResidueName,
                            chain.ID.ToString()[0],
                            lastAtom.ResidueIndex.ToString()
                        );

                        writer.WriteLine(terRecord);
                        atomIndex++;
                    }

                    lastAtom = null;
                }

                string endRecord = EndRecord();
                writer.WriteLine(endRecord);
            }
        }

        private string TestLine() {
            return "         1         2         3         4         5         6         7         8\n" +
                   "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
        }

        private string CrystRecord() {
            return "CRYST1";
        }

        private string AtomRecord(bool HET, int serial, string name, string residueName, char chainID, string residueNumber, float xAngstoms, float yAngstoms, float zAngstoms, string elementSymbol) {

            string record = "";

            record += HET ? "HETATM" : "ATOM  ";
            record += serial.ToString().PadLeft(5);
            record += "  "; // pad
            record += name.PadRight(4);
            record += residueName.PadRight(4);
            record += chainID;
            record += residueNumber.PadLeft(4);
            record += "    "; // pad
            record += (string.Format("{0:0.000}", xAngstoms)).PadLeft(8);
            record += (string.Format("{0:0.000}", yAngstoms)).PadLeft(8);
            record += (string.Format("{0:0.000}", zAngstoms)).PadLeft(8);
            record += "                      "; // pad
            record += elementSymbol.PadLeft(2);

            return record;
        }

        private string TerRecord(int serial, string residueName, char chainID, string residueNumber) {

            string record = "";
            record += "TER   ";

            record += serial.ToString().PadLeft(5);
            record += "      "; // pad
            record += residueName.PadRight(4);
            record += chainID;
            record += residueNumber.PadLeft(4);

            return record;
        }

        private string EndRecord() {
            return "END";
        }
    }
}
