using System.Diagnostics.Contracts;
using Ifc.Interfaces;
using IFCConverter.Interfaces;

namespace IFCConverter.Converters.Importers.Aveva
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