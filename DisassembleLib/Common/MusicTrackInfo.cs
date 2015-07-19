using System;
using System.Collections.Generic;
using System.Text;

namespace DisassembleLib.Common
{
    /// <summary>
    /// An enumerated type for distinguishing the type of music track.
    /// </summary>
    public enum MusicTrackType
    {
        /// <summary>
        /// If the track is a music track.
        /// </summary>
        MusicTrack,
        
        /// <summary>
        /// If the track is a sound effect.
        /// </summary>
        SoundEffect
    }

    /// <summary>
    /// A class that contains information on each track.
    /// </summary>
    public class MusicTrackInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MusicTrackInfo"/> class.
        /// </summary>
        /// <param name="trackIndex">Index of the track.</param>
        /// <param name="trackOffset">The track offset.</param>
        /// <param name="trackType">Type of the track.</param>
        public MusicTrackInfo(int trackIndex, int trackOffset, MusicTrackType trackType)
        {
            this.TrackIndex = trackIndex;
            this.TrackOffset = trackOffset;
            this.TrackType = trackType;
        }

        /// <summary>
        /// Gets or sets the index of the track.
        /// </summary>
        public int TrackIndex { get; set; }

        /// <summary>
        /// Gets or sets the track offset.
        /// </summary>
        public int TrackOffset { get; set; }

        /// <summary>
        /// Gets or sets the type of the track.
        /// </summary>
        public MusicTrackType TrackType { get; set; }
    }
}