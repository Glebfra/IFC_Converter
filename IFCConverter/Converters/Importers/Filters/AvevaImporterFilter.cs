using Ifc.Interfaces;
using IFCConverter.Interfaces;

namespace IFCConverter.Converters.Importers.Filters
{
    internal class AvevaImporterFilter : IFilter
    {
        public bool IsMatch(IIfcProject ifcProject)
        {
            return ifcProject.Model.Header.CreatingApplication.Contains("AVEVA E3D");
        }
    }
}