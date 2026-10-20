
using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public sealed class ClippedConeMeshBuilder : MeshBuilder
    {
        public FixedVector<Dim3>[] BottomRing { get; }
        public FixedVector<Dim3>[] TopRing { get; }
        
        public int[] BottomRingIndices { get; }
        public int[] TopRingIndices { get; }
        
        public ClippedConeMeshBuilder(double minRadius, double maxRadius, double height, int radialSegments = 16)
        {
            ValidatePositiveFinite(minRadius, nameof(minRadius));
            ValidatePositiveFinite(maxRadius, nameof(maxRadius));
            ValidatePositiveFinite(height, nameof(height));

            if (minRadius > maxRadius)
            {
                throw new ArgumentException("minRadius must be less than or equal to maxRadius.", nameof(minRadius));
            }

            if (radialSegments < 3)
            {
                throw new ArgumentOutOfRangeException(nameof(radialSegments), "At least 3 radius segments are required.");
            }

            BottomRing = new FixedVector<Dim3>[radialSegments];
            TopRing = new FixedVector<Dim3>[radialSegments];
            
            BottomRingIndices = new int[radialSegments];
            TopRingIndices = new int[radialSegments];

            for (int i = 0; i < radialSegments; i++)
            {
                double angle = 2.0 * Math.PI * i / radialSegments;

                double cos = Math.Cos(angle);
                double sin = Math.Sin(angle);

                BottomRing[i] = FixedVector<Dim3>.Builder.Dense(maxRadius * cos, maxRadius * sin, 0);
                TopRing[i] = FixedVector<Dim3>.Builder.Dense(minRadius * cos, minRadius * sin, height);
                
                BottomRingIndices[i] = AddVertex(BottomRing[i]);
                TopRingIndices[i] = AddVertex(TopRing[i]);
            }

            for (int i = 0; i < radialSegments; i++)
            {
                int next = (i + 1) % radialSegments;
                AddRectangle(BottomRingIndices[i], BottomRingIndices[next], TopRingIndices[i], TopRingIndices[next]);
            }
        }
    }
}
