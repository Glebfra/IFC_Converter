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
        public int Test()
        {
            MessageBox.Show("DLL is connected.");

            return 1;
        }
        
        [STAThread]
        public int Export(object startDocumentObject, int languageId)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Localize(languageId);
            
            StartDocument startDocument = new StartDocument(startDocumentObject);
            DataContainer dataContainer = new DataContainer()
            {
                InputFilePath = startDocument.GetPathName(),
                LanguageId = languageId
            };
            
            DialogResult dialogResult;
            using (ExportWindowForm exportWindowForm = new ExportWindowForm(dataContainer))
            {
                dialogResult = exportWindowForm.ShowDialog();
            }

            if (dialogResult == DialogResult.Cancel)
                return (int)ConversionResult.Canceled;

            try
            {
                IfcGenerator.Convert(startDocument, dataContainer.OutputFilePath);
                return (int)ConversionResult.Success;
            }
            catch (Exception e)
            {
                return (int)ConversionResult.Fail;
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
