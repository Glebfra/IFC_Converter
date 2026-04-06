using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcTessellatedFaceSetBuilder<out T> : IIfcTessellatedItemBuilder<T>
        where T : IIfcTessellatedFaceSet
    {
        public IIfcCartesianPointList3D? Coordinates { get; }

        public IIfcCartesianPointList3D CreateCoordinates(IModel model, IEnumerable<Vector<double>> coordinates);
    }
}