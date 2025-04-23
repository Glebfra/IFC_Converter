using IFC.Entities.Abstract;
using IFC.Entities.Interfaces;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Equipments.Vertex
{
    public class IfcVertexPumpEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public IfcNodeEntity NodeEntity { get; }
        public override XbimMatrix3D ObjectMatrix3D { get; protected set; }

        private int _numSegments;
        
        private IfcAbstractSegmentEntity[] _segmentEntities;
        
        public IfcVertexPumpEntity(StartAbstractEntity abstractEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
            : base(abstractEntity)
        {
            NodeEntity = nodeEntity;
            _segmentEntities = segmentEntities;

            _numSegments = numSegments;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}