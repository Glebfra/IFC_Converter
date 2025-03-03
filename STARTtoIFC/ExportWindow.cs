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
            exportButton.Text = LocalizationResource.ExportButton_Text;
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            string outputFilePath = outputFilePathTextbox.Text;
            string outputDirectoryPath = Path.GetDirectoryName(outputFilePath) ?? string.Empty;
            if (!Directory.Exists(outputDirectoryPath))
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_DirectoryDoesNotExists_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DirectorySecurity ds = Directory.GetAccessControl(outputDirectoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_UnauthorizedAccess_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _dataContainer.OutputFilePath = outputFilePath;
            DialogResult = DialogResult.OK;
        }

        private void selectOutputFilePathButton_Click(object sender, EventArgs e)
        {
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