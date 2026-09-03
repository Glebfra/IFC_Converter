using System.Linq;
using IFCConverter.Start.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Start.Entities.Equipments
{
    public abstract class StartAbstractEquipmentEntity : StartAbstractEntity, IStartFittingEntity
    {
        public Vector<double> Position { get; set; }
        public IStartNodeEntity Node => ConnectedEntities.OfType<IStartNodeEntity>().First();
    }
}