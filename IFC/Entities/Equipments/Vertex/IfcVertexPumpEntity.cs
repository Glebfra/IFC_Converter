using System.Linq;
using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
    #if NEW

    public class IfcVertexPumpEntity : IfcAbstractVertexPumpEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<int> NumSegments { get; }
        public override ActionProperty<double>[] Diameters { get; }
        
        public IfcVertexPumpEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double length, double[] diameters, int numSegments) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameters = diameters.Select(diameter => new ActionProperty<double>(diameter)).ToArray();
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexPumpEntity : IfcAbstractVertexPumpEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexPumpEntity(StartPumpEntity pumpEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
            : base(pumpEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = AbstractSegmentEntities[0].Diameter / 2;
        }
    }

    #endif
}