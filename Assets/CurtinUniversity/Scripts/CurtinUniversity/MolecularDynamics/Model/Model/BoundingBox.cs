using System;
using System.Collections.Generic;

using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Model {

    /// <summary>
    /// Simulation or bounding box for a molecular model
    /// </summary>
    public class BoundingBox {

        public Vector3 Origin { get; private set; }
        public Vector3 Vector1 { get; private set; }
        public Vector3 Vector2 { get; private set; }
        public Vector3 Vector3 { get; private set; }

        public float Width { get; private set; }
        public float Height{ get; private set; }
        public float Depth { get; private set; }
        public Vector3 Centre { get; private set; }

        public BoundingBox() {
            setVectors(0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// Create box based on only max positions on x,y,z planes
        /// </summary>
        public BoundingBox(float maxx, float maxy, float maxz) {
            setVectors(0, maxx, 0, maxy, 0, maxz);
        }

        /// <summary>
        /// Create box based on max and min positions on x,y,z planes
        /// </summary>
        public BoundingBox(float minx, float maxx, float miny, float maxy, float minz, float maxz) {
            setVectors(minx, maxx, miny, maxy, minz, maxz);
        }

        /// <summary>
        /// Create box based on 3 Vectors (typically from gromacs). 
        /// </summary>
        public BoundingBox(float v1x, float v2y, float v3z, float v1y, float v1z, float v2x, float v2z, float v3x, float v3y) {
            setVectors(v1x, v2y, v3z, v1y, v1z, v2x, v2z, v3x, v3y);
        }

        /// <summary>
        /// generate a box from a provided Primary Structure
        /// </summary>
        /// <returns></returns>
        public BoundingBox(PrimaryStructure structure, bool flipZ = false) {

            float minx = 0;
            float maxx = 0;
            float miny = 0;
            float maxy = 0;
            float minz = 0;
            float maxz = 0;

            bool firstAtom = true;
            foreach (Atom atom in structure.Atoms()) {

                if (firstAtom) {

                    minx = atom.Position.x;
                    maxx = atom.Position.x;
                    miny = atom.Position.y;
                    maxy = atom.Position.y;
                    minz = atom.Position.z;
                    maxz = atom.Position.z;
                    firstAtom = false;
                }
                else {

                    if (atom.Position.x < minx)
                        minx = atom.Position.x;
                    if (atom.Position.x > maxx)
                        maxx = atom.Position.x;

                    if (atom.Position.y < miny)
                        miny = atom.Position.y;
                    if (atom.Position.y > maxy)
                        maxy = atom.Position.y;

                    if (atom.Position.z < minz)
                        minz = atom.Position.z;
                    if (atom.Position.z > maxz)
                        maxz = atom.Position.z;
                }
            }

            float edgeBuffer = 0.1f;
            minx -= edgeBuffer;
            maxx += edgeBuffer;
            miny -= edgeBuffer;
            maxy += edgeBuffer;
            minz -= edgeBuffer;
            maxz += edgeBuffer;

            if (flipZ) {
                minz *= -1;
                maxz *= -1;
            }

            setVectors(minx, maxx, miny, maxy, minz, maxz);
        }

        /// <summary>
        /// generate a box from a provided Primary Structure
        /// </summary>
        /// <returns></returns>
        public BoundingBox(PrimaryStructureFrame frame, bool flipZ = false) {

            float minx = 0;
            float maxx = 0;
            float miny = 0;
            float maxy = 0;
            float minz = 0;
            float maxz = 0;

            if (frame.Coords.Length <= 0) {
                return;
            }

            // foreach (KeyValuePair<int, Atom> atom in structure.Atoms()) {
            for(int i = 0; i < frame.AtomCount; i++) {

                float[] coords = frame.GetAtomCoords(i);

                if (i == 0) {

                    minx = coords[0];
                    maxx = coords[0];
                    miny = coords[1];
                    maxy = coords[1];
                    minz = coords[2];
                    maxz = coords[2];
                }
                else {

                    if (coords[0] < minx)
                        minx = coords[0];
                    if (coords[0] > maxx)
                        maxx = coords[0];

                    if (coords[1] < miny)
                        miny = coords[1];
                    if (coords[1] > maxy)
                        maxy = coords[1];

                    if (coords[2] < minz)
                        minz = coords[2];
                    if (coords[2] > maxz)
                        maxz = coords[2];
                }
            }

            float edgeBuffer = 0.1f;
            minx -= edgeBuffer;
            maxx += edgeBuffer;
            miny -= edgeBuffer;
            maxy += edgeBuffer;
            minz -= edgeBuffer;
            maxz += edgeBuffer;

            if (flipZ) {
                minz *= -1;
                maxz *= -1;
            }

            setVectors(minx, maxx, miny, maxy, minz, maxz);
        }

        public override string ToString() {

            String output = "Origin: " + Origin;
            output += "Vector1: " + Vector1;
            output += "Vector2: " + Vector2;
            output += "Vector3: " + Vector3;

            return output;
        }

        public BoundingBox Clone() {

            return new BoundingBox() {

                Origin = this.Origin,
                Vector1 = this.Vector1,
                Vector2 = this.Vector2,
                Vector3 = this.Vector3,
                Width = this.Width,
                Height = this.Height,
                Depth = this.Depth,
                Centre = this.Centre
            };
        }

        /// <summary>
        /// Create box based on max and min positions on x,y,z planes
        /// </summary>
        private void setVectors(float minx, float maxx, float miny, float maxy, float minz, float maxz) {

            Origin = new Vector3(minx, miny, minz);
            Vector1 = new Vector3(maxx, miny, minz);
            Vector2 = new Vector3(minx, maxy, minz);
            Vector3 = new Vector3(minx, miny, maxz);

            setProperties();
        }

        /// <summary>
        /// Create box based on 3 Vectors (typically from gromacs). 
        /// </summary>
        public void setVectors(float v1x, float v2y, float v3z, float v1y, float v1z, float v2x, float v2z, float v3x, float v3y) {

            Origin = new Vector3(0, 0, 0);
            Vector1 = new Vector3(v1x, v1y, v1z);
            Vector2 = new Vector3(v2x, v2y, v2z);
            Vector3 = new Vector3(v3x, v3y, v3z);

            setProperties();
        }

        private void setProperties() { 

            Width = Vector1.x - Origin.x;
            Height = Vector2.y - Origin.y;
            Depth = Vector3.z - Origin.z;
            Centre = new Vector3((Vector1.x + Origin.x) / 2, (Vector2.y + Origin.y) / 2, (Vector3.z + Origin.z) / 2);
        }
    }
}
