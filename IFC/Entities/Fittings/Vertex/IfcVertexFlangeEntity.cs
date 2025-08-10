using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW

    public class IfcVertexFlangeEntity : IfcAbstractVertexFlangeEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double[] Diameters { get; }
        public override int NumSegments { get; }

        public IfcVertexFlangeEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double[] diameters, int numSegments) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameters = diameters;
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexFlangeEntity : IfcAbstractVertexFlangeEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double[] Radiuses { get; protected set; }
        
        public IfcVertexFlangeEntity(StartArmatureEntity armatureEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(armatureEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = armatureEntity.Length.SIProperty;
            Radiuses = segmentEntities.Select(entity => entity.Diameter / 2).ToArray();
        }
    }

    #endif
}