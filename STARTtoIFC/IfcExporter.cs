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
        public int Export(object startDocument, int languageId)
        {
            StartDocument startDocumentObject = new StartDocument(startDocument);
            string inputFilepath = startDocumentObject.GetPathName();
            string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
            
            ExportDataContainer exportDataContainer = new ExportDataContainer
            {
                LanguageId = languageId,
                InputFilepath = inputFilepath,
                OutputFilepath = outputFilepath
            };

            DialogResult dialogResult;
            using (ExportWindowForm exportWindowForm = new ExportWindowForm(exportDataContainer))
            {
                dialogResult = exportWindowForm.ShowDialog();
            }
            if (dialogResult == DialogResult.Cancel) return (int)ConversionResult.Canceled;
            
            try
            {
                IfcGenerator.Convert(startDocumentObject, exportDataContainer.OutputFilepath);
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
