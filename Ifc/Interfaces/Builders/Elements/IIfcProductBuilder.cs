using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcProductBuilder<out T>
        where T : IIfcProduct
    {
        public bool IsCreated { get; }
        public T? Instance { get; }

        public IIfcObjectPlacement? ObjectPlacement { get; }
        public IIfcGeometry? Geometry { get; }
        public IIfcProductRepresentation? Representation { get; }
        public IIfcMaterial? Material { get; }
        public List<IIfcPropertySet> PropertySets { get; }

        public IIfcObjectPlacement CreateObjectPlacement(IModel model, Matrix<double> matrix);

        public T CreateInstance(IModel model);
        public void AssignPlacement(IIfcObjectPlacement ifcObjectPlacement);
        public void AssignGeometry(IIfcGeometry geometry);
        public void AssignMaterial(IIfcMaterial material);
    }
}