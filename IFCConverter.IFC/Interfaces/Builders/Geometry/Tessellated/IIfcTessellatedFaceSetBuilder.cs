using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Tessellated
{
    public interface IIfcTessellatedFaceSetBuilder<out T> : IIfcTessellatedItemBuilder<T>
        where T : IIfcTessellatedFaceSet
    {
        IIfcCartesianPointList3D Coordinates { get; }

        IIfcCartesianPointList3D CreateCoordinates(IModel model, IEnumerable<Vector<double>> coordinates);
    }
}