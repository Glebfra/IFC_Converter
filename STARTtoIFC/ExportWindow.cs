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
            LocalizeComponents();
            
            _exportContainer = exportContainer;
            _startDocument = new StartDocument(exportContainer.StartDocumentObject);

            string inputFilepath = _startDocument.GetPathName();
            string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
            inputFilepathTextbox.Text = inputFilepath;
            outputFilepathTextbox.Text = outputFilepath;
            
            Logger.OnLogsChanged += logTextbox.AppendText;
            EventBus.OnExportFinished += ShowExportResultWindow;
        }

        private void LocalizeComponents()
        {
            Text = LocalizationResource.ExportWindowForm_Text;
            exportButton.Text = LocalizationResource.ExportButton_Text;
            inputFilepathLabel.Text = LocalizationResource.ExportWindowForm_InputFilepath_Label;
            outputFilepathLabel.Text = LocalizationResource.ExportWindowForm_OutputFilepath_Label;
            logsLabel.Text = LocalizationResource.ExportWindowForm_Log_Label;
        }

        private void ShowExportResultWindow(ConversionResult result)
        {
            //TODO: Мне кажется, мы не должны показывать сообщения о результатах конвертации. За нас это сделает старт по возвращаемому значению
            if (result == ConversionResult.Success)
            {
                DialogResult = DialogResult.OK;
                MessageBox.Show(LocalizationResource.ExportWindowForm_ShowExportResultWindow_Success, LocalizationResource.MessageBox_Title_Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (result == ConversionResult.Fail)
            {
                DialogResult = DialogResult.Abort;
                MessageBox.Show(LocalizationResource.ExportWindowForm_ShowExportResultWindow_Failure, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            //TODO: Давай вместо Filepath везде будем использовать FilePath (большая буква в середине)
            string outputFilepath = outputFilepathTextbox.Text;
            //TODO: Предлагаю еще проверять существование директории, куда собираются сохранить (можно через Directory.Exists) вместо двух ифов ниже
            //Также надо проверять наличия прав на запись в папку (можно через  System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);)
            if (string.IsNullOrEmpty(outputFilepath))
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_ExportButton_Click_NullPath, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (outputFilepath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_ExportButton_Click_InvalidPath, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            EventBus.OnExport?.Invoke(_startDocument, outputFilepath);
        }

        private void selectOutputFilepathButton_Click(object sender, EventArgs e)
        {
            //TODO: Почему не SaveFileDialog?
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = LocalizationResource.ExportWindowForm_selectOutputFilepathButton_Click_SelectFile;
                openFileDialog.Filter = @"IFC files (*.ifc)|*.ifc";
                //TODO: Предлагаю это поменять на RestoreDirectory (есть ниже)
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                //TODO: Добавляем вот это
                //saveFileDialog.DefaultExt = ".IFC";
                //saveFileDialog.RestoreDirectory = true; //Открывает последнюю открытую директорию
                //saveFileDialog.FileName = Path.GetFileNameWithoutExtension(_startFileName);

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilepathTextbox.Text = openFileDialog.FileName;
                }
            }
        }
    }
}