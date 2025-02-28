using System.ComponentModel;

namespace STARTtoIFC
{
    partial class ExportWindowForm
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
            this.inputFilepathTextboxName = new System.Windows.Forms.TextBox();
            this.outputFilepathTextboxName = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.exportButton.Location = new System.Drawing.Point(159, 102);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(106, 38);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Экспорт";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // inputFilepathTextbox
            // 
            this.inputFilepathTextbox.Location = new System.Drawing.Point(130, 11);
            this.inputFilepathTextbox.Name = "inputFilepathTextbox";
            this.inputFilepathTextbox.ReadOnly = true;
            this.inputFilepathTextbox.Size = new System.Drawing.Size(282, 20);
            this.inputFilepathTextbox.TabIndex = 1;
            // 
            // outputFilepathTextbox
            // 
            this.outputFilepathTextbox.Location = new System.Drawing.Point(130, 50);
            this.outputFilepathTextbox.Name = "outputFilepathTextbox";
            this.outputFilepathTextbox.Size = new System.Drawing.Size(282, 20);
            this.outputFilepathTextbox.TabIndex = 3;
            // 
            // selectOutputFilepathButton
            // 
            this.selectOutputFilepathButton.Location = new System.Drawing.Point(383, 50);
            this.selectOutputFilepathButton.Name = "selectOutputFilepathButton";
            this.selectOutputFilepathButton.Size = new System.Drawing.Size(29, 20);
            this.selectOutputFilepathButton.TabIndex = 4;
            this.selectOutputFilepathButton.Text = "...";
            this.selectOutputFilepathButton.UseVisualStyleBackColor = true;
            this.selectOutputFilepathButton.Click += new System.EventHandler(this.selectOutputFilepathButton_Click);
            // 
            // inputFilepathTextboxName
            // 
            this.inputFilepathTextboxName.Location = new System.Drawing.Point(12, 11);
            this.inputFilepathTextboxName.Name = "inputFilepathTextboxName";
            this.inputFilepathTextboxName.ReadOnly = true;
            this.inputFilepathTextboxName.Size = new System.Drawing.Size(112, 20);
            this.inputFilepathTextboxName.TabIndex = 5;
            this.inputFilepathTextboxName.Text = "Input filepath";
            this.inputFilepathTextboxName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // outputFilepathTextboxName
            // 
            this.outputFilepathTextboxName.BackColor = System.Drawing.SystemColors.Control;
            this.outputFilepathTextboxName.Location = new System.Drawing.Point(12, 50);
            this.outputFilepathTextboxName.Name = "outputFilepathTextboxName";
            this.outputFilepathTextboxName.ReadOnly = true;
            this.outputFilepathTextboxName.Size = new System.Drawing.Size(112, 20);
            this.outputFilepathTextboxName.TabIndex = 6;
            this.outputFilepathTextboxName.Text = "Output filepath";
            this.outputFilepathTextboxName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ExportWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 152);
            this.Controls.Add(this.outputFilepathTextboxName);
            this.Controls.Add(this.inputFilepathTextboxName);
            this.Controls.Add(this.selectOutputFilepathButton);
            this.Controls.Add(this.outputFilepathTextbox);
            this.Controls.Add(this.inputFilepathTextbox);
            this.Controls.Add(this.exportButton);
            this.Name = "ExportWindowForm";
            this.Text = "ExportWindow";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox outputFilepathTextboxName;

        private System.Windows.Forms.TextBox inputFilepathTextboxName;

        private System.Windows.Forms.TextBox outputFilepathTextbox;
        private System.Windows.Forms.Button selectOutputFilepathButton;

        private System.Windows.Forms.TextBox inputFilepathTextbox;

        private System.Windows.Forms.Button exportButton;

        #endregion
    }
}