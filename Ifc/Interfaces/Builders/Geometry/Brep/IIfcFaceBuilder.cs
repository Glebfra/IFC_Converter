using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcFaceBuilder<out T> : IIfcBuilder
        where T : IIfcFace
    {
        public T? IfcFace { get; }
        public IEnumerable<IIfcFaceBound> Bounds { get; }

        public IIfcFaceBound CreateFaceBound(IModel model, IEnumerable<Vector<double>> points);
        public T CreateFace(IModel model);
    }
}