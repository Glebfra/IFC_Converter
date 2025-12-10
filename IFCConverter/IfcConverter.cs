using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using IFCConverter.GUI;
using IFCConverter.Tools;
using Start.API;

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
                    logger.Info($"Converting start at {DateTime.Now}");
                    IfcGenerator ifcGenerator = new IfcGenerator(exportDataContainer);
                    ifcGenerator.Convert(startDocument);
                    logger.Info($"Convert is successfully ended at {DateTime.Now}");
                    
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
                catch (Exception e)
                {
                    logger.Error(e.ToString());
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
            return ImportFromFileOpen(startAutoServerObject, languageId, startTempFileName);
        }

        [STAThread]
        public int ImportFromFileOpen(object startAutoServerObject, int languageId, string startTempFileName, string ifcFileName="")
        {
            Logger logger = Logger.GetInstance();
            Localize(languageId);

            using (StartAutoServer autoServer = new StartAutoServer(startAutoServerObject))
            {
                try
                {
                    StartDocument startDocument = autoServer.LoadStartDocument(0x2, startTempFileName);
                    ImportDataContainer importDataContainer = new ImportDataContainer() { InputFilePath = ifcFileName };
                    return Import(startDocument, importDataContainer);
                }
                catch (Exception e)
                {
                    return (int)ConversionResult.Fail;
                }
            }
        }

        [STAThread]
        private int Import(StartDocument startDocument, ImportDataContainer importDataContainer)
        {
            Application.EnableVisualStyles();
            Logger logger = Logger.GetInstance();
            
            DialogResult dialogResult;
            using (ImportWindowForm importWindowForm = new ImportWindowForm(importDataContainer))
            {
                dialogResult = importWindowForm.ShowDialog();
            }
                
            if (dialogResult == DialogResult.Cancel)
            {
                return (int)ConversionResult.Canceled;
            }
            
            try
            {
                logger.Info($"Converting start at {DateTime.Now}");
                StartGenerator startGenerator = new StartGenerator(importDataContainer);
                startGenerator.Convert(startDocument);
                logger.Info($"Convert is successfully ended at {DateTime.Now}");
                    
                #if DEBUG
                logger.SaveAs(importDataContainer.InputFilePath + ".log");
                #else
                if (logger.HasErrors())
                {
                    logger.SaveAs(importDataContainer.InputFilePath + ".log");
                }
                else
                {
                    logger.Flush();
                }
                #endif
                    
                return (int)ConversionResult.Success;
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                logger.SaveAs(importDataContainer.InputFilePath + ".log");
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