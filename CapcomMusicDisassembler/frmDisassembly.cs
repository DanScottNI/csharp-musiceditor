using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DisassembleLib.CapcomEngine;
using DisassembleLib.Common;
using ROMClass;

namespace CapcomMusicDisassembler
{
    /// <summary>
    /// The form that shows all the tracks in the game.
    /// </summary>
    public partial class frmDisassembly : Form
    {
        string filename;
        MusicData musicData;
        IMusicDisassemble dis;
        Dictionary<int, string> trackDisassemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="frmDisassembly"/> class.
        /// </summary>
        /// <param name="romFilename">The rom filename.</param>
        /// <param name="music">The music information.</param>
        public frmDisassembly(string romFilename, MusicData music)
        {
            InitializeComponent();

            this.musicData = music;
            this.filename = romFilename;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtExportDirectory.Text = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), musicData.GameShortName);
            txtExportDirectory.SelectionStart = txtExportDirectory.Text.Length;
            this.PopulateListview(this.filename);
            this.Text = Properties.Resources.MusicTrackDisassembly + " - " + musicData.GameName;
        }

        private void PopulateListview(string filename)
        {
            this.dis = new CapcomEngineDisassembler(this.filename, this.musicData.MusicTableOffset, this.musicData.MusicDataSize);

            List<MusicTrackInfo> trackInfo = this.dis.RetrieveTrackInformation();
            lvwTracks.BeginUpdate();
            lvwTracks.Items.Clear();
            for (int i = 0; i < trackInfo.Count; i++)
            {
                ListViewItem item = new ListViewItem(trackInfo[i].TrackIndex.ToString());

                if (trackInfo[i].TrackType == MusicTrackType.MusicTrack)
                {
                    item.SubItems.Add(Properties.Resources.MusicTrack);
                }
                else
                {
                    item.SubItems.Add(Properties.Resources.SoundEffect);
                }

                item.SubItems.Add(trackInfo[i].TrackOffset.ToHex());

                lvwTracks.Items.Add(item);
            }

            lvwTracks.EndUpdate();

            this.trackDisassemblies = this.dis.Disassemble();
        }

        private void lvwTracks_DoubleClick(object sender, EventArgs e)
        {
            if (lvwTracks.SelectedItems.Count > 0)
            {
                frmTrackDisassembly frmTrack = new frmTrackDisassembly(this.trackDisassemblies[Convert.ToInt32(lvwTracks.SelectedItems[0].Text)]);
                frmTrack.ShowDialog(this);
            }
        }

        private void tlbDisassemble_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(txtExportDirectory.Text))
            {
                // If the directory doesn't exist, then create it.
                if (Directory.Exists(txtExportDirectory.Text) == false)
                {
                    Directory.CreateDirectory(txtExportDirectory.Text);
                }

                this.dis.Disassemble(txtExportDirectory.Text);

                MessageBox.Show(Properties.Resources.TracksDisassembled, Properties.Resources.ApplicationTitle);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (fbdOutput.ShowDialog() == DialogResult.OK)
            {
                txtExportDirectory.Text = fbdOutput.SelectedPath;
                txtExportDirectory.SelectionStart = txtExportDirectory.Text.Length;
            }
        }
    }
}