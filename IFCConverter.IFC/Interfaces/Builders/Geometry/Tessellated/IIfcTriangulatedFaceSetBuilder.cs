using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces.Geometry.Tessellated
{
    public interface IIfcTriangulatedFaceSetBuilder<out T> : IIfcTessellatedFaceSetBuilder<T>
        where T : IIfcTriangulatedFaceSet
    {
        IItemSet<IItemSet<IfcPositiveInteger>> CoordIndex { get; }
        IItemSet<IItemSet<IfcParameterValue>> Normals { get; }

        void AssignTriangleIndices(IEnumerable<IEnumerable<int>> coordIndex);
        void AssignNormals(IEnumerable<Vector<double>> normals);
    }
}