using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public sealed class TorusMeshBuilder : MeshBuilder
    {
        public TorusMeshBuilder(double majorRadius, double minorRadius, int majorSegments = 32, int minorSegments = 12)
        {
            ValidatePositiveFinite(majorRadius, nameof(majorRadius));
            ValidatePositiveFinite(minorRadius, nameof(minorRadius));

            if (minorRadius >= majorRadius)
                throw new ArgumentOutOfRangeException(nameof(minorRadius), "minorRadius must be less than majorRadius.");
            if (majorSegments < 3)
                throw new ArgumentOutOfRangeException(nameof(majorSegments), "At least 3 major segments are required.");
            if (minorSegments < 3)
                throw new ArgumentOutOfRangeException(nameof(minorSegments), "At least 3 minor segments are required.");

            int[,] vertices = new int[majorSegments, minorSegments];
            
            for (int i = 0; i < majorSegments; i++)
            {
                double u = 2.0 * Math.PI * i / majorSegments;

                double cosU = Math.Cos(u);
                double sinU = Math.Sin(u);

                for (int j = 0; j < minorSegments; j++)
                {
                    double v = 2.0 * Math.PI * j / minorSegments;

                    double cosV = Math.Cos(v);
                    double sinV = Math.Sin(v);

                    double radialDistance =
                        majorRadius + minorRadius * cosV;

                    double x = radialDistance * cosU;
                    double y = radialDistance * sinU;
                    double z = minorRadius * sinV;

                    vertices[i, j] = AddVertex(FixedVector<Dim3>.Builder.Dense(x, y, z));
                }
            }
            
            for (int i = 0; i < majorSegments; i++)
            {
                int nextI = (i + 1) % majorSegments;
                for (int j = 0; j < minorSegments; j++)
                {
                    int nextJ = (j + 1) % minorSegments;
                    AddRectangle(vertices[i, j], vertices[nextI, j], vertices[i, nextJ], vertices[nextI, nextJ]);
                }
            }
        }
    }
}
