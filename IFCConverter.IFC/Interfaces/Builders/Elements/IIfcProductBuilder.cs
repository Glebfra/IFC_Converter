using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcProductBuilder<out T>
        where T : IIfcProduct
    {
        bool IsCreated { get; }
        T Instance { get; }

        IIfcObjectPlacement ObjectPlacement { get; }
        IIfcGeometry Geometry { get; }
        IIfcProductRepresentation Representation { get; }
        IIfcMaterial Material { get; }
        List<IIfcPropertySet> PropertySets { get; }

        IIfcObjectPlacement CreateObjectPlacement(IModel model, Matrix<double> matrix);

        T CreateInstance(IModel model);
        void AssignPlacement(IIfcObjectPlacement ifcObjectPlacement);
        void AssignGeometry(IIfcGeometry geometry);
        void AssignMaterial(IIfcMaterial material);
    }
}