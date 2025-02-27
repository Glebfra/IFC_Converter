using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace STARTtoIFC
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("8023137E-9E17-41A7-96AE-7D7688F1EC14")]
    public class IfcExporter : IIfcExporter
    {
        public int Test()
        {
            MessageBox.Show("DLL is connected.");

            return 1;
        }

        public int Export(object startDocument, int languageId)
        {
            MessageBox.Show("Здесь будет окно эксопрта в IFC");

            if (startDocument != null)
                Marshal.ReleaseComObject(startDocument);
            startDocument = null;

            return (int)ConversionResult.Success;
        }

        private void Localize(int languageId)
        {
            var ci = new System.Globalization.CultureInfo(languageId);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
