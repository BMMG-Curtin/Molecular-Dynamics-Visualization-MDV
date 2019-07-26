using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class DynamicMeshNode {

        public DynamicMeshNode() {

            Type = DynamicMeshNodeType.Tube;
            Position = Vector3.zero;
            Rotation = null;
            VertexColor = Color.white;
        }

        public DynamicMeshNodeType Type;
        public Vector3 Position { get; set; }
        public Vector3? Rotation { get; set; } // vector for orientation of non circular mesh types. If null then uses a default rotation determined by the node path 
        public Color32 VertexColor { get; set; }

        public DynamicMeshNode Clone() {

            DynamicMeshNode newNode = new DynamicMeshNode();
            newNode.Position = this.Position;
            newNode.Rotation = this.Rotation;
            newNode.VertexColor = this.VertexColor;
            newNode.Type = this.Type;
            return newNode;
        }
    }
}

