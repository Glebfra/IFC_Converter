using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractPipeSegmentEntity : IfcAbstractStraightSegment
    {
        private StartPipeEntity _pipeEntity;
        private IfcPipeSegment? _pipeSegment;
        
        protected IfcAbstractPipeSegmentEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] nodeEntities) 
            : base(pipeEntity, nodeEntities)
        {
            _pipeEntity = pipeEntity;
        }

        protected IfcAbstractPipeSegmentEntity(IfcIdentifier tag, double length, double diameter, IfcAxisSettings axisSettings)
            : base(tag, length, diameter, axisSettings)
        {
            
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _pipeEntity.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}