using System.ComponentModel;

namespace IFCtoSTART.GUI
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
            this.importTypePanel = new System.Windows.Forms.Panel();
            this.importTypeCombobox = new System.Windows.Forms.ComboBox();
            this.importTypeLabel = new System.Windows.Forms.Label();
            this.inputFilePathPanel.SuspendLayout();
            this.importTypePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputFilePathPanel
            // 
            this.inputFilePathPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inputFilePathPanel.Controls.Add(this.inputFilePathTextbox);
            this.inputFilePathPanel.Controls.Add(this.selectInputFilePathButton);
            this.inputFilePathPanel.Location = new System.Drawing.Point(12, 24);
            this.inputFilePathPanel.Name = "inputFilePathPanel";
            this.inputFilePathPanel.Padding = new System.Windows.Forms.Padding(5);
            this.inputFilePathPanel.Size = new System.Drawing.Size(470, 40);
            this.inputFilePathPanel.TabIndex = 2;
            // 
            // inputFilePathTextbox
            // 
            this.inputFilePathTextbox.Location = new System.Drawing.Point(8, 9);
            this.inputFilePathTextbox.Name = "inputFilePathTextbox";
            this.inputFilePathTextbox.Size = new System.Drawing.Size(365, 20);
            this.inputFilePathTextbox.TabIndex = 1;
            // 
            // selectInputFilePathButton
            // 
            this.selectInputFilePathButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.selectInputFilePathButton.Location = new System.Drawing.Point(379, 7);
            this.selectInputFilePathButton.Name = "selectInputFilePathButton";
            this.selectInputFilePathButton.Size = new System.Drawing.Size(84, 23);
            this.selectInputFilePathButton.TabIndex = 2;
            this.selectInputFilePathButton.Text = "Browse...";
            this.selectInputFilePathButton.UseVisualStyleBackColor = true;
            this.selectInputFilePathButton.Click += new System.EventHandler(this.selectInputFilePathButton_Click);
            // 
            // inputFilePathLabel
            // 
            this.inputFilePathLabel.AutoSize = true;
            this.inputFilePathLabel.Location = new System.Drawing.Point(21, 18);
            this.inputFilePathLabel.Name = "inputFilePathLabel";
            this.inputFilePathLabel.Size = new System.Drawing.Size(187, 13);
            this.inputFilePathLabel.TabIndex = 4;
            this.inputFilePathLabel.Text = "Enter a full path of IFC file to be saved";
            this.inputFilePathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // importButton
            // 
            this.importButton.Location = new System.Drawing.Point(388, 126);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(84, 23);
            this.importButton.TabIndex = 5;
            this.importButton.Text = "Import";
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButton_Click);
            // 
            // importTypePanel
            // 
            this.importTypePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.importTypePanel.Controls.Add(this.importTypeCombobox);
            this.importTypePanel.Location = new System.Drawing.Point(12, 90);
            this.importTypePanel.Name = "importTypePanel";
            this.importTypePanel.Padding = new System.Windows.Forms.Padding(7);
            this.importTypePanel.Size = new System.Drawing.Size(143, 40);
            this.importTypePanel.TabIndex = 6;
            // 
            // importTypeCombobox
            // 
            this.importTypeCombobox.Location = new System.Drawing.Point(8, 9);
            this.importTypeCombobox.Name = "importTypeCombobox";
            this.importTypeCombobox.Size = new System.Drawing.Size(127, 21);
            this.importTypeCombobox.TabIndex = 0;
            // 
            // importTypeLabel
            // 
            this.importTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.importTypeLabel.AutoSize = true;
            this.importTypeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.importTypeLabel.Location = new System.Drawing.Point(26, 84);
            this.importTypeLabel.Name = "importTypeLabel";
            this.importTypeLabel.Size = new System.Drawing.Size(63, 13);
            this.importTypeLabel.TabIndex = 7;
            this.importTypeLabel.Text = "Import Type";
            this.importTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ImportWindowForm
            // 
            this.AcceptButton = this.importButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 161);
            this.Controls.Add(this.importTypeLabel);
            this.Controls.Add(this.importTypePanel);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.inputFilePathLabel);
            this.Controls.Add(this.inputFilePathPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportWindowForm";
            this.Text = "Import from IFC";
            this.inputFilePathPanel.ResumeLayout(false);
            this.inputFilePathPanel.PerformLayout();
            this.importTypePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label importTypeLabel;

        private System.Windows.Forms.Panel importTypePanel;
        private System.Windows.Forms.ComboBox importTypeCombobox;

        private System.Windows.Forms.Button importButton;

        private System.Windows.Forms.Label inputFilePathLabel;

        private System.Windows.Forms.Panel inputFilePathPanel;
        private System.Windows.Forms.TextBox inputFilePathTextbox;
        private System.Windows.Forms.Button selectInputFilePathButton;

        #endregion
    }
}