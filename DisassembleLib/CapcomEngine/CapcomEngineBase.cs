using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ROMClass;
using ROMClass.NES;

namespace DisassembleLib.CapcomEngine
{
    /// <summary>
    /// A class that contains common properties for Capcom Engine related classes.
    /// </summary>
    public abstract class CapcomEngineBase
    {
        /// <summary>
        /// Initializes a new instance of the CapcomEngineBase class.
        /// </summary>
        /// <param name="fileName">The filename of the ROM.</param>
        /// <param name="tableOffset">The offset in the ROM for the music\sound effects table.</param>
        /// <param name="musicDataSize">The size that the music data should be.</param>
        public CapcomEngineBase(string fileName, int tableOffset, int musicDataSize)
        {
            // Load in the NES ROM.
            this.ROM = new INESROMImage(fileName);

            // Store the size of the music data.
            this.MusicDataSize = musicDataSize;

            // Store the music table offset.
            this.MusicTableOffset = tableOffset;

            // Load in the music opcodes.
            this.LoadMusicOpcodes();

            // Load in the sound effect opcodes.
            this.LoadSoundEffectOpcodes();
        }

        /// <summary>
        /// Gets or sets the ROM that contains the music tracks.
        /// </summary>
        internal INESROMImage ROM { get; set; }

        /// <summary>
        /// Gets or sets the music opcodes.
        /// </summary>
        internal Dictionary<byte, MusicOpcodeFormat> MusicOpcodes { get; set; }

        /// <summary>
        /// Gets or sets the sound effect opcodes.
        /// </summary>
        internal Dictionary<byte, MusicOpcodeFormat> SoundEffectOpcodes { get; set; }

        /// <summary>
        /// Gets or sets the offset for the music table.
        /// </summary>
        internal int MusicTableOffset { get; set; }

        /// <summary>
        /// Gets or sets the maximum size that the music data should be, when inserting into the ROM.
        /// </summary>
        internal int MusicDataSize { get; set; }

        /// <summary>
        /// Loads the music opcodes.
        /// </summary>
        private void LoadMusicOpcodes()
        {
            this.MusicOpcodes = this.LoadOpcodeDefinitionFile("DisassembleLib.CapcomEngine.musicopcodes.dat");
        }

        /// <summary>
        /// Loads the sound effect opcodes.
        /// </summary>
        private void LoadSoundEffectOpcodes()
        {
            this.SoundEffectOpcodes = this.LoadOpcodeDefinitionFile("DisassembleLib.CapcomEngine.seopcodes.dat");
        }

        /// <summary>
        /// Loads the opcode definition file.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>A List of MusicOpcodeFormat objects.</returns>
        private Dictionary<byte, MusicOpcodeFormat> LoadOpcodeDefinitionFile(string resourceName)
        {
            Assembly resAssembly;
            resAssembly = Assembly.GetExecutingAssembly();
            Dictionary<byte, MusicOpcodeFormat> opcodes = new Dictionary<byte, MusicOpcodeFormat>();
            MusicOpcodeFormat opcode = null;

            TextReader textReader = new StreamReader(resAssembly.GetManifestResourceStream(resourceName));
            string line = string.Empty;

            while ((line = textReader.ReadLine()) != null)
            {
                // Ignore all lines beginning with the semi-colon character.
                if (line.StartsWith(";") == false)
                {
                    opcode = new MusicOpcodeFormat();

                    // Split the line into the fields by the comma character.
                    string[] fields = line.Split(',');

                    if (fields.Length == 3)
                    {
                        opcode.Name = fields[1];
                        opcode.ID = fields[0].HexValueToByte();
                        opcode.Length = Convert.ToByte(fields[2].Length / 2);
                        opcode.OpcodeTypes = new Dictionary<int, MusicOpcodeType>();

                        // Parse the third field, for the opcode types, and the
                        // the length.
                        for (int i = 0; i < fields[2].Length; i = i + 2)
                        {
                            string opcodecharacters = fields[2].Substring(i, 2);
                            MusicOpcodeType opcodetype = MusicOpcodeType.Data;
                            switch (opcodecharacters)
                            {
                                case "AA":
                                    opcodetype = MusicOpcodeType.Address;
                                    break;
                                case "CC":
                                    opcodetype = MusicOpcodeType.Command;
                                    break;
                                case "DD":
                                    opcodetype = MusicOpcodeType.Data;
                                    break;
                                case "EE":
                                    opcodetype = MusicOpcodeType.End;
                                    break;
                                case "VV":
                                    opcodetype = MusicOpcodeType.VibratoIndexReference;
                                    break;
                                case "LL":
                                    opcodetype = MusicOpcodeType.LoopCounter;
                                    break;
                                default:
                                    break;
                            }

                            opcode.OpcodeTypes.Add((i / 2), opcodetype);
                        }
                    }

                    opcodes.Add(fields[0].HexValueToByte(), opcode);
                }
            }

            return opcodes;
        }
    }
}
