namespace CurtinUniversity.MolecularDynamics.Model.Definitions {

    public enum StandardResidue {
        AminoAcid,
        DNA,
        RNA,
        None   // used for uninitialised
    }

    // Protein
    public enum StandardAminoAcid {

        ALA, // Alanine
        ARG, // Arginine
        ASN, // Asparagine
        ASP, // Aspartic acid
        CYS, // Cysteine
        GLU, // Glutamic acid
        GLN, // Glutamine
        GLY, // Glycine
        HIS, HSE, // Histidine
        ILE, // Isoleucine
        LEU, // Leucine
        LYS, // Lysine
        MET, // Methionine
        PHE, // Phenylalanine
        PRO, // Proline
        SER, // Serine
        THR, // Threonine
        TRP, // Tryptophan
        TYR, // Tyrosine
        VAL  // Valine
    }

    // RNA
    public enum StandardRibonucleotide {

        A, AMP, // Adenosine
        C, CMP, // Cytidine
        G, GMP, // Guanosine
        U, UMP, // Uridine
        I, IMP  // ? Inosinate - Standard in PDB. Not sure in others
    }

    // DNA
    public enum StandardDeoxyribonucleotide {

        DA, // Adenosine 
        DC, // Cytosine
        DG, // Guanine
        DT, // Thymine
        DI, // ? DeoxyInosine - Standard in PDB. Not sure in others
        DU  // ? DeoxyUridine - Standard in PDB. Not sure in others
    }

}
