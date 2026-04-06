using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Interfaces
{
    public interface IIfcTriangulatedFaceSetBuilder<out T> : IIfcTessellatedFaceSetBuilder<T>
        where T : IIfcTriangulatedFaceSet
    {
        public IItemSet<IItemSet<IfcPositiveInteger>>? CoordIndex { get; }
        public IItemSet<IItemSet<IfcParameterValue>>? Normals { get; }

        public void AssignTriangleIndices(IEnumerable<IEnumerable<int>> coordIndex);
        public void AssignNormals(IEnumerable<Vector<double>> normals);
    }
}