using System;
using System.IO;
using System.Windows.Forms;
using IFCConverter.Exporter.GUI.Localization;
using IFCConverter.Utils.Pipeline;

namespace IFCConverter.Exporter.GUI
{
    public partial class ExportWindowForm : Form
    {
        private readonly ExportDataContainer _exportDataContainer;

        public ExportWindowForm(ExportDataContainer exportDataContainer)
        {
            InitializeComponent();

            _exportDataContainer = exportDataContainer;

            string inputFilePath = _exportDataContainer.InputFilePath;
            string outputFilePath = inputFilePath.Replace(".ctp", ".ifc");
            outputFilePathTextbox.Text = outputFilePath;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            string outputFilePath = outputFilePathTextbox.Text;
            if (!IsValidEmptyPath(outputFilePath)) return;

            string outputDirectoryPath = Path.GetDirectoryName(outputFilePath) ?? string.Empty;
            if (!IsValidExistDirectory(outputDirectoryPath)) return;
            if (!IsValidAccessControl(outputDirectoryPath)) return;

            _exportDataContainer.OutputFilePath = outputFilePath;

            DialogResult = DialogResult.OK;
        }

        private static bool IsValidEmptyPath(string filePath)
        {
            bool result = !string.IsNullOrEmpty(filePath);
            if (!result)
                MessageBox.Show(
                    LocalizationResource.ExportWindowForm_OutputFilePath_Empty_Error,
                    LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error
                );

            return result;
        }

        private static bool IsValidExistDirectory(string directoryPath)
        {
            bool result = Directory.Exists(directoryPath);
            if (!result)
                MessageBox.Show(
                    LocalizationResource.DirectoryDoesNotExists_Error,
                    LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error
                );

            return result;
        }

        private static bool IsValidAccessControl(string directoryPath)
        {
            try
            {
                Directory.GetAccessControl(directoryPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    LocalizationResource.UnauthorizedAccess_Error,
                    LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                return false;
            }
        }

        private void selectOutputFilePathButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                //saveFileDialog.FileName = Имя файла старт;
                saveFileDialog.Title = LocalizationResource.ExportWindowForm_SaveDialogFile_Title;
                saveFileDialog.Filter = @"IFCConverter.IFC files (*.ifc)|*.ifc";
                saveFileDialog.DefaultExt = ".ifc";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    outputFilePathTextbox.Text = saveFileDialog.FileName;
            }
        }
    }
}