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
            this.exportButton = new System.Windows.Forms.Button();
            this.inputFilepathTextbox = new System.Windows.Forms.TextBox();
            this.outputFilepathTextbox = new System.Windows.Forms.TextBox();
            this.selectOutputFilepathButton = new System.Windows.Forms.Button();
            this.inputFilepathLabel = new System.Windows.Forms.Label();
            this.outputFilepathLabel = new System.Windows.Forms.Label();
            this.logTextbox = new System.Windows.Forms.TextBox();
            this.logsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.exportButton.Location = new System.Drawing.Point(138, 269);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(106, 38);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Экспорт";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // inputFilepathTextbox
            // 
            this.inputFilepathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.inputFilepathTextbox.Location = new System.Drawing.Point(98, 12);
            this.inputFilepathTextbox.Name = "inputFilepathTextbox";
            this.inputFilepathTextbox.ReadOnly = true;
            this.inputFilepathTextbox.Size = new System.Drawing.Size(282, 20);
            this.inputFilepathTextbox.TabIndex = 1;
            // 
            // outputFilepathTextbox
            // 
            this.outputFilepathTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.outputFilepathTextbox.Location = new System.Drawing.Point(98, 51);
            this.outputFilepathTextbox.Name = "outputFilepathTextbox";
            this.outputFilepathTextbox.Size = new System.Drawing.Size(282, 20);
            this.outputFilepathTextbox.TabIndex = 3;
            // 
            // selectOutputFilepathButton
            // 
            this.selectOutputFilepathButton.Location = new System.Drawing.Point(351, 51);
            this.selectOutputFilepathButton.Name = "selectOutputFilepathButton";
            this.selectOutputFilepathButton.Size = new System.Drawing.Size(29, 20);
            this.selectOutputFilepathButton.TabIndex = 4;
            this.selectOutputFilepathButton.Text = "...";
            this.selectOutputFilepathButton.UseVisualStyleBackColor = true;
            this.selectOutputFilepathButton.Click += new System.EventHandler(this.selectOutputFilepathButton_Click);
            // 
            // inputFilepathLabel
            // 
            this.inputFilepathLabel.Location = new System.Drawing.Point(24, 12);
            this.inputFilepathLabel.Name = "inputFilepathLabel";
            this.inputFilepathLabel.Size = new System.Drawing.Size(68, 20);
            this.inputFilepathLabel.TabIndex = 5;
            this.inputFilepathLabel.Text = "Input filepath";
            this.inputFilepathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // outputFilepathLabel
            // 
            this.outputFilepathLabel.Location = new System.Drawing.Point(16, 51);
            this.outputFilepathLabel.Name = "outputFilepathLabel";
            this.outputFilepathLabel.Size = new System.Drawing.Size(76, 20);
            this.outputFilepathLabel.TabIndex = 6;
            this.outputFilepathLabel.Text = "Output filepath";
            this.outputFilepathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // logTextbox
            // 
            this.logTextbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextbox.Location = new System.Drawing.Point(12, 120);
            this.logTextbox.Multiline = true;
            this.logTextbox.Name = "logTextbox";
            this.logTextbox.ReadOnly = true;
            this.logTextbox.Size = new System.Drawing.Size(366, 143);
            this.logTextbox.TabIndex = 7;
            // 
            // logsLabel
            // 
            this.logsLabel.Location = new System.Drawing.Point(12, 94);
            this.logsLabel.Name = "logsLabel";
            this.logsLabel.Size = new System.Drawing.Size(366, 23);
            this.logsLabel.TabIndex = 8;
            this.logsLabel.Text = "Logs";
            this.logsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ExportWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 319);
            this.Controls.Add(this.logsLabel);
            this.Controls.Add(this.logTextbox);
            this.Controls.Add(this.outputFilepathLabel);
            this.Controls.Add(this.inputFilepathLabel);
            this.Controls.Add(this.selectOutputFilepathButton);
            this.Controls.Add(this.outputFilepathTextbox);
            this.Controls.Add(this.inputFilepathTextbox);
            this.Controls.Add(this.exportButton);
            this.Name = "ExportWindowForm";
            this.Text = "ExportWindow";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label logsLabel;

        private System.Windows.Forms.TextBox logTextbox;

        private System.Windows.Forms.Label outputFilepathLabel;

        private System.Windows.Forms.Label inputFilepathLabel;

        private System.Windows.Forms.TextBox outputFilepathTextbox;
        
        private System.Windows.Forms.TextBox inputFilepathTextbox;
        
        private System.Windows.Forms.Button selectOutputFilepathButton;

        private System.Windows.Forms.Button exportButton;

        #endregion
    }
}