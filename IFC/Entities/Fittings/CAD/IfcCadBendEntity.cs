using IFC.Entities.Abstract.Fittings;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    public class IfcCadBendEntity : IfcAbstractCadBendEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double Angle { get; }
        public override double BendRadius { get; }
        public override double PipeRadius { get; }

        public IfcCadBendEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double angle, double bendRadius, double pipeRadius)
            : base(objectMatrix3D)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            Length = new ActionProperty<double>(length);
            Angle = angle;
            BendRadius = bendRadius;
            PipeRadius = pipeRadius;
        }
    }
}