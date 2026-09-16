using System;
using System.Collections.Generic;

namespace IFCConverter.Geometry.MeshResolvers
{
    public struct ClosedClippedConeMeshProperties
    {

    }

    public sealed class ClosedClippedConeMeshResolver
    {
        private readonly PlanarComponentFinder _finder;
        private readonly double _normalTolerance;
        private readonly double _planeTolerance;

        public ClosedClippedConeMeshResolver(double normalTolerance = 1e-6, double planeTolerance = 1e-4)
        {
            _normalTolerance = normalTolerance;
            _planeTolerance = planeTolerance;
            _finder = new PlanarComponentFinder(normalTolerance);
        }

        public ClosedClippedConeMeshProperties Resolve(IMesh mesh)
        {
            IReadOnlyList<PlanarComponent> components = _finder.Find(mesh);
            throw new NotImplementedException();
        }
    }
}