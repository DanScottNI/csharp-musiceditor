using System;
using System.Windows.Forms;

namespace CapcomMusicDisassembler
{
    public partial class frmTrackDisassembly : Form
    {
        string trackdisassembly;

        public frmTrackDisassembly(string trackDisassembly)
        {
            InitializeComponent();
            this.trackdisassembly = trackDisassembly;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {

        }

        private void frmTrackDisassembly_Load(object sender, EventArgs e)
        {
            txtDisassembly.Text = trackdisassembly;
        }
    }
}
