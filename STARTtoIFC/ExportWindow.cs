using System;
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
            _exportDataContainer.InputFilepath = inputFilepathTextbox.Text;
            _exportDataContainer.OutputFilepath = outputFilepathTextbox.Text;
            
            Close();
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