using System.Linq;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.Entities.Equipments
{
    public abstract class StartAbstractEquipmentEntity : StartAbstractEntity, IStartFittingEntity
    {
        public FixedVector<Dim3> Position { get; set; }
        public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}