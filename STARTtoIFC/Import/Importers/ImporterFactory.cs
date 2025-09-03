using STARTtoIFC.Tools;
using Xbim.Common;

namespace STARTtoIFC.Importers
{
    internal static class ImporterFactory
    {
        public static IImporter CreateImporter(IModel model, ImportTypeEnum importTypeEnum)
        {
            return importTypeEnum switch
            {
                ImportTypeEnum.STANDARD => new StandardImporter(model),
                ImportTypeEnum.AVEVA => new AvevaImporter(model),
                _ => new StandardImporter(model)
            };
        }
    }
}