using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities
{
    public sealed class IfcCylindricalShellEntity : IfcAbstractSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; }
        public override double Diameter { get; }

        public override IfcIdentifier Tag { get; protected set; } = "Cylindrical Shell";
        
        private StartPipeEntity _startPipeEntity;
        private IfcPipeSegment _pipeSegment;
        
        public IfcCylindricalShellEntity(StartPipeEntity startPipeEntity, IfcNodeEntity[] ifcNodeEntities) 
            : base(ifcNodeEntities)
        {
            _startPipeEntity = startPipeEntity;
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            Direction = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            Length = Direction.Length;

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
            return _pipeSegment;
        }
    }
}