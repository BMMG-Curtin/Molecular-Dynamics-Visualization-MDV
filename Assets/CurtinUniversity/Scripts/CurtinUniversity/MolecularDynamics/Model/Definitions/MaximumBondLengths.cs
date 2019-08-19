using System;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.Model {

    public static class MaximumBondLengths {

        // Maximum bond length across all bond types
        public const float MaximumLengthAllElements = 0.19f;

        // Maximum bond length between elements
        public static Dictionary<ElementPair, float> Lengths = new Dictionary<ElementPair, float>() {
            { new ElementPair(ChemicalElement.O, ChemicalElement.O), 0.15f },
            { new ElementPair(ChemicalElement.O, ChemicalElement.C), 0.165f },
        };
    }
}
