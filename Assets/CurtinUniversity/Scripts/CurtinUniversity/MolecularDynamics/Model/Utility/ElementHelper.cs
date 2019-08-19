using System;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class ElementHelper {

        public static ChemicalElement Parse(string element) {

            foreach (string elementID in Enum.GetNames(typeof(ChemicalElement))) {

                if (element.StartsWith(elementID)) {
                    return (ChemicalElement)Enum.Parse(typeof(ChemicalElement), elementID);
                }
            }

            return ChemicalElement.None;
        }
    }
}
