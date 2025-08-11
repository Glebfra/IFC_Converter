using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractSegmentEntity : IfcAbstractEntity, IIfcClippable, IIfcTwoNodeEntity
    {
        public abstract ActionProperty<double> Diameter { get; }
        public ActionProperty<double> Length { get; }
        public IfcNodeEntity[] NodeEntities { get; }

        public IfcNodeEntity StartNode => NodeEntities[0];
        public IfcNodeEntity EndNode => NodeEntities[1];
        public XbimVector3D SegmentDirection => ObjectMatrix3D.Value.Forward * Length;

        public IfcAbstractSegmentEntity(XbimMatrix3D matrix3D, double length) 
            : base(matrix3D)
        {
            XbimMatrix3D secondMatrix3D = XbimMatrix3D.CreateWorld(
                matrix3D.Translation + matrix3D.Forward * length, 
                matrix3D.Forward, 
                matrix3D.Up
            );
            NodeEntities = new IfcNodeEntity[]
            {
                new IfcNodeEntity(matrix3D),
                new IfcNodeEntity(secondMatrix3D)
            };
            Length = length;
        }
        
        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            if (IsStartNode(nodeEntity))
                ObjectMatrix3D.Value = XbimMatrix3D.CreateWorld(
                    ObjectMatrix3D.Value.Translation + ObjectMatrix3D.Value.Forward * clipLength,
                    ObjectMatrix3D.Value.Forward,
                    ObjectMatrix3D.Value.Up
                );
            Length.Value -= clipLength;
        }

        protected T CreateIfcEntity<T>(IModel model, IfcPipeSegmentTypeEnum pipeSegmentType)
            where T : IfcPipeSegment, IInstantiableEntity
        {
            T pipeSegment = base.CreateIfcEntity<T>(model);
            pipeSegment.PredefinedType = pipeSegmentType;
            
            return pipeSegment;
        }

        private bool IsStartNode(IfcNodeEntity nodeEntity)
        {
            XbimVector3D nodeCoordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D startPipeCoordinates = ObjectMatrix3D.Value.Translation;
            XbimVector3D endPipeCoordinates = ObjectMatrix3D.Value.Translation + ObjectMatrix3D.Value.Forward * Length.Value;

            return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
        }
    }
}