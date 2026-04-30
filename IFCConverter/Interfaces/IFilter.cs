using Ifc.Interfaces;

namespace IFCConverter.Interfaces
{
    internal interface IFilter
    {
        public bool IsMatch(IIfcProject ifcProject);
    }
}