using System;
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
    public sealed class IfcRigidElementEntity : IfcAbstractSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; }
        public override double Diameter { get; }

        protected override IfcIdentifier Tag { get; set; } = "Rigid Element";

        private StartRigidElementEntity _startRigidElementEntity;
        private IfcPipeSegment _pipeSegment;

        public IfcRigidElementEntity(StartRigidElementEntity startRigidElementEntity, IfcNodeEntity[] ifcNodeEntities, IfcAbstractSegmentEntity[]? abstractSegmentEntities = null) 
            : base(ifcNodeEntities)
        {
            _startRigidElementEntity = startRigidElementEntity;
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            Direction = new XbimVector3D(
                _startRigidElementEntity.ProjectionAlongOXAxis,
                _startRigidElementEntity.ProjectionAlongOYAxis,
                _startRigidElementEntity.ProjectionAlongOZAxis
            );
            Length = Direction.Length;

            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = Direction.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(Coordinates, forward, up);

            if (abstractSegmentEntities != null)
            {
                Diameter = Math.Min(abstractSegmentEntities[0].Diameter, abstractSegmentEntities[1].Diameter);
                if (Diameter > 0.05) Diameter = 0.05;
            }
            else
            {
                Diameter = 0.05;
            }
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _startRigidElementEntity.Name, IfcPipeSegmentTypeEnum.RIGIDSEGMENT);
            return _pipeSegment;
        }
    }
}