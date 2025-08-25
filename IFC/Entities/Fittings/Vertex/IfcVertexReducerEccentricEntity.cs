using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    public class IfcVertexReducerEccentricEntity : IfcAbstractVertexReducerEccentricEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double DisplacementLength { get; }
        public override ActionProperty<double>[] Diameters { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcVertexReducerEccentricEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double displacementLength, double[] diameters, int numSegments) : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            DisplacementLength = displacementLength;
            Diameters = diameters.Select(diameter => new ActionProperty<double>(diameter)).ToArray();
            NumSegments = numSegments;
        }
    }
}