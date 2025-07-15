using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

namespace IFC.Entities.Abstract
{
    public abstract class NewIfcAbstractEntity : IIfcNewEntity
    {
        public abstract ActionProperty<IfcLabel> Name { get; }
        public abstract ActionProperty<IfcIdentifier> Tag { get; } 
        public abstract ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public ActionProperty<Colour> Colour { get; } = new ActionProperty<Colour>(IFC.Tools.Colour.FromHEX("bebebe"));
        
        public List<IPropertySet> PropertySets { get; } = new List<IPropertySet>();

        public abstract IfcProduct CreateAndAdd(IModel model);

        protected T CreateIfcEntity<T>(IModel model)
            where T : IfcElement, IInstantiableEntity
        {
            IfcCartesianPoint cartesianPoint = IfcAxis.CreatePoint(model, ObjectMatrix3D.Value.Translation);
            IfcDirection forward = IfcAxis.CreateDirection(model, ObjectMatrix3D.Value.Forward);
            IfcDirection right = IfcAxis.CreateDirection(model, ObjectMatrix3D.Value.Right);
            
            ObjectMatrix3D.OnValueChange += () => cartesianPoint.SetVector(ObjectMatrix3D.Value.Translation);
            ObjectMatrix3D.OnValueChange += () => forward.SetVector(ObjectMatrix3D.Value.Forward);
            ObjectMatrix3D.OnValueChange += () => right.SetVector(ObjectMatrix3D.Value.Right);
            
            IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, cartesianPoint, forward, right);
            IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);

            T ifcElement = model.Instances.New<T>(product =>
            {
                product.Name = Name;
                product.Tag = Tag;
                product.ObjectPlacement = localPlacement;

                Name.OnValueChange += () => product.Name = Name;
                Tag.OnValueChange += () => product.Tag = Tag;
            });
            
            AddProperties(model, ifcElement);
            return ifcElement;
        }

        private void AddProperties(IModel model, IfcProduct product)
        {
            foreach (IPropertySet propertySet in PropertySets)
            {
                model.Instances.New<IfcRelDefinesByProperties>(properties =>
                {
                    properties.Name = propertySet.GetType().Name;
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