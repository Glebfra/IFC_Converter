using IFC.Entities.Fittings;

namespace IFC.Entities.Interfaces
{
    public interface IIfcOneNodeEntity : IIfcEntity
    {
        public IfcNodeEntity IfcNodeEntity { get; }
    }
}