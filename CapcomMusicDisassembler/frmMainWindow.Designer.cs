namespace CapcomMusicDisassembler
{
    partial class frmMainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.btnDisassemble = new System.Windows.Forms.Button();
            this.btnReassemble = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnDisassemble
            // 
            this.btnDisassemble.Location = new System.Drawing.Point(459, 364);
            this.btnDisassemble.Name = "btnDisassemble";
            this.btnDisassemble.Size = new System.Drawing.Size(75, 23);
            this.btnDisassemble.TabIndex = 3;
            this.btnDisassemble.Text = "Disassemble";
            this.btnDisassemble.UseVisualStyleBackColor = true;
            this.btnDisassemble.Click += new System.EventHandler(this.btnDisassemble_Click);
            // 
            // btnReassemble
            // 
            this.btnReassemble.Location = new System.Drawing.Point(540, 364);
            this.btnReassemble.Name = "btnReassemble";
            this.btnReassemble.Size = new System.Drawing.Size(75, 23);
            this.btnReassemble.TabIndex = 4;
            this.btnReassemble.Text = "Reassemble";
            this.btnReassemble.UseVisualStyleBackColor = true;
            this.btnReassemble.Click += new System.EventHandler(this.btnReassemble_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.Location = new System.Drawing.Point(12, 12);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(603, 342);
            this.lstFiles.TabIndex = 5;
            // 
            // frmMainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 399);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.btnReassemble);
            this.Controls.Add(this.btnDisassemble);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Capcom Music Engine Tool";
            this.Load += new System.EventHandler(this.frmMainWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDisassemble;
        private System.Windows.Forms.Button btnReassemble;
        private System.Windows.Forms.ListBox lstFiles;
    }
}