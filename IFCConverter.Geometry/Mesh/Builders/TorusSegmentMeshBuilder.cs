using System;
using System.Linq;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public sealed class TorusSegmentMeshBuilder : MeshBuilder
    {
        public int[] StartCapIndices { get; }
        public int[] EndCapIndices { get; }
        
        public TorusSegmentMeshBuilder(
            double majorRadius, double minorRadius,
            double startAngle, double endAngle,
            int majorSegments = 16, int minorSegments = 12)
        {
            ValidatePositiveFinite(majorRadius, nameof(majorRadius));
            ValidatePositiveFinite(minorRadius, nameof(minorRadius));
            ValidateFinite(startAngle, nameof(startAngle));
            ValidateFinite(endAngle, nameof(endAngle));

            if (minorRadius >= majorRadius)
                throw new ArgumentOutOfRangeException(nameof(minorRadius), "minorRadius must be less than majorRadius.");
            if (endAngle <= startAngle)
                throw new ArgumentException("endAngle must be greater than startAngle.");
            if (endAngle - startAngle > 2.0 * Math.PI)
                throw new ArgumentOutOfRangeException(nameof(endAngle), "A torus segment cannot span more than 2 PI radians.");
            if (majorSegments < 3)
                throw new ArgumentOutOfRangeException(nameof(majorSegments), "At least 3 major segment is required.");
            if (minorSegments < 3)
                throw new ArgumentOutOfRangeException(nameof(minorSegments), "At least 3 minor segments are required.");

            int[,] vertices = new int[majorSegments + 1, minorSegments];
            
            for (int i = 0; i <= majorSegments; i++)
            {
                double u = startAngle + (endAngle - startAngle) * i / majorSegments;

                double cosU = Math.Cos(u);
                double sinU = Math.Sin(u);

                for (int j = 0; j < minorSegments; j++)
                {
                    double v = 2.0 * Math.PI * j / minorSegments;

                    double cosV = Math.Cos(v);
                    double sinV = Math.Sin(v);

                    double radialDistance = majorRadius + minorRadius * cosV;

                    double x = radialDistance * cosU;
                    double y = radialDistance * sinU;
                    double z = minorRadius * sinV;

                    vertices[i, j] = AddVertex(FixedVector<Dim3>.Builder.Dense(x, y, z));
                }
            }
            
            StartCapIndices = Enumerable.Range(0, minorSegments)
                .Select(j => vertices[0, j])
                .ToArray();

            EndCapIndices = Enumerable.Range(0, minorSegments)
                .Select(j => vertices[majorSegments, j])
                .ToArray();
            
            for (int i = 0; i < majorSegments; i++)
            {
                int nextI = i + 1;
                for (int j = 0; j < minorSegments; j++)
                {
                    int nextJ = (j + 1) % minorSegments;
                    AddRectangle(vertices[i, j], vertices[nextI, j], vertices[i, nextJ], vertices[nextI, nextJ]);
                }
            }
        }
    }
}
