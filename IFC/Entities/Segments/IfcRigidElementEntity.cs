using System;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Segments
{
    public sealed class IfcRigidElementEntity : IfcAbstractRigidElementEntity, IIfcSegmentDependedEntity
    {
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
        
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override Colour Colour { get; protected set; } = Colour.FromHEX("009249");
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        public override ActionProperty<double> RealLength { get; protected set; }
        public override ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public override ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        public override XbimVector3D Direction { get; }

        public IfcRigidElementEntity(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(rigidElement, nodeEntities)
        {
            AbstractSegmentEntities = segmentEntities;
            
            Coordinates = new ActionProperty<XbimVector3D>(nodeEntities[0].ObjectMatrix3D.Translation);
            Direction = nodeEntities[1].ObjectMatrix3D.Translation - Coordinates.Value;
            RealLength = new ActionProperty<double>(Direction.Length);
            Length = Direction.Length;

            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates.Value, forward);

            Diameter = segmentEntities.Length switch
            {
                1 => segmentEntities[0].Diameter,
                2 => Math.Min(segmentEntities[0].Diameter, segmentEntities[1].Diameter),
                _ => 0.05
            };
            if (Diameter > 0.05) Diameter = 0.05;
            OuterSurfaceArea = new ActionProperty<double>(MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value));
            
            RealLength.OnValueChange += () => OuterSurfaceArea.Value = MathExtensions.CalculateCylinderArea(Diameter / 2, RealLength.Value);
        }
    }
}