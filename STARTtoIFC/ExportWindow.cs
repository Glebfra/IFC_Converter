using System;
using System.IO;
using System.Windows.Forms;

namespace STARTtoIFC
{
    public partial class ExportWindowForm : Form
    {
        private ExportDataContainer _exportDataContainer;
        
        public ExportWindowForm(ExportDataContainer exportDataContainer)
        {
            InitializeComponent();
            
            _exportDataContainer = exportDataContainer;
            
            inputFilepathTextbox.Text = _exportDataContainer.InputFilepath;
            outputFilepathTextbox.Text = _exportDataContainer.OutputFilepath;
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

            _exportDataContainer.OutputFilepath = outputFilepath;
            DialogResult = DialogResult.OK;
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