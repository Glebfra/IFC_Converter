using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Equipments
{
    public abstract class IfcAbstractEquipmentEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public abstract ActionProperty<double> Length { get; }

        public IfcNodeEntity NodeEntity { get; }

        protected IfcAbstractEquipmentEntity(XbimMatrix3D objectMatrix)
            : base(objectMatrix)
        {
            NodeEntity = new IfcNodeEntity(objectMatrix);
        }
    }
}