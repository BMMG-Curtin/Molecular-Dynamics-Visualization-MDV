
namespace CurtinUniversity.MolecularDynamics.Model.Model {

    /// <summary>
    /// A bond between two atoms. Only the atomIDs are stored
    /// A bond of atom1 to atom2 is equal to a bond of atom2 to atom1. This is reflected in the overriden Equals and GetHashCode methods.
    /// </summary>
    public class Bond {

        public int Atom1Index { get; }
        public int Atom2Index { get; }

        public Bond(int atom1Number, int atom2number) {

            // store lowest number first
            if(atom1Number < atom2number) {
                Atom1Index = atom1Number;
                Atom2Index = atom2number;
            }
            else {
                Atom1Index = atom2number;
                Atom2Index = atom1Number;
            }
        }

        public override bool Equals(object obj) {

            Bond bond = obj as Bond;
            if (bond == null) {
                return false;
            }

            // bonds are equal if both atoms exist in both bond objects. 
            // Lower atom number in both bonds will be in atom number 1
            if(bond.Atom1Index == this.Atom1Index && bond.Atom2Index == this.Atom2Index) { 
                return true;
            }

            return false;
        }
  
        public override int GetHashCode() {

            // atom1 will always be lower than atom2 so the bond hash will always equal
            return (Atom1Index.ToString() + Atom2Index.ToString()).GetHashCode();
        }

        public override string ToString() {

            return "Bond [" + Atom1Index + "][" + Atom2Index + "]";
        }
    }
}
