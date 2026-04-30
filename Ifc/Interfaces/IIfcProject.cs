using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace Ifc.Interfaces
{
    public interface IIfcProject
    {
        public IModel Model { get; }
        
        public void AddEntityRaw(IfcProduct product);
        public void SaveAs(string filepath);
    }
}