using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Mesh.Builders
{
    public sealed class SphereMeshBuilder : MeshBuilder
    {
        public SphereMeshBuilder(double radius, int longitudeSegments = 8, int latitudeSegments = 8)
        {
            ValidatePositiveFinite(radius, nameof(radius));

            int southPole = AddVertex(FixedVector<Dim3>.Builder.Dense(0, 0, -radius));
            int[,] rings = new int[latitudeSegments - 1, longitudeSegments];

            for (int latitude = 1; latitude < latitudeSegments; latitude++)
            {
                double phi = -Math.PI / 2.0 + Math.PI * latitude / latitudeSegments;

                double cosPhi = Math.Cos(phi);
                double sinPhi = Math.Sin(phi);

                for (int longitude = 0; longitude < longitudeSegments; longitude++)
                {
                    double theta = 2.0 * Math.PI * longitude / longitudeSegments;

                    double x = radius * cosPhi * Math.Cos(theta);
                    double y = radius * cosPhi * Math.Sin(theta);
                    double z = radius * sinPhi;

                    rings[latitude - 1, longitude] = AddVertex(FixedVector<Dim3>.Builder.Dense(x, y, z));
                }
            }

            int northPole = AddVertex(FixedVector<Dim3>.Builder.Dense(0, 0, radius));
            
            // Южная шапка.
            for (int longitude = 0; longitude < longitudeSegments; longitude++)
            {
                int next = (longitude + 1) % longitudeSegments;
                AddTriangle(new Triangle(southPole, rings[0, next], rings[0, longitude]));
            }

            // Боковая поверхность между кольцами.
            for (int latitude = 0; latitude < latitudeSegments - 2; latitude++)
            {
                for (int longitude = 0; longitude < longitudeSegments; longitude++)
                {
                    int next = (longitude + 1) % longitudeSegments;

                    int lowerCurrent = rings[latitude, longitude];
                    int lowerNext = rings[latitude, next];
                    int upperCurrent = rings[latitude + 1, longitude];
                    int upperNext = rings[latitude + 1, next];

                    AddTriangle(new Triangle(lowerCurrent, lowerNext, upperCurrent));
                    AddTriangle(new Triangle(lowerNext, upperNext, upperCurrent));
                }
            }

            // Северная шапка.
            int lastRing = latitudeSegments - 2;
            for (int longitude = 0; longitude < longitudeSegments; longitude++)
            {
                int next = (longitude + 1) % longitudeSegments;
                AddTriangle(new Triangle(northPole, rings[lastRing, longitude], rings[lastRing, next]));
            }
        }
        
        private static void ValidateRadius(double radius)
        {
            if (double.IsNaN(radius) || double.IsInfinity(radius) || radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be finite and greater than zero.");
            }
        }
    }
}