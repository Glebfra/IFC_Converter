using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IFCtoSTART
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("151D2EB7-E600-4129-AC34-BE89846C5BA4")]
    public class IfcImporter : IIfcImporter
    {
        public int Test()
        {
            MessageBox.Show("DLL is connected.");

            return 1;
        }
        
        public int Import(ref object startDocumentObject, int languageId)
        {
            throw new System.NotImplementedException();
        }
    }
}