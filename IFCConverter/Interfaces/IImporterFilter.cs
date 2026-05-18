using System.Diagnostics.Contracts;
using Ifc.Interfaces;

namespace IFCConverter.Interfaces
{
    internal interface IImporterFilter
    {
        [Pure]
        public bool IsMatch(IIfcProject ifcProject);
    }
}