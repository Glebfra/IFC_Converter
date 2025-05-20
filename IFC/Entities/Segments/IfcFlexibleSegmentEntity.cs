using System;
using IFC.Entities.Abstract;
using IFC.Entities.Interfaces;
using IFC.Extensions;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Segments
{
    public sealed class IfcFlexibleSegmentEntity : IfcAbstractSegmentEntity, IIfcSegmentDependedEntity
    {
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        public override XbimVector3D Direction { get; protected set; }
        public override double OuterDiameter { get; protected set; }
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }

        private StartFlexibleElementEntity _startFlexibleElementEntity;
        private IfcPipeSegment _pipeSegment;
        
        public IfcFlexibleSegmentEntity(StartFlexibleElementEntity startFlexibleElementEntity, IfcNodeEntity[] ifcNodeEntities, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startFlexibleElementEntity, ifcNodeEntities)
        {
            _startFlexibleElementEntity = startFlexibleElementEntity;
            AbstractSegmentEntities = abstractSegmentEntities;
            
            Coordinates = ifcNodeEntities[0].ObjectMatrix3D.Translation;
            Direction = ifcNodeEntities[1].ObjectMatrix3D.Translation - Coordinates;
            Length = Direction.Length;
            
            //TODO change to MatrixExtensions
            XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
            XbimVector3D forward = Direction.Normalized();
            if (forward == WorldUp || forward == -1 * WorldUp) 
                WorldUp = new XbimVector3D(0, 1, 0);
            XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
            
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(Coordinates, forward, up);
            OuterDiameter = abstractSegmentEntities.Length switch
            {
                1 => abstractSegmentEntities[0].OuterDiameter,
                2 => Math.Min(abstractSegmentEntities[0].OuterDiameter, abstractSegmentEntities[1].OuterDiameter),
                _ => 0.05
            };
            OuterSurfaceArea = MathExtensions.CalculateCylinderArea(OuterDiameter / 2, Length);
            
            _OnLengthChanged += () => MathExtensions.CalculateCylinderArea(OuterDiameter / 2, Length);
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _startFlexibleElementEntity.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}