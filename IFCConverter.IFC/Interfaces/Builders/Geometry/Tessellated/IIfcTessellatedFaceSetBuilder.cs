using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Tessellated
{
    public interface IIfcTessellatedFaceSetBuilder<out T> : IIfcTessellatedItemBuilder<T>
        where T : IIfcTessellatedFaceSet
    {
        IIfcCartesianPointList3D Coordinates { get; }

        IIfcCartesianPointList3D CreateCoordinates(IModel model, IEnumerable<FixedVector<Dim3>> coordinates);
    }
}