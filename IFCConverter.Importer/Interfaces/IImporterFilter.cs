using System.Diagnostics.Contracts;
using Ifc.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IImporterFilter
    {
        [Pure]
        public bool IsMatch(IIfcProject ifcProject);
    }
}