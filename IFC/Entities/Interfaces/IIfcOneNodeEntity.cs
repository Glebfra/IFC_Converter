namespace IFC.Entities.Interfaces
{
    public interface IIfcOneNodeEntity : IIfcEntity
    {
        public IfcNodeEntity NodeEntity { get; set; }
    }
}