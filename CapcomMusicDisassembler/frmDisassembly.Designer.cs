namespace CapcomMusicDisassembler
{
    partial class frmDisassembly
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
            this.lvwTracks = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExportDirectory = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnDisassemble = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwTracks
            // 
            this.lvwTracks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwTracks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwTracks.FullRowSelect = true;
            this.lvwTracks.GridLines = true;
            this.lvwTracks.Location = new System.Drawing.Point(12, 65);
            this.lvwTracks.Name = "lvwTracks";
            this.lvwTracks.Size = new System.Drawing.Size(583, 312);
            this.lvwTracks.TabIndex = 0;
            this.lvwTracks.UseCompatibleStateImageBehavior = false;
            this.lvwTracks.View = System.Windows.Forms.View.Details;
            this.lvwTracks.DoubleClick += new System.EventHandler(this.lvwTracks_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Track Index.";
            this.columnHeader1.Width = 100;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Track Type.";
            this.columnHeader2.Width = 150;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Offset.";
            this.columnHeader3.Width = 100;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Export Directory:";
            // 
            // txtExportDirectory
            // 
            this.txtExportDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExportDirectory.Location = new System.Drawing.Point(104, 9);
            this.txtExportDirectory.Name = "txtExportDirectory";
            this.txtExportDirectory.Size = new System.Drawing.Size(410, 21);
            this.txtExportDirectory.TabIndex = 2;
            this.txtExportDirectory.Text = "C:\\Documents and Settings\\Dan\\My Documents\\Visual Studio 2008\\Projects\\MusicDisas" +
                "sembler\\CapcomMusicDisassembler\\bin\\Debug\\Megaman Music";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(520, 9);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 21);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnDisassemble
            // 
            this.btnDisassemble.Location = new System.Drawing.Point(520, 36);
            this.btnDisassemble.Name = "btnDisassemble";
            this.btnDisassemble.Size = new System.Drawing.Size(75, 23);
            this.btnDisassemble.TabIndex = 4;
            this.btnDisassemble.Text = "Disassemble";
            this.btnDisassemble.UseVisualStyleBackColor = true;
            this.btnDisassemble.Click += new System.EventHandler(this.tlbDisassemble_Click);
            // 
            // frmDisassembly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 389);
            this.Controls.Add(this.btnDisassemble);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtExportDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lvwTracks);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(610, 420);
            this.Name = "frmDisassembly";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Music Track Disassembly";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvwTracks;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.FolderBrowserDialog fbdOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExportDirectory;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnDisassemble;



    }
}

