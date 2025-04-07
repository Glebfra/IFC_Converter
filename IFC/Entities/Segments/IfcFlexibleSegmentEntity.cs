using System;
using IFC.Entities.Abstract;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Segments
{
    public sealed class IfcFlexibleSegmentEntity : IfcAbstractSegmentEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; }
        public override double Diameter { get; }

        private StartFlexibleElementEntity _startFlexibleElementEntity;
        private IfcPipeSegment _pipeSegment;
        
        public IfcFlexibleSegmentEntity(StartFlexibleElementEntity startFlexibleElementEntity, IfcNodeEntity[] ifcNodeEntities, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startFlexibleElementEntity, ifcNodeEntities)
        {
            _startFlexibleElementEntity = startFlexibleElementEntity;
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            Direction = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            Length = Direction.Length;
            
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = Direction.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(Coordinates, forward, up);
            Diameter = abstractSegmentEntities.Length switch
            {
                1 => abstractSegmentEntities[0].Diameter,
                2 => Math.Min(abstractSegmentEntities[0].Diameter, abstractSegmentEntities[1].Diameter),
                _ => 0.05
            };
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _startFlexibleElementEntity.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}