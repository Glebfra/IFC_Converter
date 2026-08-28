using System.Diagnostics.Contracts;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Importer.Interfaces;

namespace IFCConverter.Importer.Importers.Aveva
{
    internal class AvevaImporterImporterFilter : IImporterFilter
    {
        [Pure]
        public bool IsMatch(IIfcProject ifcProject)
        {
            return ifcProject.Model.Header.CreatingApplication.Contains("AVEVA E3D");
        }
    }
}