using System.Linq;
using IFC.Entities.Abstract.Equipments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
    public class IfcVertexCompressorEntity : IfcAbstractVertexCompressorEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double>[] Diameters { get; }
        public override int NumSegments { get; }
        
        public IfcVertexCompressorEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double length, double[] diameters, int numSegments) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameters = diameters.Select(diameter => new ActionProperty<double>(diameter)).ToArray();
            NumSegments = numSegments;
        }
    }
}