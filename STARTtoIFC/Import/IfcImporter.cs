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
                    StartGenerator startGenerator = new StartGenerator(dataContainer);
                    startGenerator.Convert(startDocument);
                    
                    return (int)ConversionResult.Success;
                }
                catch (Exception e)
                {
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