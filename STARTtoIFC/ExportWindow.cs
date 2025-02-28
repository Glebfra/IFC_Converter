using System;
using System.IO;
using System.Windows.Forms;
using Start.API;

namespace STARTtoIFC
{
    internal partial class ExportWindowForm : Form
    {
        private ExportContainer _exportContainer;
        private StartDocument _startDocument;

        public ExportWindowForm(ExportContainer exportContainer)
        {
            InitializeComponent();
            _exportContainer = exportContainer;
            
            _startDocument = new StartDocument(exportContainer.StartDocumentObject);

            string inputFilepath = _startDocument.GetPathName();
            string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
            inputFilepathTextbox.Text = inputFilepath;
            outputFilepathTextbox.Text = outputFilepath;

            Logger.OnLogsChanged += logTextbox.AppendText;
            EventBus.OnExportFinished += ShowExportSuccessWindow;
        }

        private void ShowExportSuccessWindow(ConversionResult result)
        {
            if (result == ConversionResult.Success)
            {
                DialogResult = DialogResult.OK;
                MessageBox.Show("Экспорт завершен", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == ConversionResult.Fail)
            {
                DialogResult = DialogResult.Abort;
                MessageBox.Show("Экспорт не заверешен из-за внутренней ошибки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            string outputFilepath = outputFilepathTextbox.Text;
            if (string.IsNullOrEmpty(outputFilepath))
            {
                MessageBox.Show("Путь не может быть пустым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outputFilepath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                MessageBox.Show("Выберите корректное расположение файла", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            EventBus.OnExport?.Invoke(_startDocument, outputFilepath);
        }

        private void selectOutputFilepathButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = @"Выберите файл для экспорта";
                openFileDialog.Filter = @"IFC files (*.ifc)|*.ifc";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilepathTextbox.Text = openFileDialog.FileName;
                }
            }
        }
    }
}