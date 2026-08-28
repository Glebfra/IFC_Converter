using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Brep
{
    public interface IIfcFaceBuilder<out T> : IIfcBuilder
        where T : IIfcFace
    {
        T IfcFace { get; }
        IEnumerable<IIfcFaceBound> Bounds { get; }

        IIfcFaceBound CreateFaceBound(IModel model, IEnumerable<Vector<double>> points);
        T CreateFace(IModel model);
    }
}