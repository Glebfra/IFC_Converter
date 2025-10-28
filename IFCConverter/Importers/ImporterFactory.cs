using IFC;
using IFCConverter.Tools;

namespace IFCConverter.Importers
{
    internal static class ImporterFactory
    {
        public static IImporter CreateImporter(IFCProject ifcProject, ImportTypeEnum importTypeEnum)
        {
            return importTypeEnum switch
            {
                ImportTypeEnum.STANDARD => new StandardImporter(ifcProject),
                ImportTypeEnum.AVEVA => new AvevaImporter(ifcProject),
                _ => new StandardImporter(ifcProject)
            };
        }
    }
}