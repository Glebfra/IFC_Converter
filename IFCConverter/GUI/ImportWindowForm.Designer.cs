using System.ComponentModel;

namespace IFCConverter.GUI
{
    internal partial class ImportWindowForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportWindowForm));
            this.inputFilePathPanel = new System.Windows.Forms.Panel();
            this.inputFilePathTextbox = new System.Windows.Forms.TextBox();
            this.selectInputFilePathButton = new System.Windows.Forms.Button();
            this.inputFilePathLabel = new System.Windows.Forms.Label();
            this.importButton = new System.Windows.Forms.Button();
            this.inputFilePathPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputFilePathPanel
            // 
            this.inputFilePathPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputFilePathPanel.Controls.Add(this.inputFilePathTextbox);
            this.inputFilePathPanel.Controls.Add(this.selectInputFilePathButton);
            resources.ApplyResources(this.inputFilePathPanel, "inputFilePathPanel");
            this.inputFilePathPanel.Name = "inputFilePathPanel";
            // 
            // inputFilePathTextbox
            // 
            resources.ApplyResources(this.inputFilePathTextbox, "inputFilePathTextbox");
            this.inputFilePathTextbox.Name = "inputFilePathTextbox";
            // 
            // selectInputFilePathButton
            // 
            resources.ApplyResources(this.selectInputFilePathButton, "selectInputFilePathButton");
            this.selectInputFilePathButton.Name = "selectInputFilePathButton";
            this.selectInputFilePathButton.UseVisualStyleBackColor = true;
            this.selectInputFilePathButton.Click += new System.EventHandler(this.selectInputFilePathButton_Click);
            // 
            // inputFilePathLabel
            // 
            resources.ApplyResources(this.inputFilePathLabel, "inputFilePathLabel");
            this.inputFilePathLabel.Name = "inputFilePathLabel";
            // 
            // importButton
            // 
            resources.ApplyResources(this.importButton, "importButton");
            this.importButton.Name = "importButton";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // ImportWindowForm
            // 
            this.AcceptButton = this.importButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.inputFilePathLabel);
            this.Controls.Add(this.inputFilePathPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportWindowForm";
            this.inputFilePathPanel.ResumeLayout(false);
            this.inputFilePathPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button importButton;

        private System.Windows.Forms.Label inputFilePathLabel;

        private System.Windows.Forms.Panel inputFilePathPanel;
        private System.Windows.Forms.TextBox inputFilePathTextbox;
        private System.Windows.Forms.Button selectInputFilePathButton;

        #endregion
    }
}