using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using IFCConverter.Converters;
using IFCConverter.GUI;
using IFCConverter.Utils;
using Start.API;
using Utils;

namespace IFCConverter
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("BA6393A0-DE1C-4DB6-8A60-B8C66AC8B512")]
    public class IfcConverter : IIfcConverter
    {
        public int Test()
        {
            MessageBox.Show("DLL is connected.");

            return 1;
        }
        
        [STAThread]
        public int Export(object startDocumentObject, int languageId)
        {
            try
            {
                Application.EnableVisualStyles();
                Logger logger = Logger.GetInstance();
            
                Localize(languageId);

                StartDocument startDocument = new StartDocument(startDocumentObject);
                ExportDataContainer exportDataContainer = new ExportDataContainer()
                {
                    InputFilePath = startDocument.GetPathName(),
                    LanguageId = languageId
                };

                DialogResult dialogResult;
                using (ExportWindowForm exportWindowForm = new ExportWindowForm(exportDataContainer))
                {
                    dialogResult = exportWindowForm.ShowDialog();
                }

                if (dialogResult == DialogResult.Cancel)
                {
                    return (int)ConversionResult.Canceled;
                }
                
                try
                {
                    logger.System($"Converting started at {DateTime.Now}");
                    StartToIfcConverter converter = new(exportDataContainer);
                    converter.Convert(startDocument);
                    logger.System($"Converting ended at {DateTime.Now}");
                    
                    #if DEBUG
                    logger.SaveAs(exportDataContainer.OutputFilePath + ".log");
                    #else
                    if (logger.HasErrors())
                    {
                        logger.SaveAs(exportDataContainer.OutputFilePath + ".log");
                    }
                    else
                    {
                        logger.Flush();
                    }
                    #endif
                    
                    return (int)ConversionResult.Success;
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                    logger.SaveAs(exportDataContainer.OutputFilePath + ".log");
                    return (int)ConversionResult.Fail;
                }
            } 
            catch (Exception)
            {
                return (int)ConversionResult.Fail;
            }
        }

        [STAThread]
        public int ImportFromFileImport(object startAutoServerObject, int languageId, string startTempFileName)
        {
            throw new NotImplementedException();
        }

        [STAThread]
        public int ImportFromFileOpen(object startAutoServerObject, int languageId, string startTempFileName, string ifcFileName)
        {
            throw new NotImplementedException();
        }

        private void Localize(int languageId)
        {
            CultureInfo ci = new CultureInfo(languageId);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}