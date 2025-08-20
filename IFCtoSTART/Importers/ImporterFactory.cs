using IFCtoSTART.Tools;

namespace IFCtoSTART.Importers
{
    internal static class ImporterFactory
    {
        public static IImporter CreateImporter(ImportType importType)
        {
            return importType switch
            {
                ImportType.STANDARD => new StandardImporter(),
                ImportType.START => new StartImporter(),
                _ => new StandardImporter()
            };
        }
    }
}