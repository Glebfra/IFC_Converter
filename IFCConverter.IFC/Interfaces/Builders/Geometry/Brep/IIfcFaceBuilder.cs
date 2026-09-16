using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Brep
{
    public interface IIfcFaceBuilder<out T> : IIfcBuilder
        where T : IIfcFace
    {
        T IfcFace { get; }
        IEnumerable<IIfcFaceBound> Bounds { get; }

        IIfcFaceBound CreateFaceBound(IModel model, IEnumerable<FixedVector<Dim3>> points);
        T CreateFace(IModel model);
    }
}