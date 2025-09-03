using System.Runtime.InteropServices;

namespace IFCConverter
{
    [Guid("43F28B41-5D50-4387-8F06-FEF894D67D59")]
    public interface IIfcConverter
    {
        int Export(object startDocumentObject, int languageId);
        int ImportFromFile(object startDocumentObject, int languageId);
        int ImportFromDrop(object startAutoServerObject, int languageId);

        int Test();
    }
}