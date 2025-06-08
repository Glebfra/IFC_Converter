using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractFlexibleSegmentEntity : IfcAbstractStraightSegment
    {
        private StartFlexibleElementEntity _flexibleElement;
        private IfcPipeSegment? _pipeSegment;
        
        protected IfcAbstractFlexibleSegmentEntity(StartFlexibleElementEntity flexibleElement, IfcNodeEntity[] nodeEntities) 
            : base(flexibleElement, nodeEntities)
        {
            _flexibleElement = flexibleElement;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _flexibleElement.Name, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}