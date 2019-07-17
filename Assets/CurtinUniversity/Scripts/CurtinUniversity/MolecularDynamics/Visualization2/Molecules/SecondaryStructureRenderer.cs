using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using CurtinUniversity.MolecularDynamics.Visualization.Utility;
using CurtinUniversity.MolecularDynamics.Model.Definitions;
using CurtinUniversity.MolecularDynamics.Model.Model;

namespace CurtinUniversity.MolecularDynamics.VisualizationP3 {

    public class SecondaryStructureRenderer : MonoBehaviour {

        public GameObject SecondaryStructurePrefab;
        public GameObject ResiduePlanePrefab;
        public GameObject TestCubePrefab;
        public GameObject StructureParent;

        public List<Color32> DebugColors;
        public Color32 ErrorColor;


        public bool BuildingModel { get { return buildingModel; } }

        private PrimaryStructure primaryStructure;
        //private SecondaryStructure defaultSecondaryStructure;
        private Dictionary<string, Mesh> structureCache;

        private bool initialised = false;
        private bool buildingModel = false;

        public void Initialise(PrimaryStructure primaryStructure) {

            this.primaryStructure = primaryStructure;
            structureCache = new Dictionary<string, Mesh>();
            initialised = true;
        }

        public IEnumerator Render(MoleculeRenderSettings settings, PrimaryStructureFrame frame, SecondaryStructure secondaryStructure) {

            if (!initialised) {
                yield break;
            }

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            buildingModel = true;

            yield return null;

            List<GameObject> oldObjects = new List<GameObject>();
            foreach (Transform child in StructureParent.transform) {
                oldObjects.Add(child.gameObject);
            }

            // create mesh
            if (secondaryStructure != null && settings.ShowSecondaryStructure) {
                yield return StartCoroutine(createSecondaryStructure(settings, frame, secondaryStructure));
            }

            // show new mesh, remove old mesh
            foreach (Transform child in StructureParent.transform) {

                if (oldObjects.Contains(child.gameObject)) {
                    child.gameObject.SetActive(false);
                }
                else {
                    child.gameObject.SetActive(true);
                }
            }

            foreach (GameObject oldObject in oldObjects) {
                Destroy(oldObject);
            }

            buildingModel = false;
            watch.Stop();
        }

        private IEnumerator createSecondaryStructure(MoleculeRenderSettings settings, PrimaryStructureFrame frame, SecondaryStructure secondaryStructure) {

            for (int i = 0; i < primaryStructure.Chains().Count; i++) {

                Chain chain = primaryStructure.Chains()[i];

                if (chain.ResidueType != StandardResidue.AminoAcid) {
                    // UnityEngine.Debug.Log("Skipping secondary strucure. Non protein structures not currently supported.");
                    continue;
                }

                if (chain.MainChainResidues.Count < 2) {
                    // UnityEngine.Debug.Log("Skipping secondary strucure. Protein structure doesn't have enough residues for mesh.");
                    continue;
                }

                Mesh structureMesh = BuildStructureMesh(settings, chain, frame, secondaryStructure);

                GameObject structure = (GameObject)Instantiate(SecondaryStructurePrefab);
                structure.GetComponent<MeshFilter>().sharedMesh = structureMesh;

                structure.SetActive(false);
                structure.transform.SetParent(StructureParent.transform, false);

                yield return null;
            }
        }

        private Mesh BuildStructureMesh(MoleculeRenderSettings renderSettings, Chain chain, PrimaryStructureFrame frame, SecondaryStructure secondaryStructure) {

            Mesh structureMesh = null;
            int interpolation = 20;
            int resolution = 6; // should be in config
            float radius = 0.015f;

            List<DynamicMeshNode> nodes = new List<DynamicMeshNode>();

            Vector3 lastPosition = Vector3.zero;
            Vector3 lastNormal = Vector3.zero;
            Vector3 averagedNormal = Vector3.zero;
            SecondaryStructureType lastType = SecondaryStructureType.Coil;

            HashSet<string> customDisplayResidues = null;
            if (renderSettings.CustomDisplayResidues != null && renderSettings.CustomDisplayResidues.Count > 0) {
                customDisplayResidues = renderSettings.CustomDisplayResidues;
            }

            Dictionary<string, Visualization.ResidueDisplayOptions> residueOptions = null;
            if (customDisplayResidues != null) {
                residueOptions = renderSettings.ResidueOptions;
            }

            for (int i = 0; i < chain.MainChainResidues.Count; i++) {

                DynamicMeshNode node = new DynamicMeshNode();

                Residue residue = chain.MainChainResidues[i];

                // check if residue mainchain information is complete. Ignore if not
                if (residue.AlphaCarbon == null || residue.CarbonylCarbon == null || residue.CarbonylOxygen == null) {
                    continue;
                }

                // set position
                Atom atom = residue.AlphaCarbon;

                // if no frame number use the base structure coordinates.
                Vector3 position;
                if (frame == null) {
                    position = new Vector3(atom.Position.x, atom.Position.y, atom.Position.z);
                }
                else {
                    position = new Vector3(frame.Coords[atom.Index * 3], frame.Coords[(atom.Index * 3) + 1], frame.Coords[(atom.Index * 3) + 2]);
                }

                if (Settings.FlipZCoordinates) {
                    position.z = position.z * -1;
                }

                node.Position = position;

                SecondaryStructureInfomation structureInformation = secondaryStructure.GetStructureInformation(residue.Index);

                Residue nextResidue = null;
                if (i + 1 < chain.MainChainResidues.Count) {
                    nextResidue = chain.MainChainResidues[i + 1];
                }

                SecondaryStructureInfomation nextResidueStructureInfo = null;
                if (nextResidue != null) {
                    nextResidueStructureInfo = secondaryStructure.GetStructureInformation(nextResidue.Index);
                }

                // store the node type
                if (structureInformation != null) {

                    if (renderSettings.ShowHelices &&
                        (structureInformation.type == SecondaryStructureType.ThreeHelix ||
                        structureInformation.type == SecondaryStructureType.AlphaHelix ||
                        structureInformation.type == SecondaryStructureType.FiveHelix)) {

                        node.Type = DynamicMeshNodeType.SpiralRibbon;
                    }
                    else if (renderSettings.ShowSheets &&
                        structureInformation.type == SecondaryStructureType.BetaSheet) {

                        if (nextResidue == null || (nextResidueStructureInfo != null && nextResidueStructureInfo.type != SecondaryStructureType.BetaSheet)) {
                            node.Type = DynamicMeshNodeType.RibbonHead;
                        }
                        else {
                            node.Type = DynamicMeshNodeType.Ribbon;
                        }
                    }
                    else if (renderSettings.ShowTurns &&
                        structureInformation.type == SecondaryStructureType.Turn) {

                        node.Type = DynamicMeshNodeType.LargeTube;
                    }
                    else {
                        node.Type = DynamicMeshNodeType.Tube;
                    }


                    // calculate and store the node color

                    bool foundColour = false;

                    if (customDisplayResidues != null && customDisplayResidues.Contains(residue.Name)) {

                        Visualization.ResidueDisplayOptions displayOptions = residueOptions[residue.Name];

                        if (displayOptions != null && displayOptions.ColourSecondaryStructure) {

                            node.VertexColor = displayOptions.CustomColour;
                            foundColour = true;
                        }
                    }

                    if (foundColour == false) {

                        switch (structureInformation.type) {

                            case SecondaryStructureType.ThreeHelix:
                                node.VertexColor = Settings.ThreeHelixColour;
                                break;

                            case SecondaryStructureType.AlphaHelix:
                                node.VertexColor = Settings.AlphaHelixColour;
                                break;

                            case SecondaryStructureType.FiveHelix:
                                node.VertexColor = Settings.FiveHelixColour;
                                break;

                            case SecondaryStructureType.Turn:
                                node.VertexColor = Settings.TurnColour;
                                break;

                            case SecondaryStructureType.BetaSheet:
                                node.VertexColor = Settings.BetaSheetColour;
                                break;

                            case SecondaryStructureType.BetaBridge:
                                node.VertexColor = Settings.BetaBridgeColour;
                                break;

                            case SecondaryStructureType.Bend:
                                node.VertexColor = Settings.BendColour;
                                break;

                            case SecondaryStructureType.Coil:
                                node.VertexColor = Settings.CoilColour;
                                break;

                            default:
                                node.VertexColor = ErrorColor;
                                break;
                        }
                    }
                }
                else {
                    UnityEngine.Debug.Log("*** Structure info null: assigning defaults");
                    node.Type = DynamicMeshNodeType.Tube;
                    node.VertexColor = ErrorColor;
                }

                // determine the node rotation 
                // calculate the normal from the peptide plane and store as the node rotation
                Vector3 vertexA = residue.AlphaCarbon.Position;
                Vector3 vertexB = residue.CarbonylCarbon.Position;
                Vector3 vertexC = residue.CarbonylOxygen.Position;

                if (Settings.FlipZCoordinates) {
                    vertexA.z *= -1;
                    vertexB.z *= -1;
                    vertexC.z *= -1;
                }

                //// create a triangle to show the peptide plane on the model for debugging purposes
                //GameObject residuePlane = createTriangle(vertexA, vertexB, vertexC);
                //residuePlane.name = "ResiduePlane";
                //AddMeshToModel(residuePlane, StructureParent);

                Vector3 direction = Vector3.Cross(vertexB - vertexA, vertexC - vertexA);
                Vector3 normal = Vector3.Normalize(direction);

                if (structureInformation != null && structureInformation.type == SecondaryStructureType.BetaSheet || lastType == SecondaryStructureType.BetaSheet) {
                    if (Vector3.Dot(normal, lastNormal) < 0) {
                        normal *= -1;
                    }

                    if (lastType != SecondaryStructureType.BetaSheet) {
                        averagedNormal = normal;
                    }
                    else {
                        averagedNormal += normal;
                        averagedNormal.Normalize();
                        normal = averagedNormal;
                    }
                }

                node.Rotation = normal;

                lastNormal = normal;
                if (structureInformation != null) {
                    lastType = structureInformation.type;
                }
                else {
                    lastType = SecondaryStructureType.Coil;
                }

                nodes.Add(node);
            }


            if (Settings.SmoothRibbons) {
                nodes = smoothMeshNodes(nodes);
            }

            //// draw debug line from node points along rotation vector
            //for (int q = 0; q < nodePositions.Count; q++) {

            //    Vector3 fromPosition = nodePositions[q];
            //    Vector3 toPosition = fromPosition + nodeRotations[q] * 0.3f;
            //    GameObject line = createLine(fromPosition, toPosition, Color.white, Color.red);
            //    AddMeshToModel(line, StructureParent);
            //}

            List<Vector3> nodePositions = new List<Vector3>();
            foreach (DynamicMeshNode node in nodes) {
                nodePositions.Add(node.Position);
            }
            List<DynamicMeshNode> splineNodes = new List<DynamicMeshNode>();
            IEnumerable splinePoints = Interpolate.NewCatmullRom(nodePositions.ToArray(), interpolation, false);
            int j = 0;

            foreach (Vector3 position in splinePoints) {

                int nodeIndex = j / (interpolation + 1);
                int splinePointsSinceLastNode = j % (interpolation + 1);

                DynamicMeshNode node = new DynamicMeshNode();
                node.Position = position;

                //int colorIndex = nodeIndex % DebugColors.Count;
                //node.VertexColor = DebugColors[colorIndex];

                node.VertexColor = nodes[nodeIndex].VertexColor;
                node.Type = nodes[nodeIndex].Type;

                // set the mesh rotations for the node
                // dont do rotations on tube structures
                switch (node.Type) {

                    case DynamicMeshNodeType.Ribbon:
                    case DynamicMeshNodeType.RibbonHead:
                    case DynamicMeshNodeType.SpiralRibbon:

                        if (nodeIndex < nodes.Count - 1) {
                            float percentThroughNode = (float)splinePointsSinceLastNode / ((float)interpolation + 1f);
                            node.Rotation = Vector3.Lerp((Vector3)nodes[nodeIndex].Rotation, (Vector3)nodes[nodeIndex + 1].Rotation, percentThroughNode);
                        }
                        else { // last node
                            node.Rotation = (Vector3)nodes[nodeIndex].Rotation;
                        }

                        break;
                }

                splineNodes.Add(node);
                j++;
            }

            DynamicMesh dynamicMesh = new DynamicMesh(splineNodes, radius, resolution, interpolation + 1);
            structureMesh = dynamicMesh.Build(Settings.DebugFlag);

            return structureMesh;
        }

        private List<DynamicMeshNode> smoothMeshNodes(List<DynamicMeshNode> nodes) {

            // average the beta sheet ribbon positions

            // copy nodes into new List
            List<DynamicMeshNode> smoothedNodes = new List<DynamicMeshNode>();
            foreach (DynamicMeshNode node in nodes) {
                smoothedNodes.Add(node.Clone());
            }

            // adjust new list ribbons node positions 
            for (int i = 0; i < smoothedNodes.Count; i++) {

                if (smoothedNodes[i].Type == DynamicMeshNodeType.Ribbon) {

                    if (i == 0 || smoothedNodes[i - 1].Type != DynamicMeshNodeType.Ribbon) {
                        smoothedNodes.Insert(i, smoothedNodes[i].Clone());
                    }
                }

                if (smoothedNodes[i].Type == DynamicMeshNodeType.RibbonHead) {
                    smoothedNodes.RemoveAt(i - 1);
                }
            }

            // average ribbon node positions
            int ribbonNodeCount = 0;
            for (int i = 0; i < smoothedNodes.Count; i++) {

                if (smoothedNodes[i].Type == DynamicMeshNodeType.Ribbon) {

                    if (i == 0 || smoothedNodes[i - 1].Type != DynamicMeshNodeType.Ribbon) {
                        ribbonNodeCount = 1;
                    }
                    else {
                        ribbonNodeCount++;
                    }

                    if (ribbonNodeCount > 1) {
                        smoothedNodes[i].Position = ((smoothedNodes[i + 1].Position - smoothedNodes[i].Position) / 2) + smoothedNodes[i].Position;
                    }

                    // retrospectively fix bad average due to insufficient data
                    if (ribbonNodeCount == 3) {
                        smoothedNodes[i - 1].Position = ((smoothedNodes[i].Position - smoothedNodes[i - 2].Position) / 2) + smoothedNodes[i - 2].Position;
                    }
                }
            }


            // orient the helix ribbons to face centre of helix
            for (int i = 1; i < smoothedNodes.Count - 1; i++) {

                if (smoothedNodes[i].Type == DynamicMeshNodeType.SpiralRibbon || smoothedNodes[i - 1].Type == DynamicMeshNodeType.SpiralRibbon) {

                    Vector3 vectorToNextNode = Vector3.Normalize(smoothedNodes[i].Position - smoothedNodes[i + 1].Position);
                    Vector3 vectorToPreviousNode = Vector3.Normalize(smoothedNodes[i].Position - smoothedNodes[i - 1].Position);
                    Vector3 averageVector = Vector3.Normalize((vectorToPreviousNode - vectorToNextNode) / 2 + vectorToNextNode);
                    smoothedNodes[i].Rotation = averageVector * -1;
                }
            }

            //if (smoothedNodes.Count == nodes.Count) {
            return smoothedNodes;
            //}
            //else {
            //    UnityEngine.Debug.Log("*** error in smoothing. New node count != old node count");
            //    return nodes;
            //}
        }

        // used for showing normals to debug mesh rendering
        private GameObject createLine(Vector3 start, Vector3 end, Color startColor, Color endColor) {

            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();

            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.useWorldSpace = false;
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.startColor = startColor;
            lr.endColor = endColor;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.01f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            return myLine;
        }

        // used for showing peptide planes to debug mesh rendering
        private GameObject createTriangle(Vector3 pos1, Vector3 pos2, Vector3 pos3) {

            Vector3[] vertices = new Vector3[3];
            vertices[0] = pos1;
            vertices[1] = pos2;
            vertices[2] = pos3;

            int[] triangles = new int[6];
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 1;

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            GameObject prefab = (GameObject)Instantiate(ResiduePlanePrefab, Vector3.zero, Quaternion.identity);
            MeshFilter meshFilter = prefab.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            return prefab;
        }
    }
}

