using IFC.Entities.Abstract.Fittings;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcNonStandardTeeEntity : IfcAbstractTeeEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> BranchDiameter { get; }
        public override ActionProperty<double> HeadDiameter { get; }
        public override ActionProperty<double> Height { get; }
        public override ActionProperty<double> Angle { get; }

        public IfcNonStandardTeeEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double branchDiameter, double headDiameter, double height, double angle) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            BranchDiameter = branchDiameter;
            HeadDiameter = headDiameter;
            Height = height;
            Angle = angle;
        }
    }
}