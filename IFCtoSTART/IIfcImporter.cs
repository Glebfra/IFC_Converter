using System.Runtime.InteropServices;

namespace IFCtoSTART
{
    [Guid("FEE279C3-BBCE-4134-9BC5-A4786C4FFD1C")]
    public interface IIfcImporter
    {
        int Import(ref object startDocumentObject, int languageId);

        int Test();
    }
}