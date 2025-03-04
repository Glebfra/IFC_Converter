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
            this.panel1 = new System.Windows.Forms.Panel();
            this.outputFilePathTextbox = new System.Windows.Forms.TextBox();
            this.selectOutputFilePathButton = new System.Windows.Forms.Button();
            this.outputFilePathLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(392, 83);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(84, 23);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Export";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.outputFilePathTextbox);
            this.panel1.Controls.Add(this.selectOutputFilePathButton);
            this.panel1.Location = new System.Drawing.Point(12, 24);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(470, 40);
            this.panel1.TabIndex = 1;
            // 
            // outputFilePathTextbox
            // 
            this.outputFilePathTextbox.Location = new System.Drawing.Point(8, 9);
            this.outputFilePathTextbox.Name = "outputFilePathTextbox";
            this.outputFilePathTextbox.Size = new System.Drawing.Size(365, 20);
            this.outputFilePathTextbox.TabIndex = 2;
            // 
            // selectOutputFilePathButton
            // 
            this.selectOutputFilePathButton.Location = new System.Drawing.Point(379, 7);
            this.selectOutputFilePathButton.Name = "selectOutputFilePathButton";
            this.selectOutputFilePathButton.Size = new System.Drawing.Size(84, 23);
            this.selectOutputFilePathButton.TabIndex = 1;
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
            // ExportWindowForm
            // 
            this.AcceptButton = this.exportButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 118);
            this.Controls.Add(this.outputFilePathLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.exportButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportWindowForm";
            this.Text = "ExportWindow";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label outputFilePathLabel;

        private System.Windows.Forms.TextBox outputFilePathTextbox;

        private System.Windows.Forms.Button selectOutputFilePathButton;

        private System.Windows.Forms.Panel panel1;

        private System.Windows.Forms.Button exportButton;

        #endregion
    }
}