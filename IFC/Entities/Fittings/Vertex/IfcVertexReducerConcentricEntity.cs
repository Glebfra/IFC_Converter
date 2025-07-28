using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW

    public class IfcVertexReducerConcentricEntity : IfcAbstractVertexReducerConcentricEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double>[] Diameters { get; }
        public override ActionProperty<int> NumSegments { get; }

        public IfcVertexReducerConcentricEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double[] diameters, int numSegments) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameters = diameters.Select(diameter => new ActionProperty<double>(diameter)).ToArray();
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexReducerConcentricEntity : IfcAbstractVertexReducerConcentricEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexReducerConcentricEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(reducerEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = reducerEntity.LengthOfConicalPart.SIProperty;
        }
    }

    #endif
}