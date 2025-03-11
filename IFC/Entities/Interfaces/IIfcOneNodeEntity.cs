using IFC.Entities.Fittings;

namespace IFC.Entities.Interfaces
{
    public interface IIfcOneNodeEntity
    {
        public IfcNodeEntity NodeEntity { get; set; }
    }
}