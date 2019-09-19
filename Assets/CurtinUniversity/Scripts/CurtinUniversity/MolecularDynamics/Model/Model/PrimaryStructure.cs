using System;
using System.Collections.Generic;

/// <summary>
/// Molecular Model including structure and trajectory. 
/// Coordinate structure is in nanometres. 1 nanometre = 10 angstoms
/// </summary>
namespace CurtinUniversity.MolecularDynamics.Model {

    public class PrimaryStructure {

        public string Title { get; set; }
        public BoundingBox OriginalBoundingBox { get; set; } // stores the bounding box from the file. Will ne null if there was none
        public float Time { get; set; }

        // Note these model components are stored here by an independent index number that starts at 1
        // The name/ID/Serial number of each component is not necessarily unique within the model and cannot be used as a unique identifier

        private List<Chain> chains;          // global accessor to chains. Includes residues specific to the molecule. Indexed by input file order.
        private Dictionary<int, Residue> residues;      // global accessor for residues. Includes atoms specific to the residues. Indexed by input file order.
        private List<Atom> atoms;            // Global accessor for all atoms associated with this model. Indexed by input file order.
                                             // Atom positions drawn from stucture file. Trajectory positions can be retrieved from trajectory frames by index number.

        private Dictionary<int, Atom> atomsByID;

        private HashSet<string> elementNames; // unique element names in this model
        private HashSet<string> residueNames; // unique residue names in this model
        private HashSet<int> residueIDs; // unique residue names in this model


        public PrimaryStructure() {

            chains = new List<Chain>();
            residues = new Dictionary<int, Residue>();
            atoms = new List<Atom>();
            atomsByID = new Dictionary<int, Atom>();
        }


        // Atom Methods ///////////////////////////////////////////////////////

        public List<Atom> Atoms() {
            return atoms;
        }

        public Dictionary<int, Atom> AtomsByID() {
            return atomsByID;
        }

        public int AtomCount() {

            if (atoms != null) {
                return atoms.Count;
            }
            else {
                return 0;
            }
        }

        public void AddAtom(int atomIndex, Atom atom) {

            try {
                atoms.Add(atom);
                atomsByID.Add(atomIndex, atom);
            }
            catch (ArgumentException) {
                // do nothing. Don't add new data that clashes with existing data.
            }
        }

        public bool ContainsAtom(int atomIndex) {

            if(atomIndex >= 0 && atomIndex < atoms.Count) {
                return true;
            }

            return false;
        }

        public List<Atom> GetAtoms(Element element) {

            List<Atom> elementAtoms = new List<Atom>();

            foreach (Atom atom in atoms) {

                if (atom.Element == element) {
                    elementAtoms.Add(atom);
                }
            }

            return atoms;
        }

        public Dictionary<int, Atom> GetAtoms(List<int> residueIDs) {

            Dictionary<int, Atom> combinedAtoms = new Dictionary<int, Atom>();

            foreach (int residueID in residueIDs) {
                if (residues.ContainsKey(residueID)) {
                    foreach (KeyValuePair<int, Atom> atoms in residues[residueID].Atoms) {
                        combinedAtoms.Add(atoms.Key, atoms.Value);
                    }
                }
            }

            return combinedAtoms;
        }

        public Dictionary<int, Atom> GetAtoms(bool standardAtoms = true, bool nonStandardAtoms = true, HashSet<string> includedElements = null, HashSet<string> includedResidueNames = null, HashSet<int> includedResidueIDs = null) {

            Dictionary<int, Atom> returnAtoms = new Dictionary<int, Atom>();

            foreach (Atom atom in atoms) {

                if ((standardAtoms && atom.ResidueType != StandardResidue.None) ||
                    (nonStandardAtoms && atom.ResidueType == StandardResidue.None)) {

                    if (includedElements == null || includedElements.Contains(atom.Element.ToString().ToUpper())) {

                        if (includedResidueNames == null || (atom.ResidueName != null && includedResidueNames.Contains(atom.ResidueName.ToString().ToUpper()))) {

                            if (includedResidueIDs == null || (includedResidueIDs.Contains(atom.ResidueID))) {

                                returnAtoms.Add(atom.Index, atom);
                            }
                        }
                    }
                }
            }

            return returnAtoms;
        }

        public Dictionary<int, Atom> GetAtoms(bool standardAtoms = true, bool nonStandardAtoms = true, HashSet<string> includedElements = null, HashSet<int> includedResidues = null) {

            Dictionary<int, Atom> returnAtoms = new Dictionary<int, Atom>();

            foreach (Atom atom in atoms) {

                if ((standardAtoms && atom.ResidueType != StandardResidue.None) ||
                    (nonStandardAtoms && atom.ResidueType == StandardResidue.None)) {

                    if (includedElements == null || includedElements.Contains(atom.Element.ToString().ToUpper())) {

                        if (includedResidues == null || (includedResidues.Contains(atom.ResidueID))) {

                            returnAtoms.Add(atom.Index, atom);
                        }
                    }
                }
            }

            return returnAtoms;
        }

        public Dictionary<Element, Dictionary<int, Atom>> GetAtomsByElement(bool standardAtoms = true, bool nonStandardAtoms = true, HashSet<string> includedElements = null, HashSet<string> includedResidues = null) {

            Dictionary<Element, Dictionary<int, Atom>> returnAtoms = new Dictionary<Element, Dictionary<int, Atom>>();

            foreach (Atom atom in atoms) {

                if ((standardAtoms && atom.ResidueType != StandardResidue.None) ||
                    (nonStandardAtoms && atom.ResidueType == StandardResidue.None)) {

                    if (includedElements == null || includedElements.Contains(atom.Element.ToString().ToUpper())) {

                        if (includedResidues == null || (atom.ResidueName != null && includedResidues.Contains(atom.ResidueName.ToString().ToUpper()))) {

                            if (!returnAtoms.ContainsKey(atom.Element)) {
                                Dictionary<int, Atom> elementAtoms = new Dictionary<int, Atom>();
                                returnAtoms.Add(atom.Element, elementAtoms);
                            }

                            returnAtoms[atom.Element].Add(atom.Index, atom);
                        }
                    }
                }
            }

            return returnAtoms;
        }

        public Dictionary<Element, Dictionary<int, Atom>> GetStandardResidueAtomsByElement() {

            Dictionary<Element, Dictionary<int, Atom>> atoms = new Dictionary<Element, Dictionary<int, Atom>>();

            foreach (KeyValuePair<int, Residue> residue in Residues()) {

                if (residue.Value.ResidueType == StandardResidue.None) {
                    continue;
                }

                foreach (KeyValuePair<int, Atom> atom in residue.Value.Atoms) {

                    if (!atoms.ContainsKey(atom.Value.Element)) {
                        Dictionary<int, Atom> elementAtoms = new Dictionary<int, Atom>();
                        atoms.Add(atom.Value.Element, elementAtoms);
                    }

                    atoms[atom.Value.Element].Add(atom.Key, atom.Value);
                }
            }

            return atoms;
        }

        public Dictionary<int, Atom> GetNonStandardResidueAtoms() {

            Dictionary<int, Atom> atoms = new Dictionary<int, Atom>();

            foreach (KeyValuePair<int, Residue> residue in Residues()) {

                if (residue.Value.ResidueType != StandardResidue.None) {
                    continue;
                }

                foreach (KeyValuePair<int, Atom> atom in residue.Value.Atoms) {
                    atoms.Add(atom.Key, atom.Value);
                }
            }

            return atoms;
        }

        public Dictionary<Element, Dictionary<int, Atom>> GetNonStandardResidueAtomsByElement() {

            Dictionary<Element, Dictionary<int, Atom>> atoms = new Dictionary<Element, Dictionary<int, Atom>>();

            foreach (KeyValuePair<int, Residue> residue in Residues()) {

                if (residue.Value.ResidueType != StandardResidue.None) {
                    continue;
                }

                foreach (KeyValuePair<int, Atom> atom in residue.Value.Atoms) {

                    if (!atoms.ContainsKey(atom.Value.Element)) {
                        Dictionary<int, Atom> elementAtoms = new Dictionary<int, Atom>();
                        atoms.Add(atom.Value.Element, elementAtoms);
                    }

                    atoms[atom.Value.Element].Add(atom.Key, atom.Value);
                }
            }

            return atoms;
        }


        // Residue Methods ////////////////////////////////////////////////////

        public Dictionary<int, Residue> Residues() {
            return residues;
        }

        public int ResidueCount() {

            if (residues != null) {
                return residues.Count;
            }
            else {
                return 0;
            }
        }

        public void AddResidue(int residueIndex, Residue residue) {

            try {
                residues.Add(residueIndex, residue);
            }
            catch (ArgumentException) {
                // do nothing. Don't add new data that clashes with existing data.
            }
        }

        public bool ContainsResidue(int residueIndex) {

            if (residues.ContainsKey(residueIndex)) {
                return true;
            }

            return false;
        }

        public Residue GetResidue(int residueIndex) {

            if (residues.ContainsKey(residueIndex)) {
                return residues[residueIndex];
            }

            return null;
        }

        public List<Residue> GetResiduesByName(string residueName) {

            List<Residue> matchingResidues = new List<Residue>();

            foreach (KeyValuePair<int, Residue> residue in residues) {
                if (residue.Value.Name == residueName) {
                    matchingResidues.Add(residue.Value);
                }
            }

            return matchingResidues;
        }

        public List<Residue> GetResiduesByID(int residueID) {

            // residue IDs can match multiple residues
            List<Residue> matchingResidues = new List<Residue>();

            foreach (KeyValuePair<int, Residue> residue in residues) {
                if (residue.Value.ID == residueID) {
                    matchingResidues.Add(residue.Value);
                }
            }

            return matchingResidues;
        }

        public List<Residue> GetResiduesByID(List<int> residueIDs) {

            // residue IDs can match multiple residues
            List<Residue> matchingResidues = new List<Residue>();

            foreach (KeyValuePair<int, Residue> residue in residues) {
                if (residueIDs.Contains(residue.Value.ID)) {
                    matchingResidues.Add(residue.Value);
                }
            }

            return matchingResidues;
        }

        public HashSet<int> ResidueIDs {

            get {

                if (residueIDs != null) {
                    return residueIDs;
                }

                // we use a HashSet because residue names are not unique when iterating across residues
                residueIDs = new HashSet<int>();

                foreach (KeyValuePair<int, Residue> residue in residues) {
                    residueIDs.Add(residue.Value.ID);
                }

                return residueIDs;
            }
        }

        public HashSet<int> GetResidueIDs(List<string> residueNames) {

            // we use a HashSet because residue IDs are not unique when iterating across residues
            HashSet<int> residueIDs = new HashSet<int>();

            foreach (KeyValuePair<int, Residue> residue in residues) {
                if(residueNames.Contains(residue.Value.Name)) {
                    residueIDs.Add(residue.Value.ID);
                }
            }

            return residueIDs;
        }

        public HashSet<string> ResidueNames {

            get {
                if (residueNames != null) {
                    return residueNames;
                }

                // we use a HashSet because residue names are not unique when iterating across residues
                residueNames = new HashSet<string>();

                foreach (KeyValuePair<int, Residue> residue in residues) {
                    residueNames.Add(residue.Value.Name);
                }

                return residueNames;
            }
        }

        public HashSet<string> ResidueNamesForIDs(List<int> residueIDs) {

            // we use a HashSet because residue names are not unique when iterating across residues
            HashSet<string> matchedNames = new HashSet<string>();

            foreach (KeyValuePair<int, Residue> residue in residues) {
                if(residueIDs.Contains(residue.Value.ID)) {
                    matchedNames.Add(residue.Value.Name);
                }
            }

            return matchedNames;
        }

        public Dictionary<string, HashSet<int>> GetResidueIDsByName() {

            Dictionary<string, HashSet<int>> residuesByName = new Dictionary<string, HashSet<int>>();

            foreach(string residueName in ResidueNames) {

                HashSet<int> residueIDs = GetResidueIDs(new List<string>() { residueName });
                residuesByName.Add(residueName, residueIDs);
            }

            return residuesByName;
        }

        public Dictionary<int, Residue> GetStandardResidues() {

            Dictionary<int, Residue> residues = new Dictionary<int, Residue>();

            foreach (KeyValuePair<int, Residue> residue in Residues()) {

                if (residue.Value.ResidueType != StandardResidue.None) {
                    residues.Add(residue.Key, residue.Value);
                }
            }

            return residues;
        }

        public Dictionary<int, Residue> GetNonStandardResidues() {

            Dictionary<int, Residue> residues = new Dictionary<int, Residue>();

            foreach (KeyValuePair<int, Residue> residue in Residues()) {

                if (residue.Value.ResidueType == StandardResidue.None) {
                    residues.Add(residue.Key, residue.Value);
                }
            }

            return residues;
        }



        // Chain Methods //////////////////////////////////////////////////////
        
        public List<Chain> Chains() {
            return chains;
        }

        public int ChainCount() {

            if (chains != null) {
                return chains.Count;
            }
            else {
                return 0;
            }
        }

        public void AddChain(Chain chain) {

            try {
                chains.Add(chain);
            }
            catch (ArgumentException) {
                // do nothing. Don't add new data that clashes with existing data.
            }
        }

        public Chain GetChainByID(string chainID) {

            foreach(Chain chain in chains) {

                if(chain.ID == chainID) {
                    return chain;
                }
            }

            return null;
        }


        // Other Methods //////////////////////////////////////////////////////

        public HashSet<string> ElementNames {

            get {
                if (elementNames != null) {
                    return elementNames;
                }

                elementNames = new HashSet<string>();

                foreach (Atom atom in Atoms()) {
                    elementNames.Add(atom.Element.ToString().ToUpper());
                }

                return elementNames;
            }
        }
        
        public override string ToString() {

            string output = "";

            List<int> keyList = new List<int>(residues.Keys);
            keyList.Sort();

            foreach (int key in keyList) {
                output += "\t" + residues[key];
            }

            return output;
        }

        public Dictionary<int, Bond> GenerateBonds(int processorCores) {

            BondCalculator bondCalculator = new BondCalculator();
            return bondCalculator.CalculateBonds(atomsByID, processorCores);
        }
    }
}
