using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Start.API;

namespace STARTtoIFC
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("8023137E-9E17-41A7-96AE-7D7688F1EC14")]
    public class IfcExporter : IIfcExporter
    {
        private Action<StartDocument, string, Action<ConversionResult>?>? OnExport;
        private Action<ConversionResult>? OnExportFinished;

        public int Test()
        {
            MessageBox.Show("DLL is connected.");

            return 1;
        }
        
        [STAThread]
        public int Export(object startDocument, int languageId)
        {
            ExportContainer exportContainer = new ExportContainer()
            {
                StartDocumentObject = startDocument,
                LanguageId = languageId
            };
            EventBus.OnExport += IfcGenerator.Convert;

            DialogResult dialogResult;
            using (ExportWindowForm exportWindowForm = new ExportWindowForm(exportContainer))
            {
                dialogResult = exportWindowForm.ShowDialog();
            }

            if (dialogResult == DialogResult.OK)
            {
                return (int)ConversionResult.Success;
            }
            else
            {
                return (int)ConversionResult.Canceled;
            }
        }

        private void Localize(int languageId)
        {
            var ci = new System.Globalization.CultureInfo(languageId);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
