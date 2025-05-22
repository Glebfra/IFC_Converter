using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Segments
{
    public sealed class IfcPipeSegmentEntity : IfcAbstractPipeSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override double Diameter { get; protected set; }
        public override ActionProperty<double> Length { get; protected set; }
        public override ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public override ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        public override XbimVector3D Direction { get; }

        public IfcPipeSegmentEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities) 
            : base(pipeEntity, nodeEntities)
        {
            Coordinates = new ActionProperty<XbimVector3D>(nodeEntities[0].ObjectMatrix3D.Translation);
            XbimVector3D nodesDirection = nodeEntities[1].ObjectMatrix3D.Translation - Coordinates.Value;
            XbimVector3D pipeProjection = new XbimVector3D(
                pipeEntity.ProjectionAlongOXAxis.SIProperty,
                pipeEntity.ProjectionAlongOYAxis.SIProperty,
                pipeEntity.ProjectionAlongOZAxis.SIProperty
            );
            Direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            Length = new ActionProperty<double>(Direction.Length);

            XbimVector3D forward = Direction.Normalized();
            ObjectMatrix3D = MatrixExtensions.CreateWorld(Coordinates.Value, forward);
            
            Diameter = pipeEntity.Diameter.SIProperty;
            OuterSurfaceArea = new ActionProperty<double>(MathExtensions.CalculateCylinderArea(Diameter / 2, Length.Value));
            
            Length.OnValueChange += () => OuterSurfaceArea.Value = MathExtensions.CalculateCylinderArea(Diameter / 2, Length.Value);
        }
    }
}