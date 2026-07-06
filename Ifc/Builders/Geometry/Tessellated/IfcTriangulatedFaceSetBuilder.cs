using System;
using System.Collections.Generic;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Geometry.Tessellated
{
    public class IfcTriangulatedFaceSetBuilder<T> : IfcTessellatedFaceSetBuilder<T>, IIfcTriangulatedFaceSetBuilder<T>
        where T : IIfcTriangulatedFaceSet, IInstantiableEntity
    {
        private IEnumerable<IEnumerable<int>>? _coordIndex;
        private IEnumerable<IEnumerable<double>>? _normals;
        public IItemSet<IItemSet<IfcPositiveInteger>>? CoordIndex { get; }
        public IItemSet<IItemSet<IfcParameterValue>>? Normals { get; }

        public void AssignTriangleIndices(IEnumerable<IEnumerable<int>> coordIndex)
        {
            _coordIndex = coordIndex;
        }

        public void AssignNormals(IEnumerable<Vector<double>> normals)
        {
            _normals = normals;
        }

        public override T CreateTessellatedItem(IModel model)
        {
            T instance = base.CreateTessellatedItem(model);
            int index = 0;
            if (_coordIndex == null)
                throw new InvalidOperationException(
                    $"CoordIndex is not assigned. Call {nameof(AssignTriangleIndices)} before {nameof(CreateTessellatedItem)}");
            foreach (IEnumerable<int> coordIndexes in _coordIndex)
            {
                IItemSet<IfcPositiveInteger> itemSet = instance.CoordIndex.GetAt(index++);
                foreach (int coordIndex in coordIndexes)
                    itemSet.Add(coordIndex);
            }

            index = 0;
            if (_normals == null)
                throw new InvalidOperationException(
                    $"Normals is not assigned. Call {nameof(AssignNormals)} before {nameof(CreateTessellatedItem)}");
            foreach (IEnumerable<double> normalVector in _normals)
            {
                IItemSet<IfcParameterValue> itemSet = instance.Normals.GetAt(index++);
                foreach (double coord in normalVector)
                    itemSet.Add(coord);
            }

            return instance;
        }
    }
}