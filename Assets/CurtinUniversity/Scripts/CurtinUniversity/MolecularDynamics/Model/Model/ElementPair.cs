
namespace CurtinUniversity.MolecularDynamics.Model {

    // equals and hash are element order independent
    public struct ElementPair {

        public ChemicalElement Element1 { get; private set; }
        public ChemicalElement Element2 { get; private set; }

        public ElementPair(ChemicalElement element1, ChemicalElement element2) {
            Element1 = element1;
            Element2 = element2;
        }

        public override bool Equals(object obj) {

            ElementPair e = (ElementPair)obj;
            if ((this.Element1 == e.Element1 && this.Element2 == e.Element2) ||
                (this.Element1 == e.Element2 && this.Element2 == e.Element1)) {
                return true;
            }

            return false;
        }

        // assumes no more than 999 elements
        public override int GetHashCode() {

            if ((int)Element1 < (int)Element2) {
                return ((int)Element1 * 1000) + (int)Element2;
            }
            else {
                return ((int)Element2 * 1000) + (int)Element1;
            }
        }
    }
}
