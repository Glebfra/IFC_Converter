using System;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace STARTtoIFC
{
    internal partial class ExportWindowForm : Form
    {
        private readonly DataContainer _dataContainer;

        public ExportWindowForm(DataContainer dataContainer)
        {
            InitializeComponent();
            LocalizeComponents();
            
            _dataContainer = dataContainer;

            string inputFilePath = _dataContainer.InputFilePath;
            string outputFilePath = inputFilePath.Replace(".ctp", ".ifc");
            outputFilePathTextbox.Text = outputFilePath;
        }

        private void LocalizeComponents()
        {
            Text = LocalizationResource.ExportWindowForm_Text;
            exportButton.Text = LocalizationResource.ExportWindowForm_ExportButton_Text;
            selectOutputFilePathButton.Text = LocalizationResource.ExportWindowForm_selectOutputFilePathButton_Text;
            outputFilePathLabel.Text = LocalizationResource.ExportWindowForm_OutputFilepath_Label;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            string outputFilePath = outputFilePathTextbox.Text;
            if (string.IsNullOrEmpty(outputFilePath))
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_outputFilePath_Empty_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string outputDirectoryPath = Path.GetDirectoryName(outputFilePath) ?? string.Empty;
            if (!Directory.Exists(outputDirectoryPath))
            {
                MessageBox.Show(LocalizationResource.DirectoryDoesNotExists_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DirectorySecurity ds = Directory.GetAccessControl(outputDirectoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(LocalizationResource.UnauthorizedAccess_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _dataContainer.OutputFilePath = outputFilePath;
            DialogResult = DialogResult.OK;
        }

        private void selectOutputFilePathButton_Click(object sender, EventArgs e)
        {
            //TODO: Ты не реализовал через SaveFileDialog, а оставил OpenFileDialog. Давай поменяем?
            using (OpenFileDialog saveFileDialog = new OpenFileDialog())
            {
                saveFileDialog.Title = LocalizationResource.ExportWindowForm_selectOutputFilepathButton_Click_SelectFile;
                saveFileDialog.Filter = @"IFC files (*.ifc)|*.ifc";
                saveFileDialog.DefaultExt = ".IFC";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilePathTextbox.Text = saveFileDialog.FileName;
                }
            }
        }
    }
}