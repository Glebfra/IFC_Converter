using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using IFCtoSTART.Tools;

namespace IFCtoSTART
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("079501D2-E3F2-4F69-8236-C578FB94013F")]
    public class IfcImporter : IIfcImporter
    {
        public int Test()
        {
            MessageBox.Show("DLL is connected.");

            return 1;
        }
        
        public int Import(object startDocumentObject, int languageId)
        {
            try
            {
                Application.EnableVisualStyles();
                Localize(languageId);
                
                return (int)ConversionResult.Success;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return (int)ConversionResult.Fail;
            }
        }
        
        private void Localize(int languageId)
        {
            CultureInfo ci = new CultureInfo(languageId);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}