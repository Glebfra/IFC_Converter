using System.Collections.Generic;
using IFCConverter.IFC.Exceptions;
using IFCConverter.IFC.Extensions;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;

namespace IFCConverter.IFC.Builders.Elements
{
    public class IfcProductBuilder<T> : IIfcProductBuilder<T>
        where T : IfcProduct, IInstantiableEntity
    {
        public IfcLabel? ObjectType { get; private set; }
        public bool IsCreated { get; private set; }
        public T Instance { get; private set; }
        public IIfcObjectPlacement ObjectPlacement { get; private set; }
        public IIfcGeometry Geometry { get; private set; }
        public IIfcProductRepresentation Representation { get; private set; }
        public IIfcMaterial Material { get; private set; }

        public List<IIfcPropertySet> PropertySets { get; } = new List<IIfcPropertySet>();

        public IIfcObjectPlacement CreateObjectPlacement(IModel model, FixedMatrix<Dim4> matrix)
        {
            ObjectPlacement = matrix.ToIfcObjectPlacement(model);
            return ObjectPlacement;
        }

        public virtual T CreateInstance(IModel model)
        {
            if (Geometry != null)
            {
                IIfcShapeRepresentation shapeRepresentation = Geometry.CreateShapeRepresentation(model);
                Representation = Geometry.CreateProductDefinitionShape(model, shapeRepresentation);
            }

            string transactionName = $"{nameof(IfcProductBuilder<T>)}: {nameof(CreateInstance)}";

            Instance = model.Instances.New<T>(product =>
            {
                if (ObjectPlacement != null)
                    product.ObjectPlacement = (IfcObjectPlacement)ObjectPlacement;
                if (Representation != null)
                    product.Representation = (IfcProductRepresentation)Representation;
                product.ObjectType = ObjectType;
            });

            if (Material != null)
            {
                RelateMaterial(model);
            }

            foreach (IIfcPropertySet ifcPropertySet in PropertySets)
            {
                model.Instances.New<IfcRelDefinesByProperties>(properties =>
                {
                    properties.Name = ifcPropertySet.Name;
                    properties.RelatedObjects.Add(Instance);
                    properties.RelatingPropertyDefinition = ifcPropertySet;
                });
            }

            IsCreated = true;

            return Instance;
        }

        public void AssignPlacement(IIfcObjectPlacement ifcObjectPlacement)
        {
            if (IsCreated)
                throw new IfcEntityCreatedException(
                    $"{GetType().Name} entity already created. Cannot call {nameof(AssignPlacement)} method!");
            ObjectPlacement = ifcObjectPlacement;
        }

        public void AssignGeometry(IIfcGeometry geometry)
        {
            if (IsCreated)
                throw new IfcEntityCreatedException(
                    $"{GetType().Name} entity already created. Cannot call {nameof(AssignGeometry)} method!");
            Geometry = geometry;
        }

        public void AssignMaterial(IIfcMaterial material)
        {
            if (IsCreated)
                throw new IfcEntityCreatedException(
                    $"{GetType().Name} entity already create. Cannot call {nameof(AssignMaterial)} method!");
            Material = material;
        }

        public void AssignObjectType(string type)
        {
            ObjectType = type;
        }

        private void RelateMaterial(IModel model)
        {
            model.Instances.New<IfcRelAssociatesMaterial>(material =>
            {
                material.RelatingMaterial = Material;
                material.RelatedObjects.Add(Instance);
            });
        }
    }
}