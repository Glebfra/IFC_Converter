using IFC.Entities.Interfaces;
using IFC.Extensions;
using Start.Entities.Abstract;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractConnectorEntity : IfcAbstractEntity, IIfcOneNodeEntity, IIfcSegmentDependedEntity
    {
        public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
        
        public IfcNodeEntity NodeEntity { get; }
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
        public double Angle { get; protected set; }

        public IfcAbstractConnectorEntity(StartAbstractEntity abstractEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
            : base(abstractEntity)
        {
            NodeEntity = ifcNodeEntity;
            AbstractSegmentEntities = abstractSegmentEntities;

            ObjectMatrix3D = MatrixExtensions.CreateWorldMatrixFromSegments(ifcNodeEntity, abstractSegmentEntities, out double angle);
            Angle = angle;
        }
    }
}