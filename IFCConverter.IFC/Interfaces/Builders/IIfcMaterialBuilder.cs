using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcMaterialBuilder : IIfcBuilder
    {
        IfcLabel MaterialName { get; }
        IfcText Description { get; }
        IfcLabel Category { get; }

        IIfcMaterial CreateMaterial(IModel model);
        bool GetOrCreateMaterial(IModel model, out IIfcMaterial material);
    }
}