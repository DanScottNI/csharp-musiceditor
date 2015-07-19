using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CapcomMusicDisassembler
{
    public partial class frmReassemble : Form
    {
        string filename;
        MusicData musicData;
        string executablePath;

        public frmReassemble(string romFilename, MusicData music)
        {
            InitializeComponent();

            this.musicData = music;
            this.filename = romFilename;
            this.executablePath = Path.GetDirectoryName(Application.ExecutablePath);

            txtImportDirectory.Text = Path.Combine(this.executablePath, this.musicData.GameShortName);
        }

        void RunWithRedirect(string executableName, string commandLineArgs)
        {
            var proc = new Process();
            proc.StartInfo.FileName = executableName;
            StreamReader readerOutput;
            // set up output redirection
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            if (!string.IsNullOrEmpty(commandLineArgs))
            {
                proc.StartInfo.Arguments = commandLineArgs;
            }
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(executableName);

            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.UseShellExecute = false;
            proc.EnableRaisingEvents = true;
            proc.Start();
            readerOutput = proc.StandardOutput;

            proc.WaitForExit();

            txtOutput.Text = readerOutput.ReadToEnd().Replace(Path.GetDirectoryName(executableName), string.Empty);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string batchFile = Path.Combine(txtImportDirectory.Text, "make.bat");
            if (File.Exists(batchFile))
            {
                string sourceLinker = Path.Combine(this.executablePath, Properties.Resources.LinkerExe);
                string sourceCompiler = Path.Combine(this.executablePath, Properties.Resources.CompilerExe);
                string destCompiler = Path.Combine(txtImportDirectory.Text, Properties.Resources.CompilerExe);
                string destLinker = Path.Combine(txtImportDirectory.Text, Properties.Resources.LinkerExe);
                // Now copy cc65.exe and ld65.exe to the import directory.
                File.Copy(sourceCompiler, destCompiler, true);
                File.Copy(sourceLinker, destLinker, true);

                // Execute the batch file.
                this.RunWithRedirect(batchFile, string.Empty);

                // Check for the existence of music.prg.
                if (File.Exists(Path.Combine(txtImportDirectory.Text, Properties.Resources.MusicPRG)))
                {

                }
                else
                {
                    MessageBox.Show(Properties.Resources.NoOutputFile, Properties.Resources.ApplicationTitle);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.AssemblerFileNotExist, Properties.Resources.ApplicationTitle);
            }
        }

    }
}
