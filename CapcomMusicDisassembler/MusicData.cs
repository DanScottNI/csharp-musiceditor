using System;
using System.Collections.Generic;
using System.Text;

namespace CapcomMusicDisassembler
{
    public class MusicData
    {
        public string GameName { get; set; }
        public string GameShortName { get; set; }
        public int MusicTableOffset { get; set; }
        public int MusicDataSize { get; set; }
    }
}
