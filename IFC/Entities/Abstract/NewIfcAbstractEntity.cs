using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.RepresentationResource;
using IfcObjectPlacement = IFC.Tools.IfcObjectPlacement;

namespace IFC.Entities.Abstract
{
    public abstract class NewIfcAbstractEntity : IIfcNewEntity
    {
        public abstract ActionProperty<IfcLabel> Name { get; }
        public abstract ActionProperty<IfcIdentifier> Tag { get; } 
        public abstract ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public virtual ActionProperty<Colour> Colour { get; } = new ActionProperty<Colour>(IFC.Tools.Colour.FromHEX("bebebe"));
        
        public List<IPropertySet> PropertySets { get; } = new List<IPropertySet>();
        public List<NewIfcAbstractEntity> ConnectedEntities { get; } = new List<NewIfcAbstractEntity>();

        public abstract IfcProduct CreateAndAdd(IModel model);

        protected T CreateIfcEntity<T>(IModel model)
            where T : IfcElement, IInstantiableEntity
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            T ifcElement = model.Instances.New<T>(product =>
            {
                product.Name = Name;
                product.Tag = Tag;
                product.ObjectPlacement = objectPlacement.LocalPlacement;

                Name.OnValueChange += () => product.Name = Name;
                Tag.OnValueChange += () => product.Tag = Tag;
            });
            
            AddProperties(model, ifcElement);
            return ifcElement;
        }

        protected void AddShapeRepresentation(IModel model, IfcProduct product, IfcRepresentationItem representationItem)
        {
            AddShapeRepresentation(model, product, new[] { representationItem });
        }
        
        protected void AddShapeRepresentation(IModel model, IfcProduct product, IEnumerable<IfcRepresentationItem> representationItems)
        {
            ColourEntity(model, representationItems);
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems);
            product.Representation = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        }

        private void AddProperties(IModel model, IfcProduct product)
        {
            foreach (IPropertySet propertySet in PropertySets)
            {
                model.Instances.New<IfcRelDefinesByProperties>(properties =>
                {
                    properties.Name = propertySet.GetType().Name;
                    properties.RelatedObjects.Add(product);
                    properties.RelatingPropertyDefinition = propertySet.CreatePropertySet(model);
                });
            }
        }
        
        protected void ColourEntity(IModel model, IEnumerable<IfcRepresentationItem> representationItems)
        {
            IfcColours.StyleItems(model, Colour, representationItems);
        }

        protected void ColourEntity(IModel model, IfcRepresentationItem representationItems)
        {
            ColourEntity(model, new[] { representationItems });
        }
    }
}