using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Start.API;
using STARTtoIFC.GUI;
using STARTtoIFC.Tools;

namespace STARTtoIFC
{
    [Obsolete("Use IfcConverter instead")]
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
        
        [STAThread]
        public int Import(object startDocumentObject, int languageId)
        {
            try
            {
                Application.EnableVisualStyles();
                Logger logger = Logger.GetInstance();
                
                Localize(languageId);
                
                StartDocument startDocument = new StartDocument(startDocumentObject);
                ImportDataContainer dataContainer = new ImportDataContainer();

                DialogResult dialogResult;
                using (ImportWindowForm importWindowForm = new ImportWindowForm(dataContainer))
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
                    StartGenerator startGenerator = new StartGenerator(dataContainer);
                    startGenerator.Convert(startDocument);
                    logger.Info($"Convert is successfully ended at {DateTime.Now}");
                    
                    if (logger.HasErrors())
                    {
                        logger.SaveAs(dataContainer.InputFilePath + ".log");
                    }
                    else
                    {
                        logger.Flush();
                    }
                    return (int)ConversionResult.Success;
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    logger.SaveAs(dataContainer.InputFilePath + ".log");
                    return (int)ConversionResult.Fail;
                }
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