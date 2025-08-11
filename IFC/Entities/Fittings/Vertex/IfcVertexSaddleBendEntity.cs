using IFC.Entities.Abstract.Fittings;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    public class IfcVertexSaddleBendEntity : IfcAbstractVertexSaddleBendEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double Angle { get; }
        public override double BendRadius { get; }
        public override double PipeRadius { get; }
        public override ActionProperty<int> NumSegments { get; }
        public override ActionProperty<double> BranchHeight { get; }
        public override ActionProperty<double> BranchPipeRadius { get; }
        
        public IfcVertexSaddleBendEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, 
            double length, double angle, double bendRadius, double pipeRadius, int numSegments, double branchHeight, double branchPipeRadius) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Angle = angle;
            BendRadius = bendRadius;
            PipeRadius = pipeRadius;
            NumSegments = numSegments;
            BranchHeight = branchHeight;
            BranchPipeRadius = branchPipeRadius;
        }
    }
}