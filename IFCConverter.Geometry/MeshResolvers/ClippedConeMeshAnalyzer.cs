using System;

namespace IFCConverter.Geometry.MeshResolvers
{
    public sealed class ClippedConeMeshAnalyzer
    {
        private readonly PlanarComponentFinder _planarComponentFinder;

        public ClippedConeMeshAnalyzer(double normalTolerance = 1e-6)
        {
            _planarComponentFinder = new PlanarComponentFinder(normalTolerance);
        }

        public ClippedConeGeometry Analyze(IMesh mesh)
        {
            throw new NullReferenceException();
        }

        private static void ValidateMesh(IMesh mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));
        }
    }
}