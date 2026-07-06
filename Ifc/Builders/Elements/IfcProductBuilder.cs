using System.Collections.Generic;
using Ifc.Exceptions;
using Ifc.Extensions;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;

namespace Ifc.Builders.Elements
{
    public class IfcProductBuilder<T> : IIfcProductBuilder<T>
        where T : IfcProduct, IInstantiableEntity
    {
        private readonly Logger _logger = Logger.GetInstance();
        public bool IsCreated { get; private set; }
        public T? Instance { get; private set; }
        public IIfcObjectPlacement? ObjectPlacement { get; private set; }
        public IIfcGeometry? Geometry { get; private set; }
        public IIfcProductRepresentation? Representation { get; private set; }
        public IIfcMaterial? Material { get; private set; }

        public List<IIfcPropertySet> PropertySets { get; } = new();

        public IIfcObjectPlacement CreateObjectPlacement(IModel model, Matrix<double> matrix)
        {
            const string transactionName = $"{nameof(IfcProductBuilder<T>)}: {nameof(CreateObjectPlacement)}";
            _logger.Info($"Begin transaction: {transactionName}");

            ObjectPlacement = matrix.ToIfcObjectPlacement(model);
            _logger.Info($"Created object placement with matrix: {matrix.ToRowString()}");

            return ObjectPlacement;
        }

        public virtual T CreateInstance(IModel model)
        {
            if (Geometry != null)
            {
                IIfcShapeRepresentation shapeRepresentation = Geometry.CreateShapeRepresentation(model);
                Representation = Geometry.CreateProductDefinitionShape(model, shapeRepresentation);
            }

            const string transactionName = $"{nameof(IfcProductBuilder<T>)}: {nameof(CreateInstance)}";
            _logger.Info($"Begin transaction: {transactionName}");

            Instance = model.Instances.New<T>(product =>
            {
                if (ObjectPlacement != null)
                    product.ObjectPlacement = (IfcObjectPlacement)ObjectPlacement;
                if (Representation != null)
                    product.Representation = (IfcProductRepresentation)Representation;
            });
            _logger.Info($"Created instance with type: {typeof(T).Name}");

            if (Material != null)
            {
                RelateMaterial(model);
                _logger.Info(
                    $"Added relation between material with id: {Material.EntityLabel} and product instance with id: {Instance.EntityLabel}");
            }

            foreach (IIfcPropertySet ifcPropertySet in PropertySets)
            {
                model.Instances.New<IfcRelDefinesByProperties>(properties =>
                {
                    properties.Name = ifcPropertySet.Name;
                    properties.RelatedObjects.Add(Instance);
                    properties.RelatingPropertyDefinition = ifcPropertySet;
                });
                _logger.Info(
                    $"Added property set with name: {ifcPropertySet.Name} to product instance with id: {Instance.EntityLabel}");
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
            _logger.Info($"Assigned object placement with id: {ifcObjectPlacement.EntityLabel}");
        }

        public void AssignGeometry(IIfcGeometry geometry)
        {
            if (IsCreated)
                throw new IfcEntityCreatedException(
                    $"{GetType().Name} entity already created. Cannot call {nameof(AssignGeometry)} method!");
            Geometry = geometry;
            _logger.Info($"Assigned geometry with type: {geometry.GetType().Name}");
        }

        public void AssignMaterial(IIfcMaterial material)
        {
            if (IsCreated)
                throw new IfcEntityCreatedException(
                    $"{GetType().Name} entity already create. Cannot call {nameof(AssignMaterial)} method!");
            Material = material;
            _logger.Info($"Assigned material with id: {material.EntityLabel}, name: {material.Name}");
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