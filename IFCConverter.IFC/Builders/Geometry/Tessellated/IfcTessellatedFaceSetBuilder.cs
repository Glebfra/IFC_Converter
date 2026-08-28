using System.Collections.Generic;
using IFCConverter.IFC.Extensions;
using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Geometry.Tessellated
{
    public class IfcTessellatedFaceSetBuilder<T> : IfcTessellatedItemBuilder<T>, IIfcTessellatedFaceSetBuilder<T>
        where T : IIfcTessellatedFaceSet, IInstantiableEntity
    {
        public IIfcCartesianPointList3D Coordinates { get; private set; }

        public IIfcCartesianPointList3D CreateCoordinates(IModel model, IEnumerable<Vector<double>> coordinates)
        {
            Coordinates = coordinates.ToCartesianPointList3D(model);
            return Coordinates;
        }

        public override T CreateTessellatedItem(IModel model)
        {
            T instance = base.CreateTessellatedItem(model);
            instance.Coordinates = Coordinates;
            return instance;
        }
    }
}