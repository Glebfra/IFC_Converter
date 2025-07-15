using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class NewIfcAbstractSegmentEntity : NewIfcAbstractEntity, IIfcClippable
    {
        public abstract ActionProperty<double> Length { get; }
        public abstract ActionProperty<double> Diameter { get; }

        protected T CreateIfcEntity<T>(IModel model, IfcPipeSegmentTypeEnum pipeSegmentType)
            where T : IfcPipeSegment, IInstantiableEntity
        {
            T pipeSegment = base.CreateIfcEntity<T>(model);
            pipeSegment.PredefinedType = pipeSegmentType;
            
            return pipeSegment;
        }

        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            throw new System.NotImplementedException();
        }
    }
}