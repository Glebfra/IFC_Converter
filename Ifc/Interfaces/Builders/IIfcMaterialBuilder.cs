using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Interfaces
{
    public interface IIfcMaterialBuilder : IIfcBuilder
    {
        public IfcLabel MaterialName { get; }
        public IfcText Description { get; }
        public IfcLabel Category { get; }

        public IIfcMaterial CreateMaterial(IModel model);
        public bool GetOrCreateMaterial(IModel model, out IIfcMaterial material);
    }
}