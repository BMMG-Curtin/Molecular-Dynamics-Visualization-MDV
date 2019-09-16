using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class GROStructureParser {

        /// <summary>
        /// This method parses a GRO formatted file for a molecular structure.
        /// It will only parse the first frame in the file and return a MolecularModel with one frame. Additional frames in the GRO file will be discarded.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static PrimaryStructure GetStructure(string filename) {

            StreamReader sr = null;
            PrimaryStructure model = null;

            try {
                sr = new StreamReader(filename);
                model = GetFrame(sr);
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

        /// <summary>
        /// This method parses a GRO formatted file for a molecular structures.
        /// If there are multiple 'frames' in the file it returns a list of structures rather than 
        /// a structure and a trajectory. i.e. each frame is a new structure added to the output structure list.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static List<PrimaryStructure> GetAllStructures(string filename) {

            StreamReader sr = null;
            List<PrimaryStructure> models = new List<PrimaryStructure>();
            PrimaryStructure model = null;

            try {
                sr = new StreamReader(filename);
                model = GetFrame(sr);

                while(model != null) {

                    models.Add(model);
                    model = GetFrame(sr);
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

            return models;
        }

        private static PrimaryStructure GetFrame(StreamReader sr) {

            PrimaryStructure model;

            // todo: improve file format error handling for misformatted files
            try {

                model = new PrimaryStructure();

                string titleLine = sr.ReadLine();

                // remove any blank lines before title line
                while (titleLine != null && titleLine.Trim().Equals("")) {
                    titleLine = sr.ReadLine();
                }

                if (titleLine == null) {
                    return null;
                }
                model.Title = titleLine;

                model.Time = 0;

                Regex g = new Regex(@"\st=\s*(\d+\.?\d*)");
                Match m = g.Match(titleLine);
                if (m.Success) {
                    string frameTimeString = m.Groups[1].Value;
                    model.Time = float.Parse(frameTimeString);
                    // Console.WriteLine("Success parsing[" + titleLine + "] for frame time");
                }
                else {
                    // no frame time in header. Assume structure file, leave frame time at 0;
                    // return false;
                }

                int residueIndex = 0;
                int chainIndex = 0;

                Chain chain = null;
                Residue residue = null;
                int currentResidueID = -1;
                Residue lastResidue = null;

                int atomCount = Int32.Parse(sr.ReadLine().Trim());

                for (int i = 0; i < atomCount; i++) {

                    string atomLine = sr.ReadLine();
                    if (atomLine == null) { // if something is wrong with the file, i.e. ends before total atom count
                        break;
                    }

                    // if atom residue number not the existing residue number store the existing residue and instantiate new residue
                    int atomResidueID = int.Parse(atomLine.Substring(0, 5).Trim());

                    if (atomResidueID != currentResidueID) {

                        // store residue - first time here will be null
                        if (residue != null) {
                            model.AddResidue(residueIndex, residue);
                            if (chain != null) {
                                chain.AddResidue(residueIndex, residue);
                            }
                        }

                        residueIndex++;
                        currentResidueID = atomResidueID;
                        string residueName = atomLine.Substring(5, 5).Trim();

                        lastResidue = residue;
                        residue = new Residue(residueIndex, currentResidueID, residueName);

                        // Residue type changes used to capture chain information. May not be 100% accurate but not parsing topology files so options are limited.
                        // Also checking to see if last two atoms before end of amino acid were Oxygen, signifying a chain terminator
                        if((lastResidue == null || residue.ResidueType != lastResidue.ResidueType) ||
                            (lastResidue.ResidueType == StandardResidue.AminoAcid && model.Atoms()[i - 1].Element == Element.O && model.Atoms()[i - 2].Element == Element.O)) {

                            if (chain != null) {
                                model.AddChain(chain);
                            }

                            chainIndex++;
                            chain = new Chain(chainIndex, chainIndex.ToString());
                        }
                    }

                    int atomIndex = i;
                    int atomID = int.Parse((atomLine.Substring(15, 5)).Trim());
                    string atomName = atomLine.Substring(10, 5).Trim();
                    Element element = ElementHelper.Parse(atomName);

                    Vector3 position = new Vector3();
                    position.x = float.Parse(atomLine.Substring(20, 8));
                    position.y = float.Parse(atomLine.Substring(28, 8));
                    position.z = float.Parse(atomLine.Substring(36, 8));

                    Atom atom = new Atom(atomIndex, atomID, atomName, element, position);

                    // check for and store main chain elements. 
                    if (residue.ResidueType != StandardResidue.None) {

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

                    atom.ChainID = chainIndex.ToString();

                    residue.Atoms.Add(atomIndex, atom);
                    model.AddAtom(atomIndex, atom);
                }

                if (residue != null) {
                    model.AddResidue(residueIndex, residue);
                    if (chain != null) {
                        chain.AddResidue(residueIndex, residue);
                    }
                }

                if (chain != null) {
                    model.AddChain(chain);
                }

                // Parse box vectors. 
                // todo: test and make this more robust for various string lengths
                float[] vertices = parseBoxLine(sr.ReadLine());
                model.OriginalBoundingBox = new BoundingBox(vertices[0], vertices[1], vertices[2], vertices[3], vertices[4], vertices[5], vertices[6], vertices[7], vertices[8]);
            }
            catch (Exception e) {
                throw new FileParseException(e.Message);
            }

            return model;
        }

        private static float[] parseBoxLine(string boxLine) {

            float[] vertices = new float[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            string[] values = boxLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < 9; i++) {
                if (values.Length > i) {
                    vertices[i] = float.Parse(values[i]);
                }
            }

            return vertices;
        }
    }
}
