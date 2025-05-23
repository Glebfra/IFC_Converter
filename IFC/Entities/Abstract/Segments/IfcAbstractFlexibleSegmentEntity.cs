using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

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