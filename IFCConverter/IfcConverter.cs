using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using IFCConverter.Exporter;
using IFCConverter.Exporter.GUI;
using IFCConverter.Importer;
using IFCConverter.Importer.GUI;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Utils.Pipeline;
using Microsoft.Extensions.Logging;

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

                Localize(languageId);

                StartDocument startDocument = new StartDocument(startDocumentObject);
                ExportDataContainer exportDataContainer = new ExportDataContainer
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
                    return (int)ConversionResult.Canceled;

                FileLoggerProvider provider = new FileLoggerProvider(exportDataContainer.OutputFilePath + ".log");
                using (ILoggerFactory factory = new LoggerFactory())
                {
                    factory.AddProvider(provider);

                    ILogger logger = factory.CreateLogger<IfcConverter>();
                    
                    try
                    {
                        logger.LogInformation("Converting started at {DateTime}", DateTime.Now);
                        StartToIfcConverter converter = new StartToIfcConverter(exportDataContainer, factory);
                        converter.Convert(startDocument);
                        logger.LogInformation("Converting ended at {DateTime}", DateTime.Now);
                        
                        provider.Save();
                        return (int)ConversionResult.Success;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e.ToString());
                        provider.Save();
                        return (int)ConversionResult.Fail;
                    }
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
            try
            {
                Localize(languageId);

                ImportDataContainer importDataContainer = new ImportDataContainer();
                DialogResult dialogResult = ShowImportWindow(ref importDataContainer);
                if (dialogResult == DialogResult.Cancel)
                    return (int)ConversionResult.Canceled;

                using (StartAutoServer autoServer = new StartAutoServer(startAutoServerObject))
                {
                    autoServer.SaveToFile(startTempFileName);
                    using (IStartDocument startDocument = autoServer.LoadStartDocument(0x2, startTempFileName))
                    {
                        return Import(startDocument, importDataContainer);
                    }
                }
            }
            catch
            {
                return (int)ConversionResult.Fail;
            }
        }

        [STAThread]
        public int ImportFromFileOpen(object startAutoServerObject, int languageId, string startTempFileName,
            string ifcFileName)
        {
            try
            {
                Localize(languageId);

                ImportDataContainer importDataContainer = new ImportDataContainer
                {
                    InputFilePath = ifcFileName
                };
                DialogResult dialogResult = ShowImportWindow(ref importDataContainer);
                if (dialogResult == DialogResult.Cancel)
                    return (int)ConversionResult.Canceled;

                using (StartAutoServer autoServer = new StartAutoServer(startAutoServerObject))
                {
                    using (IStartDocument startDocument = autoServer.LoadStartDocument(0x4, startTempFileName))
                    {
                        return Import(startDocument, importDataContainer);
                    }
                }
            }
            catch
            {
                return (int)ConversionResult.Fail;
            }
        }

        [STAThread]
        private DialogResult ShowImportWindow(ref ImportDataContainer importDataContainer)
        {
            Application.EnableVisualStyles();

            using (ImportWindowForm importWindowForm = new ImportWindowForm(importDataContainer))
            {
                return importWindowForm.ShowDialog();
            }
        }

        private int Import(IStartDocument startDocument, ImportDataContainer importDataContainer)
        {
            FileLoggerProvider provider = new FileLoggerProvider(importDataContainer.InputFilePath + ".log");

            using (ILoggerFactory factory = new LoggerFactory())
            {
                factory.AddProvider(provider);

                ILogger logger = factory.CreateLogger<IfcConverter>();
                
                try
                {
                    logger.LogInformation("Converting start at {DateTime}", DateTime.Now);
                    IfcToStartConverter startGenerator = new IfcToStartConverter(importDataContainer, factory);
                    startGenerator.Convert(startDocument);
                    logger.LogInformation("Convert is successfully ended at {DateTime}", DateTime.Now);
                    
                    provider.Save();
                    return (int)ConversionResult.Success;
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                    provider.Save();
                    return (int)ConversionResult.Fail;
                }
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