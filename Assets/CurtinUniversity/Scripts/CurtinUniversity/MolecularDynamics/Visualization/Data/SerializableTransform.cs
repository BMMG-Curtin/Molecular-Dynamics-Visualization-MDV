
using UnityEngine;

namespace CurtinUniversity.MolecularDynamics.Visualization {

    public class SerializableTransform {

        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Vector3 Scale { get; private set; }

        public SerializableTransform(Transform transform) {

            Position = transform.position;
            Rotation = transform.localRotation;
            Scale = transform.localScale;
        }

        public override string ToString() {

            return
                "Position: " + Position + "\n" +
                "Rotation: " + Rotation + "\n" +
                "Scale: " + Scale + "\n";
        }
    }
}