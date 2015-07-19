using System;
using System.Collections.Generic;
using System.Text;

namespace DisassembleLib.Common
{
    /// <summary>
    /// An interface for all music disassembly classes.
    /// </summary>
    public interface IMusicDisassemble
    {
        /// <summary>
        /// Retrieves the music track information.
        /// </summary>
        /// <returns>A List of MusicTrackInfo objects, containing information on the tracks in the ROM.</returns>
        List<MusicTrackInfo> RetrieveTrackInformation();

        /// <summary>
        /// Disassembles the music tracks.
        /// </summary>
        /// <returns>A Dictionary with the key as the track index, and the value as the disassembly.</returns>
        Dictionary<int, string> Disassemble();

        /// <summary>
        /// Disassembles the music to a specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        void Disassemble(string folderPath);
    }
}
