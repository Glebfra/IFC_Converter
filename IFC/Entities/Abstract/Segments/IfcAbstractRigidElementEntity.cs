using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractRigidElementEntity : IfcAbstractStraightSegment
    {
        private StartRigidElementEntity _rigidElement;
        private IfcPipeSegment? _pipeSegment;
        
        protected IfcAbstractRigidElementEntity(StartRigidElementEntity rigidElement, IfcNodeEntity[] nodeEntities) 
            : base(rigidElement, nodeEntities)
        {
            _rigidElement = rigidElement;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeSegment = CreatePipeSegment(model, _rigidElement.Name, IfcPipeSegmentTypeEnum.RIGIDSEGMENT);
            AddProperties(model, _pipeSegment);
            return _pipeSegment;
        }
    }
}