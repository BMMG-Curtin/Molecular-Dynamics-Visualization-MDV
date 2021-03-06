﻿using System;

namespace CurtinUniversity.MolecularDynamics.Model {

    public class FileParseException : Exception {

        public FileParseException() : base() { }
        public FileParseException(string message) : base(message) { }
        public FileParseException(string message, System.Exception inner) : base(message, inner) { }
    }
}