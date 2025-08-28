using STARTtoIFC.Tools;

namespace STARTtoIFC.Importers
{
    internal static class ImporterFactory
    {
        public static IImporter CreateImporter(ImportTypeEnum importTypeEnum)
        {
            return importTypeEnum switch
            {
                ImportTypeEnum.STANDARD => new StandardImporter(),
                _ => new StandardImporter()
            };
        }
    }
}