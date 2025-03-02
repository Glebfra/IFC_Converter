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
        //TODO: Не используется? Давай уберём
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
            //TODO: Давай это назовём иначе класс и переменную. ExportContainer не очень нравится. Можно что-то вроде dataContainer, startData, startDTO (на выбор)
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

            switch (dialogResult)
            {
                case DialogResult.OK:
                    return (int)ConversionResult.Success;
                case DialogResult.Cancel:
                    return (int)ConversionResult.Canceled;
                default:
                    return (int)ConversionResult.Fail;
            }
        }

        //TODO: С помощью этого метода можно устанавливать культуру потока. номер культуры мы будем получать от старта.
        //Если ты не хочешь устанавливать культуру здесь, то унеси метод в то место, где будешь использовать
        private void Localize(int languageId)
        {
            var ci = new System.Globalization.CultureInfo(languageId);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}
