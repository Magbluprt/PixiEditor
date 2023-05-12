﻿using ChunkyImageLib.DataHolders;
using PixiEditor.DrawingApi.Core.Numerics;
using PixiEditor.DrawingApi.Core.Surface;
using PixiEditor.DrawingApi.Core.Surface.PaintImpl;

namespace ChunkyImageLib.Operations;

internal class ApplyMaskOperation : IDrawOperation
{
    private ChunkyImage mask;
    private Paint clippingPaint = new Paint() { BlendMode = BlendMode.DstIn };

    public bool IgnoreEmptyChunks => true;

    public ApplyMaskOperation(ChunkyImage maskToApply)
    {
        mask = maskToApply;
    }

    public AffectedArea FindAffectedArea(VecI imageSize)
    {
        return new AffectedArea(mask.FindCommittedChunks());
    }
    
    public void DrawOnChunk(Chunk targetChunk, VecI chunkPos, ChunkyImage caller)
    {
        mask.DrawCommittedChunkOn(chunkPos, targetChunk.Resolution, targetChunk.Surface.DrawingSurface, VecI.Zero, clippingPaint);
    }

    public void Dispose()
    {
        clippingPaint.Dispose();
    }
}
