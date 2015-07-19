using System;
using System.Collections.Generic;
using System.Text;

namespace DisassembleLib.CapcomEngine
{
    /// <summary>
    /// Enumerated type to represent the types of opcodes.
    /// </summary>
    public enum MusicOpcodeType
    {
        /// <summary>
        /// Address type.
        /// </summary>
        Address = 1,

        /// <summary>
        /// The opcode type is data.
        /// </summary>
        Data = 2,

        /// <summary>
        /// End of the data.
        /// </summary>
        End = 3,

        /// <summary>
        /// The reference to the vibrato index table.
        /// </summary>
        VibratoIndexReference = 4,

        /// <summary>
        /// A music command.
        /// </summary>
        Command = 5,

        /// <summary>
        /// The amount of times that the loop should execute.
        /// </summary>
        LoopCounter = 6
    }

    /// <summary>
    /// Class that encapsulates the format in the opcode definition file.
    /// </summary>
    public class MusicOpcodeFormat
    {
        /// <summary>
        /// Gets or sets the hexadecimal value of this opcode.
        /// </summary>
        public byte ID { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the opcode.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets the opcode type.
        /// </summary>
        public string OpcodeType { get; set; }

        /// <summary>
        /// Gets or sets the length of the opcode.
        /// </summary>
        public byte Length { get; set; }

        /// <summary>
        /// Gets or sets the various types used in this opcode.
        /// </summary>
        public Dictionary<int, MusicOpcodeType> OpcodeTypes { get; set; }
    }
}