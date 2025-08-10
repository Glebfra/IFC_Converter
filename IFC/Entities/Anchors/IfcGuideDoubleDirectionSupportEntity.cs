using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Anchors
{
    #if NEW

    public class IfcGuideDoubleDirectionSupportEntity : IfcAbstractGuideDoubleDirectionSupportEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Diameter { get; }
        public override ActionProperty<double> Height { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcGuideDoubleDirectionSupportEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double diameter, double height, int numSegments) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Diameter = diameter;
            Height = height;
            NumSegments = numSegments;
        }
    }

#else
    
    public sealed class IfcGuideDoubleDirectionSupportEntity : IfcAbstractGuideDoubleDirectionSupportEntity
    {
        public override double Diameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Height { get; protected set; }
        
        public IfcGuideDoubleDirectionSupportEntity(StartGuideDoubleDirectionSupportEntity doubleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(doubleDirectionSupportEntity, nodeEntity, segmentEntities)
        {
            NumSegments = 8;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Height = Diameter * 2;
        }
    }

    #endif
}