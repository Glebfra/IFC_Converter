using IFC.Entities.Fittings;

namespace IFC.Entities.Interfaces
{
    public interface IIfcClippable
    {
        public void Clip(IfcNodeEntity nodeEntity, double clipLength);
    }
}