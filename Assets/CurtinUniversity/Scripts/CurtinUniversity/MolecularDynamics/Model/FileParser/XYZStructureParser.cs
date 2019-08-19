using System;
using System.IO;
using System.Text.RegularExpressions;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class XYZStructureParser {

        public static PrimaryStructure GetStructure(string filename) {

            PrimaryStructure model;
            StreamReader sr = null;

            try {
                sr = new StreamReader(filename);

                int atomCount = Int32.Parse(sr.ReadLine().Trim());
                string titleLine = sr.ReadLine();

                model = new PrimaryStructure();
                model.Title = titleLine;
                model.Time = 0;

                for (int i = 0; i < atomCount; i++) {

                    string atomLine = sr.ReadLine();
                    Regex g = new Regex(@"\s*?(\w+)\s+([\d\.\-]+)\s+([\d\.\-]+)\s+([\d\.\-]+).*$");
                    Match m = g.Match(atomLine);

                    int atomID = int.Parse(i.ToString());
                    string name = m.Groups[1].Value.Trim();
                    ChemicalElement element = ElementHelper.Parse(name);

                    Vector3 position = new Vector3();
                    position.x = float.Parse(m.Groups[2].Value) / 10f;
                    position.y = float.Parse(m.Groups[3].Value) / 10f;
                    position.z = float.Parse(m.Groups[4].Value) / 10f;

                    Atom atom = new Atom(i, atomID, name, element, position);

                    atom.ResidueType = StandardResidue.None;
                    model.AddAtom(i, atom);
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
    }
}
