using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Segments
{
    public sealed class IfcConeElementEntity : IfcAbstractConeElementEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override Colour Colour { get; protected set; } = Colour.FromHEX("46008b");
        public override double Diameter { get; protected set; }
        public override ActionProperty<double> Length { get; protected set; }
        public override ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public override ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        public override XbimVector3D Direction { get; }
        public override double SecondDiameter { get; set; }
        protected override int _NumSegments { get; set; } = 16;
        
        public IfcConeElementEntity(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities) 
            : base(coneElement, nodeEntities)
        {
            Coordinates = new ActionProperty<XbimVector3D>(nodeEntities[0].ObjectMatrix3D.Translation);
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - Coordinates.Value;
            XbimVector3D pipeProjection = new XbimVector3D(
                coneElement.ProjectionAlongOXAxis.SIProperty,
                coneElement.ProjectionAlongOYAxis.SIProperty,
                coneElement.ProjectionAlongOZAxis.SIProperty
            );
            Direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            Length = new ActionProperty<double>(Direction.Length);
            Direction = Direction.Normalized();
            
            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates.Value, forward);

            Diameter = coneElement.Diameter.SIProperty;
            SecondDiameter = coneElement.SecondDiameter.SIProperty;
            OuterSurfaceArea = new ActionProperty<double>(MathExtensions.CalculateClippedConeArea(Diameter / 2, SecondDiameter / 2, Length.Value));
            
            Length.OnValueChange += () => OuterSurfaceArea.Value = MathExtensions.CalculateClippedConeArea(Diameter / 2, SecondDiameter / 2, Length.Value);
        }
    }
}