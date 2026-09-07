using System.Collections.Generic;
using IFCConverter.IFC.Extensions;
using IFCConverter.IFC.Interfaces.Geometry.Brep;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.TopologyResource;

namespace IFCConverter.IFC.Builders.Geometry.Brep
{
    public class IfcFaceBuilder<T> : IIfcFaceBuilder<T>
        where T : IIfcFace, IInstantiableEntity
    {
        private readonly List<IIfcFaceBound> _bounds = new List<IIfcFaceBound>();
        public object Instance => IfcFace;
        public T IfcFace { get; private set; }
        public IEnumerable<IIfcFaceBound> Bounds => _bounds;

        public IIfcFaceBound CreateFaceBound(IModel model, IEnumerable<Vector<double>> points)
        {
            IIfcFaceBound faceBound = model.Instances.New<IfcFaceBound>(bound =>
                bound.Bound = points.ToPolyLoop(model)
            );
            _bounds.Add(faceBound);

            return faceBound;
        }

        public virtual T CreateFace(IModel model)
        {
            IfcFace = model.Instances.New<T>(face => face.Bounds.AddRange(_bounds));
            return IfcFace;
        }

        public object Build(IModel model)
        {
            return CreateFace(model);
        }
    }
}