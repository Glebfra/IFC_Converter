using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using STARTtoIFC.Localization;
using STARTtoIFC.Tools;

namespace STARTtoIFC.GUI
{
    internal partial class ExportWindowForm : Form
    {
        private readonly ExportDataContainer _exportDataContainer;

        public ExportWindowForm(ExportDataContainer exportDataContainer)
        {
            InitializeComponent();
            LocalizeComponents();

            _exportDataContainer = exportDataContainer;

            string inputFilePath = _exportDataContainer.InputFilePath;
            string outputFilePath = inputFilePath.Replace(".ctp", ".ifc");
            outputFilePathTextbox.Text = outputFilePath;
        }

        private void LocalizeComponents()
        {
            Text = LocalizationResource.ExportWindowForm_Text;
            exportButton.Text = LocalizationResource.ExportWindowForm_ExportButton_Text;
            selectOutputFilePathButton.Text = LocalizationResource.ExportWindowForm_selectOutputFilePathButton_Text;
            outputFilePathLabel.Text = LocalizationResource.ExportWindowForm_OutputFilePath_Label_Text;
            exportTypeLabel.Text = LocalizationResource.ExportWindowForm_ExportType_Label_Text;
            vertexSegmentsLabel.Text = LocalizationResource.ExportWindowForm_VertexSegments_Textbox;

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
            if (!IsValidEmptyPath(outputFilePath)) return;

            string outputDirectoryPath = Path.GetDirectoryName(outputFilePath) ?? string.Empty;
            if (!IsValidExistDirectory(outputDirectoryPath)) return;
            if (!IsValidAccessControl(outputDirectoryPath)) return;

            int vertexNum = Convert.ToInt32(vertexSegmentsTextbox.Text);
            if (!IsValidVertexNum(vertexNum)) return;
            
            if (exportTypeCombobox.SelectedItem is not IfcExportType exportType)
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_ExportType_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            _exportDataContainer.OutputFilePath = outputFilePath;
            _exportDataContainer.ExportType = exportType.Type;
            _exportDataContainer.NumSegments = vertexNum;
            
            DialogResult = DialogResult.OK;
        }

        private bool IsValidVertexNum(int vertexNum)
        {
            bool result = vertexNum > 4;
            if (!result)
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_VertexSegmentsNum_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        private bool IsValidEmptyPath(string filePath)
        {
            bool result = !string.IsNullOrEmpty(filePath);
            if (!result)
            {
                MessageBox.Show(LocalizationResource.ExportWindowForm_OutputFilePath_Empty_Error, LocalizationResource.MessageBox_Title_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void selectOutputFilePathButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
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