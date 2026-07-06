using System.Runtime.InteropServices;

namespace IFCConverter
{
    [Guid("43F28B41-5D50-4387-8F06-FEF894D67D59")]
    public interface IIfcConverter
    {
        int Export(object startDocumentObject, int languageId);
        int ImportFromFileImport(object startAutoServerObject, int languageId, string startTempFileName);

        int ImportFromFileOpen(object startAutoServerObject, int languageId, string startTempFileName,
            string ifcFileName);

        int Test();
    }
}