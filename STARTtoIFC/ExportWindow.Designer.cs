using System.ComponentModel;

namespace STARTtoIFC
{
    internal partial class ExportWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportWindowForm));
            this.exportButton = new System.Windows.Forms.Button();
            this.outputFilePathPanel = new System.Windows.Forms.Panel();
            this.outputFilePathTextbox = new System.Windows.Forms.TextBox();
            this.selectOutputFilePathButton = new System.Windows.Forms.Button();
            this.outputFilePathLabel = new System.Windows.Forms.Label();
            this.exportTypePanel = new System.Windows.Forms.Panel();
            this.exportTypeCombobox = new System.Windows.Forms.ComboBox();
            this.exportTypeLabel = new System.Windows.Forms.Label();
            this.vertexSegmentsPanel = new System.Windows.Forms.Panel();
            this.vertexSegmentsTextbox = new System.Windows.Forms.TextBox();
            this.vertexSegmentsLabel = new System.Windows.Forms.Label();
            this.outputFilePathPanel.SuspendLayout();
            this.exportTypePanel.SuspendLayout();
            this.vertexSegmentsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(392, 136);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(84, 23);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // outputFilePathPanel
            // 
            this.outputFilePathPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outputFilePathPanel.Controls.Add(this.outputFilePathTextbox);
            this.outputFilePathPanel.Controls.Add(this.selectOutputFilePathButton);
            this.outputFilePathPanel.Location = new System.Drawing.Point(12, 24);
            this.outputFilePathPanel.Name = "outputFilePathPanel";
            this.outputFilePathPanel.Padding = new System.Windows.Forms.Padding(5);
            this.outputFilePathPanel.Size = new System.Drawing.Size(470, 40);
            this.outputFilePathPanel.TabIndex = 1;
            // 
            // outputFilePathTextbox
            // 
            this.outputFilePathTextbox.Location = new System.Drawing.Point(8, 9);
            this.outputFilePathTextbox.Name = "outputFilePathTextbox";
            this.outputFilePathTextbox.Size = new System.Drawing.Size(365, 20);
            this.outputFilePathTextbox.TabIndex = 1;
            // 
            // selectOutputFilePathButton
            // 
            this.selectOutputFilePathButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.selectOutputFilePathButton.Location = new System.Drawing.Point(379, 7);
            this.selectOutputFilePathButton.Name = "selectOutputFilePathButton";
            this.selectOutputFilePathButton.Size = new System.Drawing.Size(84, 23);
            this.selectOutputFilePathButton.TabIndex = 2;
            this.selectOutputFilePathButton.Text = "Browse...";
            this.selectOutputFilePathButton.UseVisualStyleBackColor = true;
            this.selectOutputFilePathButton.Click += new System.EventHandler(this.selectOutputFilePathButton_Click);
            // 
            // outputFilePathLabel
            // 
            this.outputFilePathLabel.AutoSize = true;
            this.outputFilePathLabel.Location = new System.Drawing.Point(21, 18);
            this.outputFilePathLabel.Name = "outputFilePathLabel";
            this.outputFilePathLabel.Size = new System.Drawing.Size(187, 13);
            this.outputFilePathLabel.TabIndex = 3;
            this.outputFilePathLabel.Text = "Enter a full path of IFC file to be saved";
            this.outputFilePathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exportTypePanel
            // 
            this.exportTypePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exportTypePanel.Controls.Add(this.exportTypeCombobox);
            this.exportTypePanel.Location = new System.Drawing.Point(12, 90);
            this.exportTypePanel.Name = "exportTypePanel";
            this.exportTypePanel.Padding = new System.Windows.Forms.Padding(7);
            this.exportTypePanel.Size = new System.Drawing.Size(143, 40);
            this.exportTypePanel.TabIndex = 4;
            // 
            // exportTypeCombobox
            // 
            this.exportTypeCombobox.Location = new System.Drawing.Point(8, 9);
            this.exportTypeCombobox.Name = "exportTypeCombobox";
            this.exportTypeCombobox.Size = new System.Drawing.Size(127, 21);
            this.exportTypeCombobox.TabIndex = 0;
            // 
            // exportTypeLabel
            // 
            this.exportTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.exportTypeLabel.AutoSize = true;
            this.exportTypeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.exportTypeLabel.Location = new System.Drawing.Point(21, 84);
            this.exportTypeLabel.Name = "exportTypeLabel";
            this.exportTypeLabel.Size = new System.Drawing.Size(60, 13);
            this.exportTypeLabel.TabIndex = 0;
            this.exportTypeLabel.Text = "Export type";
            this.exportTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vertexSegmentsPanel
            // 
            this.vertexSegmentsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.vertexSegmentsPanel.Controls.Add(this.vertexSegmentsTextbox);
            this.vertexSegmentsPanel.Location = new System.Drawing.Point(161, 90);
            this.vertexSegmentsPanel.Name = "vertexSegmentsPanel";
            this.vertexSegmentsPanel.Size = new System.Drawing.Size(118, 40);
            this.vertexSegmentsPanel.TabIndex = 5;
            // 
            // vertexSegmentsTextbox
            // 
            this.vertexSegmentsTextbox.Location = new System.Drawing.Point(3, 10);
            this.vertexSegmentsTextbox.MaxLength = 2;
            this.vertexSegmentsTextbox.Name = "vertexSegmentsTextbox";
            this.vertexSegmentsTextbox.Size = new System.Drawing.Size(110, 20);
            this.vertexSegmentsTextbox.TabIndex = 0;
            this.vertexSegmentsTextbox.Text = "16";
            this.vertexSegmentsTextbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.vertexSegmentsTextbox_KeyPress);
            // 
            // vertexSegmentsLabel
            // 
            this.vertexSegmentsLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.vertexSegmentsLabel.AutoSize = true;
            this.vertexSegmentsLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.vertexSegmentsLabel.Location = new System.Drawing.Point(165, 84);
            this.vertexSegmentsLabel.Name = "vertexSegmentsLabel";
            this.vertexSegmentsLabel.Size = new System.Drawing.Size(85, 13);
            this.vertexSegmentsLabel.TabIndex = 6;
            this.vertexSegmentsLabel.Text = "Vertex segments\r\n";
            this.vertexSegmentsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExportWindowForm
            // 
            this.AcceptButton = this.exportButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(494, 171);
            this.Controls.Add(this.vertexSegmentsLabel);
            this.Controls.Add(this.vertexSegmentsPanel);
            this.Controls.Add(this.exportTypeLabel);
            this.Controls.Add(this.exportTypePanel);
            this.Controls.Add(this.outputFilePathLabel);
            this.Controls.Add(this.outputFilePathPanel);
            this.Controls.Add(this.exportButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(15, 15);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportWindowForm";
            this.Text = "Export to IFC";
            this.outputFilePathPanel.ResumeLayout(false);
            this.outputFilePathPanel.PerformLayout();
            this.exportTypePanel.ResumeLayout(false);
            this.vertexSegmentsPanel.ResumeLayout(false);
            this.vertexSegmentsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label vertexSegmentsLabel;

        private System.Windows.Forms.Panel vertexSegmentsPanel;

        private System.Windows.Forms.TextBox vertexSegmentsTextbox;

        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.ComboBox exportTypeCombobox;

        private System.Windows.Forms.Label exportTypeLabel;

        private System.Windows.Forms.Panel exportTypePanel;

        private System.Windows.Forms.Label outputFilePathLabel;

        private System.Windows.Forms.TextBox outputFilePathTextbox;

        private System.Windows.Forms.Button selectOutputFilePathButton;

        private System.Windows.Forms.Panel outputFilePathPanel;

        private System.Windows.Forms.Button exportButton;

        #endregion
    }
}