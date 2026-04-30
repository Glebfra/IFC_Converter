using System.Diagnostics.Contracts;
using Ifc.Interfaces;

namespace IFCConverter.Interfaces
{
    internal interface IFilter
    {
        [Pure]
        public bool IsMatch(IIfcProject ifcProject);
    }
}