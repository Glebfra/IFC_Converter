using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using STARTtoIFC.Localization;

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
            outputFilePathLabel.Text = LocalizationResource.ExportWindowForm_OutputFilePath_Label;
            exportTypeLabel.Text = LocalizationResource.ExportWindowForm_ExportType_Label;
            
            ArrayList types = new ArrayList
            {
                new IfcExportType(IfcExportTypeEnum.VERTEX, LocalizationResource.ExportWindowForm_ExportType_Vertex),
                new IfcExportType(IfcExportTypeEnum.CAD, LocalizationResource.ExportWindowForm_ExportType_Topological)
            };
            exportTypeCombobox.DataSource = types;
            exportTypeCombobox.DisplayMember = "TypeName";
            exportTypeCombobox.ValueMember = "Type";
            exportTypeCombobox.SelectedItem = types[1];
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            string outputFilePath = outputFilePathTextbox.Text;
            CheckEmptyPath(outputFilePath);

            string outputDirectoryPath = Path.GetDirectoryName(outputFilePath) ?? string.Empty;
            CheckExistDirectory(outputDirectoryPath);
            CheckAccessControl(outputDirectoryPath);
            
            _dataContainer.OutputFilePath = outputFilePath;
            
            if (exportTypeCombobox.SelectedItem is not IfcExportType exportType)
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_ExportType_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _dataContainer.ExportType = exportType.Type;
            _dataContainer.NumSegments = Convert.ToInt32(vertexSegmentsTextbox.Text);
            DialogResult = DialogResult.OK;
        }

        private void CheckEmptyPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_OutputFilePath_Empty_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckExistDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                MessageBox.Show(LocalizationResource.DirectoryDoesNotExists_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckAccessControl(string directoryPath)
        {
            try
            {
                Directory.GetAccessControl(directoryPath);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(LocalizationResource.UnauthorizedAccess_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void selectOutputFilePathButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // TODO: Предлагаю вообще убрать Title(он автоматически задаётся с учётом локализации) и добавить это:
                //saveFileDialog.FileName = Имя файла старт;
                saveFileDialog.Title = LocalizationResource.ExportWindowForm_SaveDialogFile_Title;
                saveFileDialog.Filter = @"IFC files (*.ifc)|*.ifc";
                saveFileDialog.DefaultExt = ".ifc";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    outputFilePathTextbox.Text = saveFileDialog.FileName;
                }
            }
        }

        private void vertexSegmentsTextbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}