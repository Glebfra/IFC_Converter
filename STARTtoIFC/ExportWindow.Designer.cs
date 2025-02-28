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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(148, 99);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(106, 38);
            this.exportButton.TabIndex = 0;
            this.exportButton.Text = "Экспорт";
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButton_Click);
            // 
            // inputFilepathTextbox
            // 
            this.inputFilepathTextbox.Location = new System.Drawing.Point(98, 12);
            this.inputFilepathTextbox.Name = "inputFilepathTextbox";
            this.inputFilepathTextbox.ReadOnly = true;
            this.inputFilepathTextbox.Size = new System.Drawing.Size(282, 20);
            this.inputFilepathTextbox.TabIndex = 1;
            // 
            // outputFilepathTextbox
            // 
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Input filepath";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Output filepath";
            // 
            // ExportWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 149);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.selectOutputFilepathButton);
            this.Controls.Add(this.outputFilepathTextbox);
            this.Controls.Add(this.inputFilepathTextbox);
            this.Controls.Add(this.exportButton);
            this.Name = "ExportWindowForm";
            this.Text = "ExportWindow";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label label2;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.TextBox outputFilepathTextbox;
        
        private System.Windows.Forms.TextBox inputFilepathTextbox;
        
        private System.Windows.Forms.Button selectOutputFilepathButton;

        private System.Windows.Forms.Button exportButton;

        #endregion
    }
}