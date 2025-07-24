using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW

    public sealed class IfcConnectorEntity : IfcAbstractConnectorEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }

        public IfcConnectorEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter)
        {
            Name = name;
            Tag = tag;
            ObjectMatrix3D = objectMatrix3D;
            Length = length;
            Diameter = diameter;
        }
    }
    
    #else
    
    public sealed class IfcConnectorEntity : IfcAbstractConnectorEntity
    {
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }

        public IfcConnectorEntity(StartConnectorEntity connectorEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(connectorEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            Diameter = abstractSegmentEntities[0].Diameter;
            Length = Diameter / 4;
        }
    }

    #endif
}