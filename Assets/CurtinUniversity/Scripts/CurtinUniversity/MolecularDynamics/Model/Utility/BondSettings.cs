using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class BondSettings {

        // this will limit the tree search to improve performance. 
        // Need to experiment with higher values to see performance impact
        public const int MaxBondsPerAtom = 5;

        // Maximum bond length across all bond types
        public const float MaximumLengthAllElements = 0.19f;

        // default value for hydrogen simple calculations, 
        // can be overriden in dictionary below
        public const float MaximumLengthHydrogen = 0.11f;

        // Maximum bond length between elements
        public static Dictionary<ElementPair, float> Lengths = new Dictionary<ElementPair, float>() {
            { new ElementPair(Element.O, Element.O), 0.15f },
            { new ElementPair(Element.O, Element.C), 0.165f },
        };
    }
}
