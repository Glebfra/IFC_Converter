using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Segments
{
    [IfcEntityType(false, StartElementType.PIPE_ELEMENT)]
    public sealed class IfcPipeEntity : IfcAbstractSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; }
        public override double Diameter { get; }

        private StartPipeEntity _startPipeEntity;
        private IfcPipeSegment _pipeSegment;

        public IfcPipeEntity(StartPipeEntity startPipeEntity, IfcNodeEntity[] ifcNodeEntities) 
            : base(startPipeEntity, ifcNodeEntities)
        {
            _startPipeEntity = startPipeEntity;
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            XbimVector3D nodesDirection = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            XbimVector3D pipeProjection = new XbimVector3D(
                startPipeEntity.ProjectionAlongOXAxis,
                startPipeEntity.ProjectionAlongOYAxis,
                startPipeEntity.ProjectionAlongOZAxis
            );
            Direction = (pipeProjection * XbimVector3D.DotProduct(nodesDirection, pipeProjection)).Normalized() * pipeProjection.Length;
            Length = Direction.Length;
            Direction = Direction.Normalized();

            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = Direction.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(Coordinates, forward, up);
            Diameter = _startPipeEntity.Diameter;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _startPipeEntity.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}