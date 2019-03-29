using System;

namespace CurtinUniversity.MolecularDynamics.Model.Analysis {

    public class StrideException : Exception {

            public StrideException() : base() { }
            public StrideException(string message) : base(message) { }
            public StrideException(string message, System.Exception inner) : base(message, inner) { }
    }
}
