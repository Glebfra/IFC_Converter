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
            this.outputFilePathTextbox = new System.Windows.Forms.TextBox();
            this.selectOutputFilepathButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(398, 62);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(84, 23);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // outputFilePathTextbox
            // 
            this.outputFilePathTextbox.Location = new System.Drawing.Point(26, 14);
            this.outputFilePathTextbox.Name = "outputFilePathTextbox";
            this.outputFilePathTextbox.Size = new System.Drawing.Size(366, 20);
            this.outputFilePathTextbox.TabIndex = 1;
            // 
            // selectOutputFilepathButton
            // 
            this.selectOutputFilepathButton.Location = new System.Drawing.Point(398, 12);
            this.selectOutputFilepathButton.Name = "selectOutputFilepathButton";
            this.selectOutputFilepathButton.Size = new System.Drawing.Size(84, 23);
            this.selectOutputFilepathButton.TabIndex = 2;
            this.selectOutputFilepathButton.Text = "Browse...";
            this.selectOutputFilepathButton.UseVisualStyleBackColor = true;
            this.selectOutputFilepathButton.Click += new System.EventHandler(this.selectOutputFilePathButton_Click);
            // 
            // ExportWindowForm
            // 
            this.AcceptButton = this.exportButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 97);
            this.Controls.Add(this.selectOutputFilepathButton);
            this.Controls.Add(this.outputFilePathTextbox);
            this.Controls.Add(this.exportButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExportWindowForm";
            this.Text = "ExportWindow";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox outputFilePathTextbox;
        private System.Windows.Forms.Button selectOutputFilepathButton;

        private System.Windows.Forms.Button exportButton;

        #endregion
    }
}