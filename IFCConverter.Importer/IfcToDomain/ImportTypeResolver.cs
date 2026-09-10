using Xbim.Common;

namespace IFCConverter.Importer.IfcToDomain
{
    internal sealed class ImportTypeResolver
    {
        public ImportType ResolveImportType(IModel model)
        {
            if (model.Header.CreatingApplication.Contains("AVEVA E3D"))
                return ImportType.AVEVA;
            
            return ImportType.UNKNOWN;
        }
    }
}