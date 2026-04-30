using System;
using System.IO;
using System.Windows.Forms;
using IFCConverter.Localization;
using IFCConverter.Utils;

namespace IFCConverter.GUI
{
    internal partial class ImportWindowForm : Form
    {
        private readonly ImportDataContainer _dataContainer;
        
        public ImportWindowForm(ImportDataContainer dataContainer)
        {
            _dataContainer = dataContainer;
            
            InitializeComponent();
            LocalizeComponents();
        }
        
        private void LocalizeComponents()
        {
            Text = LocalizationResource.ImportWindowForm_Text;
            importButton.Text = LocalizationResource.ImportWindowForm_ImportButton_Text;
            selectInputFilePathButton.Text = LocalizationResource.ImportWindowForm_selectInputFilePathButton_Text;
            inputFilePathLabel.Text = LocalizationResource.ImportWindowForm_InputFilePath_Label_Text;

            inputFilePathTextbox.Text = _dataContainer.InputFilePath;
        }
        
        private void ImportButton_Click(object sender, EventArgs e)
        {
            string inputFilePath = inputFilePathTextbox.Text;
            if (!IsValidEmptyPath(inputFilePath)) return;
            
            string inputDirectoryPath = Path.GetDirectoryName(inputFilePath) ?? string.Empty;
            if (!IsValidExistDirectory(inputDirectoryPath)) return;
            if (!IsValidAccessControl(inputDirectoryPath)) return;

            _dataContainer.InputFilePath = inputFilePath;

            DialogResult = DialogResult.OK;
        }
        
        private bool IsValidEmptyPath(string filePath)
        {
            bool result = !string.IsNullOrEmpty(filePath);
            if (!result)
            {
                MessageBox.Show(LocalizationResource.ImportWindowForm_InputFilePath_Empty_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        private bool IsValidExistDirectory(string directoryPath)
        {
            bool result = Directory.Exists(directoryPath);
            if (!result)
            {
                MessageBox.Show(LocalizationResource.DirectoryDoesNotExists_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }
        
        private bool IsValidAccessControl(string directoryPath)
        {
            try
            {
                Directory.GetAccessControl(directoryPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(LocalizationResource.UnauthorizedAccess_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void selectInputFilePathButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = @"IFC files (*.ifc)|*.ifc";
                openFileDialog.DefaultExt = ".ifc";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    inputFilePathTextbox.Text = openFileDialog.FileName;
                }
            }
        }
    }
}