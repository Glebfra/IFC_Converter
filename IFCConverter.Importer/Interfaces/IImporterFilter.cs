using System.Diagnostics.Contracts;
using IFCConverter.IFC.Interfaces;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IImporterFilter
    {
        [Pure]
        bool IsMatch(IIfcProject ifcProject);
    }
}