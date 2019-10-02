using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model {

    public struct AtomSigmaEpsilon {
        public float Sigma;
        public float Epsilon;
    }

    public static class InteractionForces {

        // Preliminary look-up table. 
        //
        // Used on a per-element basis with the exception of some hydrogen atoms within certain protein residues 
        // (whereby the atom-type should be used instead). This includes hydrogens labelled “HG”, “HG1” and “HH”
        // for protein residues SER, THR and TYR respectively. Unfortunately there will need to be some sort of 
        // “residue-type” check beforehand because there are many instances where these three atom-types are 
        // also represented in protein residues where the corresponding sigma and epsilon values shouldn’t be applied.
        //
        // One additional exception is for “HG” atoms within CYS residues; 
        // which should ideally have sigma and epsilon values corresponding to the “HS” values that 
        // I’ve listed in the look-up table.

        // # name        sigma      epsilon
        // C           3.40000e-01  3.60000e-01
        // F           3.12000e-01  2.55000e-01
        // H           2.500000-01  6.57000e-02
        // HG          0.00000e+00  0.00000e+00
        // HG1         0.00000e+00  0.00000e+00
        // HH          0.00000e+00  0.00000e+00
        // HS          1.06908e-01  6.56888e-02
        // I           4.20000e-01  1.70000e+00
        // Cl          4.40000e-01  4.20000e-01
        // N           3.25000e-01  7.11000e-01
        // O           3.00000e-01  8.80000e-01
        // P           3.70000e-01  8.40000e-01
        // S           3.56000e-01  1.05000e+00

        // Indexed by Residue Name and Atom Type
        // Values in Angstroms
        public static Dictionary<string, AtomSigmaEpsilon> AtomTypeSigmaEpsilon = new Dictionary<string, AtomSigmaEpsilon> {

            { "SER_HG",  new AtomSigmaEpsilon { Sigma = 0.0000000f, Epsilon = 0.0000000f } },
            { "THR_HG1", new AtomSigmaEpsilon { Sigma = 0.0000000f, Epsilon = 0.0000000f } },
            { "TYR_HH",  new AtomSigmaEpsilon { Sigma = 0.0000000f, Epsilon = 0.0000000f } },
            { "SYS_HG",  new AtomSigmaEpsilon { Sigma = 0.1069080f, Epsilon = 0.0656888f } },
        };

        // Values in Angstroms
        public static Dictionary<Element, AtomSigmaEpsilon> AtomElementSigmaEpsilon = new Dictionary<Element, AtomSigmaEpsilon> {

            { Element.C,  new AtomSigmaEpsilon { Sigma = 0.3400000f, Epsilon = 0.3600000f } },
            { Element.F,  new AtomSigmaEpsilon { Sigma = 0.3120000f, Epsilon = 0.2550000f } },
            { Element.H,  new AtomSigmaEpsilon { Sigma = 0.2500000f, Epsilon = 0.0657000f } },
            { Element.I,  new AtomSigmaEpsilon { Sigma = 0.4200000f, Epsilon = 1.7000000f } },
            { Element.Cl, new AtomSigmaEpsilon { Sigma = 0.4400000f, Epsilon = 0.4200000f } },
            { Element.N,  new AtomSigmaEpsilon { Sigma = 0.3250000f, Epsilon = 0.7110000f } },
            { Element.O,  new AtomSigmaEpsilon { Sigma = 0.3000000f, Epsilon = 0.8800000f } },
            { Element.P,  new AtomSigmaEpsilon { Sigma = 0.3700000f, Epsilon = 0.8400000f } },
            { Element.S,  new AtomSigmaEpsilon { Sigma = 0.3560000f, Epsilon = 1.0500000f } },
        };

        public static AtomSigmaEpsilon defaultSigmaEpsilon = new AtomSigmaEpsilon { Sigma = 0.000000f, Epsilon = 0.000000f };

        public static AtomSigmaEpsilon GetAtomSigmaEpsilon(Atom atom) {

            string atomResidueTypeKey = atom.ResidueName + "_" + atom.Name;
            if (AtomTypeSigmaEpsilon.ContainsKey(atomResidueTypeKey)) {
                return AtomTypeSigmaEpsilon[atomResidueTypeKey];
            }

            if(AtomElementSigmaEpsilon.ContainsKey(atom.Element)) {
                return AtomElementSigmaEpsilon[atom.Element];
            }

            return defaultSigmaEpsilon;
        }
    }
}
