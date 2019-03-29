using System;

using CurtinUniversity.MolecularDynamics.Model.Definitions;

namespace CurtinUniversity.MolecularDynamics.Model.Utility {

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
