using System.Collections.Generic;
using IFC.Entities.Interfaces;
using IFC.PropertySets;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class NewIfcAbstractSegmentEntity : NewIfcAbstractEntity, IIfcTwoNodeEntity, IIfcClippable
    {
        public IfcNodeEntity[] NodeEntities { get; }
        public XbimVector3D Direction { get; }
        
        public double Length { get; }
        public double Diameter { get; }
        
        protected NewIfcAbstractSegmentEntity(IfcIdentifier tag, IEnumerable<IPropertySet> propertySets, IfcNodeEntity[] nodeEntities, double length, double diameter) 
            : base(tag, propertySets)
        {
            NodeEntities = nodeEntities;
            Length = length;
            Diameter = diameter;
        }
        
        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            throw new System.NotImplementedException();
        }
    }
}