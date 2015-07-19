using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace CapcomMusicDisassembler
{
    public partial class frmMainWindow : Form
    {
        List<MusicData> musicData;
        string executablePath;

        public frmMainWindow()
        {
            InitializeComponent();
        }

        private void btnDisassemble_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                // Look up the game with the specified name, and retrieve the table offset.
                MusicData music = musicData.Where(delegate(MusicData data)
                {
                    return data.GameName == lstFiles.SelectedItem.ToString();
                }).Single();

                string filename = Path.Combine(executablePath, music.GameShortName + Properties.Resources.NESFileExtension);

                if (File.Exists(filename))
                {
                    using (frmDisassembly frmDis = new frmDisassembly(filename, music))
                    {
                        frmDis.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ROMNotExist, Properties.Resources.ApplicationTitle);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.SelectGame, Properties.Resources.ApplicationTitle);
            }
        }

        public void PopulateListBox()
        {
            lstFiles.BeginUpdate();
            lstFiles.Items.Clear();

            foreach (MusicData data in musicData)
            {
                if (File.Exists(Path.Combine(executablePath, data.GameShortName + Properties.Resources.NESFileExtension)))
                {
                    lstFiles.Items.Add(data.GameName);
                }
            }
            lstFiles.EndUpdate();
        }

        public void LoadMusicData()
        {
            string configurationFilename = Path.Combine(executablePath, "MusicData.xml");
            if (System.IO.File.Exists(configurationFilename) == true)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFilename);

                musicData = new List<MusicData>();

                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {

                    if (node.Name == "music")
                    {
                        MusicData data = new MusicData();
                        foreach (XmlAttribute att in node.Attributes)
                        {
                            if (att.Name == "name")
                            {
                                data.GameName = att.InnerText;
                            }
                            else if (att.Name == "shortname")
                            {
                                data.GameShortName = att.InnerText;
                            }
                            else if (att.Name == "tablestart")
                            {
                                if (!string.IsNullOrEmpty(att.InnerText))
                                {
                                    data.MusicTableOffset = Convert.ToInt32("0x" + att.InnerText, 16);
                                }
                            }
                            else if (att.Name == "datasize")
                            {
                                if (!string.IsNullOrEmpty(att.InnerText))
                                {
                                    data.MusicDataSize = Convert.ToInt32("0x" + att.InnerText, 16);
                                }
                            }
                        }

                        musicData.Add(data);
                    }
                }
            }
        }

        private void frmMainWindow_Load(object sender, EventArgs e)
        {
            executablePath = Path.GetDirectoryName(Application.ExecutablePath);
            LoadMusicData();
            PopulateListBox();
        }

        private void btnReassemble_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItems.Count > 0)
            {
                // Look up the game with the specified name, and retrieve the table offset.
                MusicData music = musicData.Where(delegate(MusicData data)
                {
                    return data.GameName == lstFiles.SelectedItem.ToString();
                }).Single();

                string filename = Path.Combine(executablePath, music.GameShortName + Properties.Resources.NESFileExtension);

                if (File.Exists(filename))
                {
                    using (frmReassemble frmReass = new frmReassemble(filename, music))
                    {
                        frmReass.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.ROMNotExist, Properties.Resources.ApplicationTitle);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.SelectGame, Properties.Resources.ApplicationTitle);
            }
        }

    }
}
