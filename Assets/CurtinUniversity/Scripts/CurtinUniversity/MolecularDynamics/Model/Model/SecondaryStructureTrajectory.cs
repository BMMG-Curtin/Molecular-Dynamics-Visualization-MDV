using System;
using System.Collections.Generic;

using CurtinUniversity.MolecularDynamics.Model.Analysis;
using CurtinUniversity.MolecularDynamics.Model.FileCreator;
using CurtinUniversity.MolecularDynamics.Visualization.Utility;

namespace CurtinUniversity.MolecularDynamics.Model.Model {

    public class SecondaryStructureTrajectory {

        private PrimaryStructure primaryStructure;
        private PrimaryStructureTrajectory primaryTrajectory;
        private String strideExePath;
        private String tmpFilePath;

        private Dictionary<int, SecondaryStructure> secondaryStructuresFrames; // set of calculated secondary structures, indexed by trajectory frame number.
        private HashSet<int> badFrames;

        public SecondaryStructureTrajectory(PrimaryStructure primaryStructure, PrimaryStructureTrajectory primaryTrajectory, string strideExePath, string tmpFilePath) {

            this.primaryStructure = primaryStructure;
            this.primaryTrajectory = primaryTrajectory;
            this.strideExePath = strideExePath;
            this.tmpFilePath = tmpFilePath;

            secondaryStructuresFrames = new Dictionary<int, SecondaryStructure>();
            badFrames = new HashSet<int>();
        }

        public SecondaryStructure GetStructure(int frameNumber) {

            if(badFrames.Contains(frameNumber)) {
                return null;
            }

            SecondaryStructure secondaryStructure;
            if (secondaryStructuresFrames.TryGetValue(frameNumber, out secondaryStructure)) {
                return secondaryStructure;
            }
            else {
                try {
                    secondaryStructure = CalculateSecondaryStructure(frameNumber);
                    secondaryStructuresFrames.Add(frameNumber, secondaryStructure);
                    return secondaryStructure;
                }
                catch(StrideException ex) {
                    badFrames.Add(frameNumber);
                    throw new StrideException(ex.Message);
                }
            }
        }

        private SecondaryStructure CalculateSecondaryStructure(int frameNumber) {

            string tmpFileName = tmpFilePath + @"tempStructure_" + frameNumber + ".pdb";
            PDBStructureCreator pdbCreator = new PDBStructureCreator(primaryStructure, primaryTrajectory.GetFrame(frameNumber));
            pdbCreator.CreatePDBFile(tmpFileName, true, true, true);

            SecondaryStructure frame = null;

            try {
                StrideAnalysis stride = new StrideAnalysis(strideExePath);
                frame = stride.GetSecondaryStructure(tmpFileName);
            }
            catch (StrideException ex) {
                throw new StrideException(ex.Message);
            }
            finally {
                FileUtil.DeleteFile(tmpFileName);
            }

            return frame;
        }
    }
}
