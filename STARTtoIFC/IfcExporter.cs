using System;
using System.Globalization;
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
            Logger logger = Logger.GetInstance();
            
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
            {
                return (int)ConversionResult.Canceled;
            }
            
            try
            {
                logger.Info($"Converting start at {DateTime.Now}");
                IfcGenerator.Convert(startDocument, dataContainer.OutputFilePath);
                logger.Info($"Convert is successfully ended at {DateTime.Now}");
                logger.Flush();
                return (int)ConversionResult.Success;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                logger.SaveAs(dataContainer.OutputFilePath + ".log");
                return (int)ConversionResult.Fail;
            }
        }

        private void Localize(int languageId)
        {
            int convertedLanguageId = LanguageConverter.ConvertLanguage(languageId);
            
            var ci = new CultureInfo(convertedLanguageId);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
