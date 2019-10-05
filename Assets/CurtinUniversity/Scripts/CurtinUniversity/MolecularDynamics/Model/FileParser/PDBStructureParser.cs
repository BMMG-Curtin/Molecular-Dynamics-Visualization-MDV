using System;   
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class PDBStructureParser {

        /// <summary>
        /// This method parses a PDB formatted file for a molecular primary structure.
        /// It will only parse the first model in the file (as defined by the ENDMDL record. Additional models in the PDB file will be discarded.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static PrimaryStructure GetPrimaryStructure(string filename) {

            PrimaryStructure model;
            StreamReader sr = null;

            try {
                sr = new StreamReader(filename);

                model = new PrimaryStructure();
                bool EOF = false;

                // all indexing is 1 based and will increment before first assignment
                int atomIndex = 0;
                int residueIndex = 0;
                int chainIndex = 0;

                Chain chain = null;
                Residue residue = null;
                int currentResidueID = -1;
                string currentChainID = "";
                bool TERFound = false;

                while (!EOF) {

                    string record = sr.ReadLine();

                    // Console.WriteLine("processing Line: " + record);

                    if (record == null || record.Trim().Equals("END")) {
                        EOF = true;
                    }
                    else {

                        if (record == null || record.Trim() == "") {
                            continue;
                        }

                        string recordName = record;
                        if (recordName.Length < 6) {
                            recordName = recordName.Trim();
                        }
                        else {
                            recordName = record.Substring(0, 6).Trim();
                        }

                        switch (recordName) {

                            case "ATOM":
                            case "HETATM":

                                int atomResidueID = int.Parse(record.Substring(22, 4).Trim());

                                if (atomResidueID != currentResidueID) {

                                    // store residue - first time here will be null
                                    if (residue != null) {
                                        model.AddResidue(residueIndex, residue);
                                        if (chain != null && currentChainID != "") {
                                            chain.AddResidue(residueIndex, residue);
                                        }
                                    }

                                    residueIndex++;
                                    currentResidueID = atomResidueID;
                                    string residueName = record.Substring(17, 3).Trim();

                                    residue = new Residue(residueIndex, atomResidueID, residueName);
                                }

                                string chainID = record.Substring(21, 1).Trim();

                                if (currentChainID != chainID || TERFound) {

                                    Console.WriteLine("Handling new chain");

                                    if (chain != null && currentChainID != "") {
                                        model.AddChain(chain);
                                    }
                                    if (chainID != null) {
                                        chainIndex++;
                                        chain = new Chain(chainIndex, chainID);
                                    }
                                    currentChainID = chainID;
                                    TERFound = false;
                                }

                                int atomID = int.Parse(record.Substring(6, 5).Trim());
                                string atomName = record.Substring(12, 4).Trim();
                                Element element = ElementHelper.Parse(record.Substring(76, 2).Trim());

                                // PDB coordinates are in angstroms. Convert to nanometres.
                                Vector3 position = new Vector3();
                                position.x = float.Parse(record.Substring(30, 8)) / 10;
                                position.y = float.Parse(record.Substring(38, 8)) / 10;
                                position.z = float.Parse(record.Substring(46, 8)) / 10;

                                atomIndex++;
                                Atom atom = new Atom(atomIndex, atomID, atomName, element, position);

                                // check for and store main chain elements. 
                                if (recordName.Equals("ATOM") && residue.ResidueType != StandardResidue.None) {

                                    switch (atom.Name) {
                                        case "N":
                                            residue.AmineNitrogen = atom;
                                            break;
                                        case "CA":
                                            residue.AlphaCarbon = atom;
                                            break;
                                        case "C":
                                            residue.CarbonylCarbon = atom;
                                            break;
                                        case "O":
                                        case "O1":
                                        case "OT1":
                                            residue.CarbonylOxygen = atom;
                                            break;
                                    }
                                }

                                atom.ResidueIndex = residueIndex;
                                atom.ResidueID = currentResidueID;
                                atom.ResidueName = residue.Name;
                                atom.ResidueType = residue.ResidueType;

                                if (currentChainID != "") {
                                    atom.ChainID = currentChainID;
                                }

                                residue.Atoms.Add(atomIndex, atom);
                                model.AddAtom(atomIndex, atom);

                                break;

                            case "TER":
                                Console.WriteLine("Found TER");
                                TERFound = true;
                                break;

                            case "TITLE":

                                if (model.Title == null) {
                                    model.Title = "";
                                }

                                // many PDB's have long remark sections. Abbreviate for now.
                                if (model.Title.Length < 200) {
                                    model.Title += record.Substring(7) + "\n";
                                }
                                break;

                            case "ENDMDL":
                            case "END":

                                // currently only parse the first model in the file
                                EOF = true;
                                break;
                        }
                    }
                }

                if (residue != null) {
                    model.AddResidue(residueIndex, residue);
                    if (chain != null) {
                        chain.AddResidue(residueIndex, residue);
                    }
                }

                if (chain != null && currentChainID != "") {
                    model.AddChain(chain);
                }
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }
            finally {
                if (sr != null) {
                    sr.Close();
                }
            }

            return model;
        }

        public static SecondaryStructure GetSecondaryStructure(string inputFilePath, string strideExePath) {

            StrideAnalysis stride = new StrideAnalysis(strideExePath);
            return stride.GetSecondaryStructure(inputFilePath);
        }
    }
}