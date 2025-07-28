using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Segments
{
    #if NEW
    
    public class IfcCylindricalShellEntity : IfcAbstractPipeSegmentEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Diameter { get; }
        
        public IfcCylindricalShellEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter) 
            : base(objectMatrix3D, length)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Diameter = new ActionProperty<double>(diameter);
        }
    }
    
    #else
    
    public sealed class IfcCylindricalShellEntity : IfcAbstractPipeSegmentEntity
    {
        public override Colour Colour { get; protected set; } = Colour.FromHEX("3e3ec0");
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        public override ActionProperty<double> RealLength { get; protected set; }
        public override ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public override ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        public override XbimVector3D Direction { get; }

        public IfcCylindricalShellEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities) 
            : base(pipeEntity, nodeEntities)
        {
            Coordinates = new ActionProperty<XbimVector3D>(nodeEntities[0].ObjectMatrix3D.Translation);
            Direction = nodeEntities[1].ObjectMatrix3D.Translation - Coordinates.Value;
            RealLength = new ActionProperty<double>(Direction.Length);
            Length = Direction.Length;

            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates.Value, forward);

            Diameter = pipeEntity.Diameter.SIProperty;
            OuterSurfaceArea = new ActionProperty<double>(MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value));
            
            RealLength.OnValueChange += () => OuterSurfaceArea.Value = MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value);
        }
    }

    #endif
}