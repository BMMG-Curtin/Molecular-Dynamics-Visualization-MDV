using System;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class ElementHelper {

        public static Element Parse(string element) {

            foreach (string elementID in Enum.GetNames(typeof(Element))) {

                if (element.StartsWith(elementID)) {
                    return (Element)Enum.Parse(typeof(Element), elementID);
                }
            }

            return Element.Other;
        }
    }
}
