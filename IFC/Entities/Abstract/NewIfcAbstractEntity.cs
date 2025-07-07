using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Abstract
{
    public abstract class NewIfcAbstractEntity : IIfcEntity
    {
        public IfcIdentifier Tag { get; }
        public IEnumerable<IPropertySet>? PropertySets { get; }
        
        public virtual Colour Colour { get; } = Colour.FromHEX("bebebe");
        public virtual XbimMatrix3D ObjectMatrix3D { get; } = XbimMatrix3D.CreateWorld(XbimVector3D.Zero, VectorExtensions.Forward, VectorExtensions.Up);

        protected NewIfcAbstractEntity(IfcIdentifier tag, IEnumerable<IPropertySet> propertySets)
        {
            Tag = tag;
            PropertySets = propertySets;
        }

        public abstract IfcProduct CreateAndAdd(IModel model);

        protected void AddProperties(IModel model)
        {
            if (PropertySets == null)
                return;
            
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