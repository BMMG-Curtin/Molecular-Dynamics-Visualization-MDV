using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    /// <summary>
    /// All numbers are in nanometres (default Gromacs units)
    /// </summary>
    public static class MolecularConstants {

        /// <summary>
        /// CPK Colours
        /// 
        ///   hydrogen (H)	white
        ///   carbon(C)     black
        ///   nitrogen(N)   sky blue
        ///   oxygen(O)     red
        ///   fluorine(F), chlorine(Cl) green
        ///   bromine(Br)   dark red
        ///   iodine(I)     dark violet 
        ///   noble gases(He, Ne, Ar, Xe, Kr)    cyan
        ///   phosphorus(P) orange
        ///   sulfur(S)     yellow
        ///   boron(B), most transition metals peach, salmon
        ///   alkali metals(Li, Na, K, Rb, Cs, Fr)   violet
        ///   alkaline earth metals(Be, Mg, Ca, Sr, Ba, Ra)  dark silver
        ///   titanium(Ti)  gray
        ///   iron(Fe)      brown silver
        ///   other elements  pink
        /// 
        /// </summary>
        public static Dictionary<string, Color32> CPKColors = new Dictionary<string, Color32>() {

        { "H",  new Color32(255, 255, 255, 255) },
        // { "C",  new Color32(50, 50, 50, 255) }, // modified carbon to dark gray for better visibility
        { "C",  new Color32(0, 125, 125, 255) }, // modified to cyan per request
        { "N",  new Color32(135, 206, 235, 255) },
        { "O",  new Color32(255, 0, 0, 255) },
        { "F",  new Color32(0, 128, 0, 255) },
        { "Cl", new Color32(0, 128, 0, 255) },
        { "Br", new Color32(139, 0, 0, 255) },
        { "I",  new Color32(148, 0, 211, 255) },
        { "He", new Color32(0, 255, 255, 255) },
        { "Ne", new Color32(0, 255, 255, 255) },
        { "Ar", new Color32(0, 255, 255, 255) },
        { "Xe", new Color32(0, 255, 255, 255) },
        { "Kr", new Color32(0, 255, 255, 255) },
        { "P",  new Color32(255, 165, 0, 255) },
        { "S",  new Color32(255, 255, 0, 255) },
        { "B",  new Color32(250, 128, 114, 255) }, // have not included "transition metals"
        { "Li", new Color32(238, 130, 238, 255) },
        { "Na", new Color32(238, 130, 238, 255) },
        { "K",  new Color32(238, 130, 238, 255) },
        { "Rb", new Color32(238, 130, 238, 255) },
        { "Cs", new Color32(238, 130, 238, 255) },
        { "Fr", new Color32(238, 130, 238, 255) },
        { "Be", new Color32(169, 169, 169, 255) },
        { "Mg", new Color32(169, 169, 169, 255) },
        { "Ca", new Color32(169, 169, 169, 255) },
        { "Sr", new Color32(169, 169, 169, 255) },
        { "Ba", new Color32(169, 169, 169, 255) },
        { "Ra", new Color32(169, 169, 169, 255) },
        { "Ti", new Color32(192, 192, 192, 255) },
        { "Fe", new Color32(188, 143, 143, 255) },
        { "Bond", new Color32(230, 230, 230, 255) }, // bond colour
        { "HighlightedBond", new Color32(240, 150, 240, 255) }, // bond colour
        // { "Other",  new Color32(255, 192, 203, 255) }, // colour for atoms not found
        { "Other",  new Color32(255, 0, 0, 255) }, // colour for atoms not found
    };

        // Molecular size in Angstroms
        public static Dictionary<string, float> AtomicRadius = new Dictionary<string, float>() {

        { "H",  0.053f },
        { "He", 0.031f },
        { "Li", 0.167f },
        { "Be", 0.112f },
        { "B",  0.087f },
        { "C",  0.067f },
        { "N",  0.056f },
        { "O",  0.048f },
        { "F",  0.042f },
        { "Ne", 0.038f },
        { "Na", 0.190f },
        { "Mg", 0.145f },
        { "Al", 0.118f },
        { "Si", 0.111f },
        { "P",  0.098f },
        { "S",  0.088f },
        { "Cl", 0.079f },
        { "Ar", 0.071f },
        { "K",  0.243f },
        { "Ca", 0.194f },
        { "Sc", 0.184f },
        { "Ti", 0.176f },
        { "V",  0.171f },
        { "Cr", 0.166f },
        { "Mn", 0.161f },
        { "Fe", 0.156f },
        { "Co", 0.152f },
        { "Ni", 0.149f },
        { "Cu", 0.145f },
        { "Zn", 0.142f },
        { "Ga", 0.136f },
        { "Ge", 0.125f },
        { "As", 0.114f },
        { "Se", 0.103f },
        { "Br", 0.094f },
        { "Kr", 0.088f },
        { "Rb", 0.265f },
        { "Sr", 0.219f },
        { "Y",  0.212f },
        { "Zr", 0.206f },
        { "Nb", 0.198f },
        { "Mo", 0.190f },
        { "Tc", 0.183f },
        { "Ru", 0.178f },
        { "Rh", 0.173f },
        { "Pd", 0.169f },
        { "Ag", 0.165f },
        { "Cd", 0.161f },
        { "In", 0.156f },
        { "Sn", 0.145f },
        { "Sb", 0.133f },
        { "Te", 0.123f },
        { "I",  0.115f },
        { "Xe", 0.108f },
        { "Cs", 0.298f },
        { "Ba", 0.253f },
        { "La", 0.195f },
        { "Ce", 0.185f },
        { "Pr", 0.247f },
        { "Nd", 0.206f },
        { "Pm", 0.205f },
        { "Sm", 0.238f },
        { "Eu", 0.231f },
        { "Gd", 0.233f },
        { "Tb", 0.225f },
        { "Dy", 0.228f },
        { "Ho", 0.226f },
        { "Er", 0.226f },
        { "Tm", 0.222f },
        { "Yb", 0.222f },
        { "Lu", 0.217f },
        { "Hf", 0.208f },
        { "Ta", 0.200f },
        { "W",  0.193f },
        { "Re", 0.188f },
        { "Os", 0.185f },
        { "Ir", 0.180f },
        { "Pt", 0.177f },
        { "Au", 0.174f },
        { "Hg", 0.171f },
        { "Tl", 0.156f },
        { "Pb", 0.154f },
        { "Bi", 0.143f },
        { "Po", 0.135f },
        { "At", 0.127f },
        { "Rn", 0.120f },
        { "Fr", 0.120f },    // unknown, set to Rn as default
        { "Ra", 0.120f },    // unknowm, set to Rn as default 
        { "Ac", 0.195f },
        { "Th", 0.180f },
        { "Pa", 0.180f },
        { "U",  0.175f },
        { "Np", 0.175f },
        { "Pu", 0.175f },
        { "Am", 0.175f },
        { "Cm", 0.175f },    // unknowm, set to Am as default
        { "Other", 0.05f },  // size for atoms not found
    };

        // VDW Radius in Angstoms
        public static Dictionary<string, float> VDWRadius = new Dictionary<string, float>() {

       { "He", 0.140f },
       { "Li", 0.182f },
       { "Be", 0.153f },
       { "B",  0.192f },
       { "C",  0.170f },
       { "N",  0.155f },
       { "O",  0.152f },
       { "F",  0.147f },
       { "Ne", 0.154f },
       { "Na", 0.227f },
       { "Mg", 0.173f },
       { "Al", 0.184f },
       { "Si", 0.210f },
       { "P",  0.180f },
       { "S",  0.180f },
       { "Cl", 0.175f },
       { "Ar", 0.188f },
       { "K",  0.275f },
       { "Ca", 0.231f },
       { "Sc", 0.211f },
       { "Ni", 0.163f },
       { "Cu", 0.140f },
       { "Zn", 0.139f },
       { "Ga", 0.187f },
       { "Ge", 0.211f },
       { "As", 0.185f },
       { "Se", 0.190f },
       { "Br", 0.185f },
       { "Kr", 0.202f },
       { "Rb", 0.303f },
       { "Sr", 0.249f },
       { "Pd", 0.163f },
       { "Ag", 0.172f },
       { "Cd", 0.158f },
       { "In", 0.193f },
       { "Sn", 0.217f },
       { "Sb", 0.206f },
       { "Te", 0.206f },
       { "I",  0.198f },
       { "Xe", 0.216f },
       { "Cs", 0.343f },
       { "Ba", 0.268f },
       { "Pt", 0.175f },
       { "Au", 0.166f },
       { "Hg", 0.155f },
       { "Tl", 0.196f },
       { "Pb", 0.202f },
       { "Bi", 0.207f },
       { "Po", 0.197f },
       { "At", 0.202f },
       { "Rn", 0.220f },
       { "Fr", 0.348f },
       { "Ra", 0.283f },
       { "U",  0.186f },
       { "H",  0.120f },
       { "Other", 0.15f },
    };

        // Element names indexed by element number
        public static List<string[]> ElementNamesByAtomicNumber = new List<string[]>() {

        { new string[] { "H",  "Hydrogen" } },
        { new string[] { "He", "Helium" } },
        { new string[] { "Li", "Lithium" } },
        { new string[] { "Be", "Beryllium" } },
        { new string[] { "B",  "Boron" } },
        { new string[] { "C",  "Carbon" } },
        { new string[] { "N",  "Nitrogen" } },
        { new string[] { "O",  "Oxygen" } },
        { new string[] { "F",  "Fluorine" } },
        { new string[] { "Ne", "Neon" } },
        { new string[] { "Na", "Sodium" } },
        { new string[] { "Mg", "Magnesium" } },
        { new string[] { "Al", "Aluminium" } },
        { new string[] { "Si", "Silicon" } },
        { new string[] { "P",  "Phosphorus" } },
        { new string[] { "S",  "Sulfur" } },
        { new string[] { "Cl", "Chlorine" } },
        { new string[] { "Ar", "Argon" } },
        { new string[] { "K",  "Potassium" } },
        { new string[] { "Ca", "Calcium" } },
        { new string[] { "Sc", "Scandium" } },
        { new string[] { "Ti", "Titanium" } },
        { new string[] { "V",  "Vanadium" } },
        { new string[] { "Cr", "Chromium" } },
        { new string[] { "Mn", "Manganese" } },
        { new string[] { "Fe", "Iron" } },
        { new string[] { "Co", "Cobalt" } },
        { new string[] { "Ni", "Nickel" } },
        { new string[] { "Cu", "Copper" } },
        { new string[] { "Zn", "Zinc" } },
        { new string[] { "Ga", "Gallium" } },
        { new string[] { "Ge", "Germanium" } },
        { new string[] { "As", "Arsenic" } },
        { new string[] { "Se", "Selenium" } },
        { new string[] { "Br", "Bromine" } },
        { new string[] { "Kr", "Krypton" } },
        { new string[] { "Rb", "Rubidium" } },
        { new string[] { "Sr", "Strontium" } },
        { new string[] { "Y",  "Yttrium" } },
        { new string[] { "Zr", "Zirconium" } },
        { new string[] { "Nb", "Niobium" } },
        { new string[] { "Mo", "Molybdenum" } },
        { new string[] { "Tc", "Technetium" } },
        { new string[] { "Ru", "Ruthenium" } },
        { new string[] { "Rh", "Rhodium" } },
        { new string[] { "Pd", "Palladium" } },
        { new string[] { "Ag", "Silver" } },
        { new string[] { "Cd", "Cadmium" } },
        { new string[] { "In", "Indium" } },
        { new string[] { "Sn", "Tin" } },
        { new string[] { "Sb", "Antimony" } },
        { new string[] { "Te", "Tellurium" } },
        { new string[] { "I",  "Iodine" } },
        { new string[] { "Xe", "Xenon" } },
        { new string[] { "Cs", "Caesium" } },
        { new string[] { "Ba", "Barium" } },
        { new string[] { "La", "Lanthanum" } },
        { new string[] { "Ce", "Cerium" } },
        { new string[] { "Pr", "Praseodymium" } },
        { new string[] { "Nd", "Neodymium" } },
        { new string[] { "Pm", "Promethium" } },
        { new string[] { "Sm", "Samarium" } },
        { new string[] { "Eu", "Europium" } },
        { new string[] { "Gd", "Gadolinium" } },
        { new string[] { "Tb", "Terbium" } },
        { new string[] { "Dy", "Dysprosium" } },
        { new string[] { "Ho", "Holmium" } },
        { new string[] { "Er", "Erbium" } },
        { new string[] { "Tm", "Thulium" } },
        { new string[] { "Yb", "Ytterbium" } },
        { new string[] { "Lu", "Lutetium" } },
        { new string[] { "Hf", "Hafnium" } },
        { new string[] { "Ta", "Tantalum" } },
        { new string[] { "W",  "Tungsten" } },
        { new string[] { "Re", "Rhenium" } },
        { new string[] { "Os", "Osmium" } },
        { new string[] { "Ir", "Iridium" } },
        { new string[] { "Pt", "Platinum" } },
        { new string[] { "Au", "Gold" } },
        { new string[] { "Hg", "Mercury" } },
        { new string[] { "Tl", "Thallium" } },
        { new string[] { "Pb", "Lead" } },
        { new string[] { "Bi", "Bismuth" } },
        { new string[] { "Po", "Polonium" } },
        { new string[] { "At", "Astatine" } },
        { new string[] { "Rn", "Radon" } },
        { new string[] { "Fr", "Francium" } },
        { new string[] { "Ra", "Radium" } },
        { new string[] { "Ac", "Actinium" } },
        { new string[] { "Th", "Thorium" } },
        { new string[] { "Pa", "Protactinium" } },
        { new string[] { "U",  "Uranium" } },
        { new string[] { "Np", "Neptunium" } },
        { new string[] { "Pu", "Plutonium" } },
        { new string[] { "Am", "Americium" } },
        { new string[] { "Cm", "Curium" } },
        { new string[] { "Bk", "Berkelium" } },
        { new string[] { "Cf", "Californium" } },
        { new string[] { "Es", "Einsteinium" } },
        { new string[] { "Fm", "Fermium" } },
        { new string[] { "Md", "Mendelevium" } },
        { new string[] { "No", "Nobelium" } },
        { new string[] { "Lr", "Lawrencium" } },
        { new string[] { "Rf", "Rutherfordiu" } },
        { new string[] { "Db", "Dubnium" } },
        { new string[] { "Sg", "Seaborgium" } },
        { new string[] { "Bh", "Bohrium" } },
        { new string[] { "Hs", "Hassium" } },
        { new string[] { "Mt", "Meitnerium" } },
        { new string[] { "Ds", "Darmstadtium" } },
        { new string[] { "Rg", "Roentgenium" } },
        { new string[] { "Cn", "Copernicium" } },
        { new string[] { "Nh", "Nihonium" } },
        { new string[] { "Fl", "Flerovium" } },
        { new string[] { "Mc", "Moscovium" } },
        { new string[] { "Lv", "Livermorium" } },
        { new string[] { "Ts", "Tennessine" } },
        { new string[] { "Og", "Oganesson" } },
    };

        /// <summary>
        /// Numbered comments below indicate atomic number
        /// </summary>
        public static List<Vector2> ElementsChartPosition = new List<Vector2> {

        { new Vector2( 0.5f, 1 ) },  // 1 
        { new Vector2( 18, 1 ) },  // 2 
                              
        { new Vector2( 0.5f, 2 ) },  // 3 
        { new Vector2( 1.5f, 2 ) },  // 4 
        { new Vector2( 13, 2 ) },  // 5 
        { new Vector2( 14, 2 ) },  // 6 
        { new Vector2( 15, 2 ) },  // 7 
        { new Vector2( 16, 2 ) },  // 8 
        { new Vector2( 17, 2 ) },  // 9 
        { new Vector2( 18, 2 ) },  // 10 
                              
        { new Vector2( 0.5f, 3 ) },  // 11 
        { new Vector2( 1.5f, 3 ) },  // 12 
        { new Vector2( 13, 3 ) },  // 13 
        { new Vector2( 14, 3 ) },  // 14 
        { new Vector2( 15, 3 ) },  // 15 
        { new Vector2( 16, 3 ) },  // 16 
        { new Vector2( 17, 3 ) },  // 17 
        { new Vector2( 18, 3 ) },  // 18 
                              
        { new Vector2( 0.5f, 4 ) },  // 19 
        { new Vector2( 1.5f, 4 ) },  // 20
        { new Vector2( 3,  4 ) },  // 21
        { new Vector2( 4,  4 ) },  // 22
        { new Vector2( 5,  4 ) },  // 23
        { new Vector2( 6,  4 ) },  // 24
        { new Vector2( 7,  4 ) },  // 25
        { new Vector2( 8,  4 ) },  // 26
        { new Vector2( 9,  4 ) },  // 27
        { new Vector2( 10, 4 ) }, // 28
        { new Vector2( 11, 4 ) }, // 29
        { new Vector2( 12, 4 ) }, // 30
        { new Vector2( 13, 4 ) }, // 31
        { new Vector2( 14, 4 ) }, // 32
        { new Vector2( 15, 4 ) }, // 33
        { new Vector2( 16, 4 ) }, // 34
        { new Vector2( 17, 4 ) }, // 35
        { new Vector2( 18, 4 ) }, // 36
                              
        { new Vector2( 0.5f, 5 ) },  // 37
        { new Vector2( 1.5f, 5 ) },  // 38
        { new Vector2( 3,  5 ) },  // 39
        { new Vector2( 4,  5 ) },  // 40
        { new Vector2( 5,  5 ) },  // 41
        { new Vector2( 6,  5 ) },  // 42
        { new Vector2( 7,  5 ) },  // 43
        { new Vector2( 8,  5 ) },  // 44
        { new Vector2( 9,  5 ) },  // 45
        { new Vector2( 10, 5 ) }, // 46
        { new Vector2( 11, 5 ) }, // 47
        { new Vector2( 12, 5 ) }, // 48
        { new Vector2( 13, 5 ) }, // 49
        { new Vector2( 14, 5 ) }, // 50
        { new Vector2( 15, 5 ) }, // 51
        { new Vector2( 16, 5 ) }, // 52
        { new Vector2( 17, 5 ) }, // 53
        { new Vector2( 18, 5 ) }, // 54
                               
        { new Vector2( 0.5f, 6 ) }, // 55
        { new Vector2( 1.5f, 6 ) }, // 56
                               
        { new Vector2( 3,  8.5f ) }, // 57
        { new Vector2( 4,  8.5f ) }, // 58
        { new Vector2( 5,  8.5f ) }, // 59
        { new Vector2( 6,  8.5f ) }, // 60
        { new Vector2( 7,  8.5f ) }, // 61
        { new Vector2( 8,  8.5f ) }, // 62
        { new Vector2( 9,  8.5f ) }, // 63
        { new Vector2( 10, 8.5f ) }, // 64
        { new Vector2( 11, 8.5f ) }, // 65
        { new Vector2( 12, 8.5f ) }, // 66
        { new Vector2( 13, 8.5f ) }, // 67
        { new Vector2( 14, 8.5f ) }, // 68
        { new Vector2( 15, 8.5f ) }, // 69
        { new Vector2( 16, 8.5f ) }, // 70
                              
        { new Vector2( 3,  6 ) }, // 71
        { new Vector2( 4,  6 ) }, // 72
        { new Vector2( 6,  6 ) }, // 74
        { new Vector2( 7,  6 ) }, // 75
        { new Vector2( 8,  6 ) }, // 76
        { new Vector2( 9,  6 ) }, // 77
        { new Vector2( 10, 6 ) }, // 78
        { new Vector2( 11, 6 ) }, // 79
        { new Vector2( 12, 6 ) }, // 80
        { new Vector2( 13, 6 ) }, // 81
        { new Vector2( 14, 6 ) }, // 82
        { new Vector2( 15, 6 ) }, // 83
        { new Vector2( 16, 6 ) }, // 84
        { new Vector2( 17, 6 ) }, // 85
        { new Vector2( 18, 6 ) }, // 86
        { new Vector2( 5,  6 ) }, // 73
                               
        { new Vector2( 0.5f, 7 ) }, // 87
        { new Vector2( 1.5f, 7 ) }, // 88
                               
        { new Vector2( 3,  9.5f ) }, // 89
        { new Vector2( 4,  9.5f ) }, // 90
        { new Vector2( 5,  9.5f ) }, // 91
        { new Vector2( 6,  9.5f ) }, // 92
        { new Vector2( 7,  9.5f ) }, // 93
        { new Vector2( 8,  9.5f ) }, // 94
        { new Vector2( 9,  9.5f ) }, // 95
        { new Vector2( 10, 9.5f ) }, // 96
        { new Vector2( 11, 9.5f ) }, // 97
        { new Vector2( 12, 9.5f ) }, // 98
        { new Vector2( 13, 9.5f ) }, // 99
        { new Vector2( 14, 9.5f ) }, // 100
        { new Vector2( 15, 9.5f ) }, // 101
        { new Vector2( 16, 9.5f ) }, // 102
                               
        { new Vector2( 3,  7 ) }, // 103
        { new Vector2( 4,  7 ) }, // 104
        { new Vector2( 5,  7 ) }, // 105
        { new Vector2( 6,  7 ) }, // 106
        { new Vector2( 7,  7 ) }, // 107
        { new Vector2( 8,  7 ) }, // 108
        { new Vector2( 9,  7 ) }, // 109
        { new Vector2( 10, 7 ) }, // 110
        { new Vector2( 11, 7 ) }, // 111
        { new Vector2( 12, 7 ) }, // 112
        { new Vector2( 13, 7 ) }, // 113
        { new Vector2( 14, 7 ) }, // 114
        { new Vector2( 15, 7 ) }, // 115
        { new Vector2( 16, 7 ) }, // 116
        { new Vector2( 17, 7 ) }, // 117
        { new Vector2( 18, 7 ) }, // 118
    };
    }
}

    