﻿using ChunkyImageLib;
using PixiEditor.ChangeableDocument.Changeables.Interfaces;

namespace PixiEditor.ChangeableDocument.Changeables
{
    internal abstract class StructureMember : IChangeable, IReadOnlyStructureMember, IDisposable
    {
        public float Opacity { get; set; } = 1f;
        public bool IsVisible { get; set; } = true;
        public string Name { get; set; } = "Unnamed";
        public Guid GuidValue { get; init; }
        public ChunkyImage? Mask { get; set; } = null;
        public IReadOnlyChunkyImage? ReadOnlyMask => Mask;

        internal abstract StructureMember Clone();
        public abstract void Dispose();
    }
}
