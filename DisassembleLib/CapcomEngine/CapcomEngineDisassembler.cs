using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DisassembleLib.Common;
using ROMClass;

namespace DisassembleLib.CapcomEngine
{
    /// <summary>
    /// Class used to produce a disassembly of the music data for early Capcom games.
    /// </summary>
    /// <remarks>
    /// - 1943
    /// - Bionic Commando
    /// - Gun Smoke
    /// - Hitler no Fukkatsu - Top Secret
    /// - Legendary Wings
    /// - Mega Man
    /// - Mega Man 2
    /// - Pro Yakyuu Satsujin Jiken!
    /// - Rockman
    /// - Rockman 2
    /// - Willow
    /// </remarks>
    public class CapcomEngineDisassembler : CapcomEngineBase, IMusicDisassemble
    {
        /// <summary>
        /// Initializes a new instance of the CapcomEngineDisassembler class.
        /// </summary>
        /// <param name="fileName">The filename of the ROM.</param>
        /// <param name="tableOffset">The offset in the ROM for the music\sound effects table.</param>
        /// <param name="musicDataSize">The size that the music data should be.</param>
        public CapcomEngineDisassembler(string fileName, int tableOffset, int musicDataSize)
            : base(fileName, tableOffset, musicDataSize)
        {
        }

        /// <summary>
        /// Retrieves the track information.
        /// </summary>
        /// <returns>A List of MusicTrackInfo objects that contain information on the music tracks in this game.</returns>
        public List<MusicTrackInfo> RetrieveTrackInformation()
        {
            // We need to find where the track table ends, so we store
            // the offset of the very first track.
            int lowestTrackOffset = -1;
            int trackIndex = 0;

            // We need to store a dictionary of the track indexes, and their own particular offsets.
            List<int> musicTrackOffsets = new List<int>();

            // Okay, now we need to iterate through the table, finding the offsets of all the tracks.
            // This loop should technically never end, as want to iterate through it until we find
            // the
            for (int i = 0; true; i = i + 2)
            {
                int musicOffset = this.ROM.PointerToOffset(this.MusicTableOffset + i);

                // If this is the first offset, store the offset.
                // If it isn't, but the offset from the pointer is less than that stored
                // in the lowest track offset, store the current pointer.
                if (musicOffset > 0)
                {
                    if (lowestTrackOffset == -1)
                    {
                        lowestTrackOffset = musicOffset;
                    }
                    else if (musicOffset < lowestTrackOffset)
                    {
                        lowestTrackOffset = musicOffset;
                    }
                }

                trackIndex++;

                // If the current offset matches the track offset
                if ((this.MusicTableOffset + i) == lowestTrackOffset)
                {
                    break;
                }
                else
                {
                    // Add the location, and track index to the list.
                    musicTrackOffsets.Add(musicOffset);
                }
            }

            // Work out the number of tracks.
            int numberOfTracks = (lowestTrackOffset - this.MusicTableOffset) / 2;

            List<MusicTrackInfo> trackList = new List<MusicTrackInfo>();

            for (int i = 0; i < musicTrackOffsets.Count; i++)
            {
                int trackOffset = musicTrackOffsets[i];
                if ((this.ROM[trackOffset] & 0xF0) > 00)
                {
                    trackList.Add(new MusicTrackInfo(i, trackOffset, MusicTrackType.SoundEffect));
                }
                else
                {
                    if (this.ROM[trackOffset] != 0x0F)
                    {
                        throw new Exception("This is not a valid music format!");
                    }

                    trackList.Add(new MusicTrackInfo(i, trackOffset, MusicTrackType.MusicTrack));
                }
            }

            return trackList;
        }

        /// <summary>
        /// Disassembles the music table, and dumps the disassemblies to a specified directory.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        public void Disassemble(string folderPath)
        {
            Dictionary<int, string> trackDisassemblies = this.Disassemble();

            // controlcodes.inc
            using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, "controlcodes.inc")))
            {
                foreach (KeyValuePair<byte, MusicOpcodeFormat> pair in this.MusicOpcodes)
                {
                    sw.WriteLine(pair.Value.Name + " = $" + pair.Key.ToHex());
                }

                foreach (KeyValuePair<byte, MusicOpcodeFormat> pair in this.SoundEffectOpcodes)
                {
                    sw.WriteLine(pair.Value.Name + " = $" + pair.Key.ToHex());
                }
            }

            // make.bat
            using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, "make.bat")))
            {
                sw.WriteLine("ca65.exe music.asm");
                sw.WriteLine("ld65.exe -C nes.cfg -o music.prg music.o");
            }

            // music.asm
            using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, "music.asm")))
            {
                sw.WriteLine(".segment \"music\"");
                sw.WriteLine(".include \"controlcodes.inc\"");

                List<MusicTrackInfo> trackInfos = this.RetrieveTrackInformation();
                Dictionary<int, int> trackOffsets = new Dictionary<int, int>();

                foreach (MusicTrackInfo info in trackInfos)
                {
                    if (!trackOffsets.ContainsKey(info.TrackOffset))
                    {
                        trackOffsets.Add(info.TrackOffset, info.TrackIndex);
                    }
                }

                foreach (MusicTrackInfo info in trackInfos)
                {
                    sw.WriteLine(string.Format(".word TRACK{0}START", trackOffsets[info.TrackOffset]));
                }

                Dictionary<int, bool> includedFiles = new Dictionary<int, bool>();

                foreach (MusicTrackInfo info in trackInfos)
                {
                    if (!includedFiles.ContainsKey(trackOffsets[info.TrackOffset]))
                    {
                        sw.WriteLine(string.Format(".include \"{0}.asm\"", trackOffsets[info.TrackOffset].ToHex()));
                        includedFiles.Add(trackOffsets[info.TrackOffset], true);
                    }
                }
            }

            // nes.cfg
            using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, "nes.cfg")))
            {
                sw.WriteLine("MEMORY {");
                sw.WriteLine(string.Format("PRG0: start = ${0}, SIZE = ${1}, fill=yes,fillval = $FF;", this.ROM.OffsetToPointer(MusicTableOffset, 0x8000, 0x4000).ToHex(), this.MusicDataSize.ToHex()));
                sw.WriteLine("}");
                sw.WriteLine(" SEGMENTS {");
                sw.WriteLine(" music: LOAD = PRG0, start = $" + this.ROM.OffsetToPointer(MusicTableOffset, 0x8000, 0x4000).ToHex() + ";");
                sw.WriteLine("}");
            }

            foreach (KeyValuePair<int, string> pair in trackDisassemblies)
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(folderPath, pair.Key.ToHex() + ".asm"), false))
                {
                    sw.Write(pair.Value);
                }
            }
        }

        /// <summary>
        /// Disassembles the music table, and all the music and sound effects.
        /// </summary>
        /// <returns>A string containing the disassembly.</returns>
        public Dictionary<int, string> Disassemble()
        {
            Dictionary<int, string> trackDisassemblies = new Dictionary<int, string>();

            List<MusicTrackInfo> musicTrackInfo = this.RetrieveTrackInformation();

            // Work out the number of tracks.
            int numberOfTracks = musicTrackInfo.Count;

            for (int i = 0; i < musicTrackInfo.Count; i++)
            {
                MusicTrackInfo currentTrack = musicTrackInfo[i];

                int trackOffset = currentTrack.TrackOffset;

                if ((this.ROM[trackOffset] & 0xF0) > 00)
                {
                    // Disassemble the sound effect.
                    trackDisassemblies.Add(currentTrack.TrackIndex, this.DisassembleSoundEffect(ref currentTrack));
                }
                else
                {
                    // Disassemble the music track.
                    trackDisassemblies.Add(currentTrack.TrackIndex, this.DisassembleMusicTrack(ref currentTrack));
                }
            }

            return trackDisassemblies;
        }

        /// <summary>
        /// Disassembles the sound effect.
        /// </summary>
        /// <param name="trackInfo">The MusicTrackInfo object used to retrieve track information.</param>
        /// <returns>An annotated disassembly of the sound effect.</returns>
        private string DisassembleSoundEffect(ref MusicTrackInfo trackInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("TRACK{0}START:", trackInfo.TrackIndex));

            // The format of a sound effect is:
            // Priority Byte
            // Channel Usage
            // Sound Effect data.
            int currentPos = trackInfo.TrackOffset;

            sb.AppendLine(string.Format(".byte ${0} ;priority. Lo=music priority, Hi=sfx priority", this.ROM[currentPos].ToHex()));
            currentPos++;

            sb.AppendLine(string.Format(".byte ${0} ;channel usage", this.ROM[currentPos].ToHex()));
            currentPos++;

            sb.AppendLine(this.DisassembleSoundEffectData(trackInfo.TrackIndex, currentPos));

            return sb.ToString();
        }

        /// <summary>
        /// Disassembles the music track.
        /// </summary>
        /// <param name="trackInfo">The MusicTrackInfo object used to retrieve track information.</param>
        /// <returns>An annotated disassembly of the music track.</returns>
        private string DisassembleMusicTrack(ref MusicTrackInfo trackInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("TRACK{0}START:", trackInfo.TrackIndex));

            // The format of the music track is as follows:
            // Priority Byte
            // Channel 0 Pointer - Maybe 0000 to denote not used.
            // Channel 1 Pointer - Maybe 0000 to denote not used.
            // Channel 2 Pointer - Maybe 0000 to denote not used.
            // Channel 3 Pointer - Maybe 0000 to denote not used. Terminated with the end of data opcode.
            // Vibrato Table Pointer - Maybe 0000 to denote not used. To work out how many groups of 4 bytes there
            // are, the SetVibratoIndex command is tracked.
            int currentPos = trackInfo.TrackOffset;

            sb.AppendLine(string.Format(".byte ${0} ;priority. Lo=music priority, Hi=sfx priority", this.ROM[currentPos].ToHex()));
            currentPos++;

            // Channel 0
            if (this.ROM[currentPos] + this.ROM[currentPos + 1] > 0)
            {
                sb.AppendLine(".word Lbl_Track" + trackInfo.TrackIndex + "_Channel0");
            }
            else
            {
                sb.AppendLine(".word $0000");
            }

            currentPos += 2;

            // Channel 1
            if (this.ROM[currentPos] + this.ROM[currentPos + 1] > 0)
            {
                sb.AppendLine(".word Lbl_Track" + trackInfo.TrackIndex + "_Channel1");
            }
            else
            {
                sb.AppendLine(".word $0000");
            }

            currentPos += 2;

            // Channel 2
            if (this.ROM[currentPos] + this.ROM[currentPos + 1] > 0)
            {
                sb.AppendLine(".word Lbl_Track" + trackInfo.TrackIndex + "_Channel2");
            }
            else
            {
                sb.AppendLine(".word $0000");
            }

            currentPos += 2;

            // Channel 3
            if (this.ROM[currentPos] + this.ROM[currentPos + 1] > 0)
            {
                sb.AppendLine(".word Lbl_Track" + trackInfo.TrackIndex + "_Channel3");
            }
            else
            {
                sb.AppendLine(".word $0000");
            }

            currentPos += 2;

            bool hasVibratoPointer = false;

            // Vibrato
            if (this.ROM[currentPos] + this.ROM[currentPos + 1] > 0)
            {
                hasVibratoPointer = true;
                sb.AppendLine(".word Lbl_Track" + trackInfo.TrackIndex + "_Vibrato");
            }
            else
            {
                sb.AppendLine(".word $0000");
            }

            currentPos += 2;

            // Reset the track pointer.
            currentPos = trackInfo.TrackOffset + 1;
            int vibratoTracks = -1;

            // Channel 0
            sb.Append(this.DisassembleMusicChannelData(trackInfo.TrackIndex, currentPos, ref vibratoTracks, 0));
            currentPos += 2;

            // Channel 1
            sb.Append(this.DisassembleMusicChannelData(trackInfo.TrackIndex, currentPos, ref vibratoTracks, 1));
            currentPos += 2;

            // Channel 2
            sb.Append(this.DisassembleMusicChannelData(trackInfo.TrackIndex, currentPos, ref vibratoTracks, 2));
            currentPos += 2;

            // Channel 3
            sb.Append(this.DisassembleMusicChannelData(trackInfo.TrackIndex, currentPos, ref vibratoTracks, 3));
            currentPos += 2;

            // Vibrato
            if (vibratoTracks > -1 || hasVibratoPointer == true)
            {
                sb.Append(this.DisassembleMusicVibrato(trackInfo.TrackIndex, vibratoTracks, currentPos));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Disassembles the sound effect data.
        /// </summary>
        /// <param name="trackIndex">Index of the track.</param>
        /// <param name="soundEffectOffset">The sound effect offset.</param>
        /// <returns>A string containing the disassembled sound effect data.</returns>
        private string DisassembleSoundEffectData(int trackIndex, int soundEffectOffset)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> addressLabels = new Dictionary<int, string>();

            bool quitLoop = false;
            int firstPass = soundEffectOffset;

            while (quitLoop == false)
            {
                byte currentByte = this.ROM[firstPass];

                // Check if the current position in the ROM is for a known opcode,
                // or just a standard note.
                if (this.SoundEffectOpcodes.ContainsKey(currentByte))
                {
                    int currentAddress = 0;

                    for (int i = 0; i < this.SoundEffectOpcodes[currentByte].Length; i++)
                    {
                        MusicOpcodeType opcodeType = this.SoundEffectOpcodes[currentByte].OpcodeTypes[i];

                        if (opcodeType == MusicOpcodeType.End)
                        {
                            quitLoop = true;
                        }
                        else if (opcodeType == MusicOpcodeType.Address)
                        {
                            currentAddress = this.ROM.PointerToOffset(firstPass);
                            firstPass++;
                            firstPass++;
                            break;
                        }
                        else if (opcodeType == MusicOpcodeType.LoopCounter)
                        {
                            if (this.ROM[firstPass] == 0)
                            {
                                quitLoop = true;
                            }
                        }

                        firstPass++;
                    }

                    if (currentAddress > 0 && addressLabels.ContainsKey(currentAddress) == false)
                    {
                        addressLabels.Add(currentAddress, "Lbl_" + currentAddress.ToHex());
                    }
                }
                else
                {
                    firstPass++;
                }
            }

            quitLoop = false;

            // Now we have the address label information, we can start to 
            while (quitLoop == false)
            {
                byte currentByte = this.ROM[soundEffectOffset];

                // If the current offset has a label for it, then we should 
                if (addressLabels.ContainsKey(soundEffectOffset) == true)
                {
                    sb.AppendLine(addressLabels[soundEffectOffset] + ":");
                }

                // Check if the current position in the ROM is for a known opcode,
                // or just a standard note.
                if (this.SoundEffectOpcodes.ContainsKey(currentByte))
                {
                    sb.Append(".byte ");

                    for (int i = 0; i < this.SoundEffectOpcodes[currentByte].Length; i++)
                    {
                        MusicOpcodeType opcodeType = this.SoundEffectOpcodes[currentByte].OpcodeTypes[i];

                        if (opcodeType == MusicOpcodeType.End)
                        {
                            sb.Append(this.SoundEffectOpcodes[currentByte].Name);
                            quitLoop = true;
                        }
                        else if (opcodeType == MusicOpcodeType.Command)
                        {
                            sb.Append(this.SoundEffectOpcodes[currentByte].Name);
                        }
                        else if (opcodeType == MusicOpcodeType.Data)
                        {
                            sb.Append(", $" + this.ROM[soundEffectOffset].ToHex());
                        }
                        else if (opcodeType == MusicOpcodeType.Address)
                        {
                            if (addressLabels.ContainsKey(this.ROM.PointerToOffset(soundEffectOffset)) == false)
                            {
                                sb.Append(", $" + this.ROM[soundEffectOffset].ToHex());
                            }
                            else
                            {
                                sb.Append(Environment.NewLine + ".word " + addressLabels[this.ROM.PointerToOffset(soundEffectOffset)]);
                                soundEffectOffset++;
                                soundEffectOffset++;
                                break;
                            }
                        }
                        else if (opcodeType == MusicOpcodeType.LoopCounter)
                        {
                            sb.Append(", $" + this.ROM[soundEffectOffset].ToHex());
                            if (this.ROM[soundEffectOffset] == 0)
                            {
                                quitLoop = true;
                            }
                        }

                        soundEffectOffset++;
                    }

                    sb.Append(Environment.NewLine);
                }
                else
                {
                    sb.AppendLine(".byte $" + currentByte.ToHex());
                    soundEffectOffset++;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Disassembles the music channel data.
        /// </summary>
        /// <param name="trackIndex">Index of the track.</param>
        /// <param name="entryOffset">The entry offset.</param>
        /// <param name="vibratoTracks">The vibrato tracks.</param>
        /// <param name="channelIndex">Index of the channel.</param>
        /// <returns>A string containing the disassembly.</returns>
        private string DisassembleMusicChannelData(int trackIndex, int entryOffset, ref int vibratoTracks, int channelIndex)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> addressLabels = new Dictionary<int, string>();

            sb.AppendLine("; Channel Pointer #" + channelIndex.ToString() + " Offset " + entryOffset.ToHex());

            // Look up the channel pointer.
            if (this.ROM[entryOffset] + this.ROM[entryOffset + 1] > 0)
            {
                int channelOffset = this.ROM.PointerToOffset(entryOffset);
                bool quitLoop = false;
                int firstPass = channelOffset;

                sb.AppendLine("; Channel #" + channelIndex.ToString() + " Offset " + channelOffset.ToHex());

                sb.AppendLine("Lbl_Track" + trackIndex + "_Channel" + channelIndex + ":");

                while (quitLoop == false)
                {
                    byte currentByte = this.ROM[firstPass];

                    // Check if the current position in the ROM is for a known opcode,
                    // or just a standard note.
                    if (this.MusicOpcodes.ContainsKey(currentByte))
                    {
                        int currentAddress = 0;

                        for (int i = 0; i < this.MusicOpcodes[currentByte].Length; i++)
                        {
                            MusicOpcodeType opcodeType = this.MusicOpcodes[currentByte].OpcodeTypes[i];

                            if (opcodeType == MusicOpcodeType.End)
                            {
                                quitLoop = true;
                            }
                            else if (opcodeType == MusicOpcodeType.Address)
                            {
                                currentAddress = this.ROM.PointerToOffset(firstPass);
                                firstPass++;
                                firstPass++;
                                break;
                            }
                            else if (opcodeType == MusicOpcodeType.LoopCounter)
                            {
                                if (this.ROM[firstPass] == 0)
                                {
                                    quitLoop = true;
                                }
                            }

                            firstPass++;
                        }

                        if (currentAddress > 0 && addressLabels.ContainsKey(currentAddress) == false)
                        {
                            addressLabels.Add(currentAddress, "Lbl_" + currentAddress.ToHex());
                        }
                    }
                    else
                    {
                        firstPass++;
                    }
                }

                quitLoop = false;

                // Now we have the address label information, we can start to 
                while (quitLoop == false)
                {
                    byte currentByte = this.ROM[channelOffset];

                    // If the current offset has a label for it, then we should 
                    if (addressLabels.ContainsKey(channelOffset) == true)
                    {
                        sb.AppendLine(addressLabels[channelOffset] + ":");
                    }

                    // Check if the current position in the ROM is for a known opcode,
                    // or just a standard note.
                    if (this.MusicOpcodes.ContainsKey(currentByte))
                    {
                        sb.Append(".byte ");

                        bool endLoop = false;

                        for (int i = 0; i < this.MusicOpcodes[currentByte].Length; i++)
                        {
                            MusicOpcodeType opcodeType = this.MusicOpcodes[currentByte].OpcodeTypes[i];

                            if (opcodeType == MusicOpcodeType.End)
                            {
                                sb.Append(this.MusicOpcodes[currentByte].Name);
                                quitLoop = true;
                            }
                            else if (opcodeType == MusicOpcodeType.Command)
                            {
                                sb.Append(this.MusicOpcodes[currentByte].Name);
                            }
                            else if (opcodeType == MusicOpcodeType.Data)
                            {
                                sb.Append(", $" + this.ROM[channelOffset].ToHex());
                            }
                            else if (opcodeType == MusicOpcodeType.Address)
                            {
                                if (addressLabels.ContainsKey(this.ROM.PointerToOffset(channelOffset)) == false)
                                {
                                    sb.Append(", $" + this.ROM[channelOffset].ToHex());
                                }
                                else
                                {
                                    sb.Append(Environment.NewLine + ".word " + addressLabels[this.ROM.PointerToOffset(channelOffset)]);
                                    channelOffset++;
                                    channelOffset++;
                                    break;
                                }
                            }
                            else if (opcodeType == MusicOpcodeType.VibratoIndexReference)
                            {
                                sb.Append(", $" + this.ROM[channelOffset].ToHex());
                                if (this.ROM[channelOffset] > vibratoTracks)
                                {
                                    vibratoTracks = this.ROM[channelOffset];
                                }
                            }
                            else if (opcodeType == MusicOpcodeType.LoopCounter)
                            {
                                sb.Append(", $" + this.ROM[channelOffset].ToHex());
                                if (this.ROM[channelOffset] == 0)
                                {
                                    endLoop = true;
                                }
                            }

                            channelOffset++;
                        }

                        sb.Append(Environment.NewLine);

                        if (endLoop == true)
                        {
                            break;
                        }
                    }
                    else
                    {
                        sb.AppendLine(".byte $" + currentByte.ToHex());
                        channelOffset++;
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Disassembles the music vibrato section.
        /// </summary>
        /// <param name="trackIndex">Index of the track.</param>
        /// <param name="numberVibrato">The number vibrato.</param>
        /// <param name="entryOffset">The entry offset.</param>
        /// <returns>An annotated disassembly of the music track's vibrato section.</returns>
        private string DisassembleMusicVibrato(int trackIndex, int numberVibrato, int entryOffset)
        {
            StringBuilder sb = new StringBuilder();
            int numberVibratoEntries = numberVibrato;

            // Look up the channel pointer.
            if (this.ROM[entryOffset] + this.ROM[entryOffset + 1] > 0)
            {
                int vibratoOffset = this.ROM.PointerToOffset(entryOffset);

                // Take the number of vibrato, and add one.
                numberVibratoEntries++;

                sb.AppendLine("Lbl_Track" + trackIndex + "_Vibrato:");

                for (int i = 0; i < numberVibratoEntries; i++)
                {
                    sb.Append(".byte ");
                    sb.Append("$" + this.ROM[vibratoOffset].ToHex() + ", ");
                    sb.Append("$" + this.ROM[vibratoOffset + 1].ToHex() + ", ");
                    sb.Append("$" + this.ROM[vibratoOffset + 2].ToHex() + ", ");
                    sb.Append("$" + this.ROM[vibratoOffset + 3].ToHex());
                    sb.Append(Environment.NewLine);
                    vibratoOffset += 4;
                }
            }

            return sb.ToString();
        }
    }
}