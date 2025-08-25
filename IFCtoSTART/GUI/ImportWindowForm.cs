using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using IFCtoSTART.Localization;
using IFCtoSTART.Tools;

namespace IFCtoSTART.GUI
{
    internal partial class ImportWindowForm : Form
    {
        private readonly DataContainer _dataContainer;
        
        public ImportWindowForm(DataContainer dataContainer)
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

            importTypeLabel.Text = LocalizationResource.ImportWindowForm_ImportType_Label_Text;
            ArrayList types = new ArrayList
            {
                new ImportType(ImportTypeEnum.AUTO, LocalizationResource.ImportWindowForm_ImportType_Auto),
                new ImportType(ImportTypeEnum.STANDARD, LocalizationResource.ImportWindowForm_ImportType_Standard),
                new ImportType(ImportTypeEnum.START, LocalizationResource.ImportWindowForm_ImportType_Start),
            };
            importTypeCombobox.DataSource = types;
            importTypeCombobox.DisplayMember = "TypeName";
            importTypeCombobox.ValueMember = "Type";
            importTypeCombobox.SelectedItem = types[0];
        }
        
        private void ImportButton_Click(object sender, EventArgs e)
        {
            string inputFilePath = inputFilePathTextbox.Text;
            if (!IsValidEmptyPath(inputFilePath)) return;
            
            string inputDirectoryPath = Path.GetDirectoryName(inputFilePath) ?? string.Empty;
            if (!IsValidExistDirectory(inputDirectoryPath)) return;
            if (!IsValidAccessControl(inputDirectoryPath)) return;
            
            if (importTypeCombobox.SelectedItem is not ImportType importType)
            {
                MessageBox.Show(LocalizationResource.ImportWindowForm_ImportType_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _dataContainer.InputFilePath = inputFilePath;
            _dataContainer.ImportTypeEnum = importType.Type;

            DialogResult = DialogResult.OK;
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
    }
}