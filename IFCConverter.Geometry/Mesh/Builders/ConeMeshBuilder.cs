using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public class ConeMeshBuilder : MeshBuilder
    {
        public ConeMeshBuilder(double radius, double height, int radialSegments = 16)
        {
            ValidatePositiveFinite(radius, nameof(radius));
            ValidatePositiveFinite(height, nameof(height));
            
            if (radialSegments < 3)
                throw new ArgumentOutOfRangeException(nameof(radialSegments), "At least 3 radius segments are required.");

            int[] ring = new int[radialSegments];

            for (int i = 0; i < radialSegments; i++)
            {
                double angle = 2.0 * Math.PI * i / radialSegments;

                double x = radius * Math.Cos(angle);
                double y = radius * Math.Sin(angle);

                ring[i] = AddVertex(FixedVector<Dim3>.Builder.Dense(x, y, 0));
            }

            int apex = AddVertex(FixedVector<Dim3>.Builder.Dense(0, 0, height));

            for (int i = 0; i < radialSegments; i++)
            {
                int next = (i + 1) % radialSegments;
                AddTriangle(new Triangle(ring[i], ring[next], apex));
            }
        }
        
        private static void ValidatePositiveFinite(double value, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, "Value must be finite and greater than zero.");
            }
        }
    }
}