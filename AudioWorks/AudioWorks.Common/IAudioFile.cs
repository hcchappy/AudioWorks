﻿using System;
using System.IO;
using JetBrains.Annotations;

namespace AudioWorks.Common
{
    /// <summary>
    /// The primary base type for working with AudioWorks. Represents a single track of audio within the filesystem.
    /// </summary>
    [PublicAPI]
    public interface IAudioFile
    {
        /// <summary>
        /// Gets the fully-qualified file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        [NotNull]
        string Path { get; }

        /// <summary>
        /// Gets the audio information.
        /// </summary>
        /// <value>
        /// The audio information.
        /// </value>
        [NotNull]
        AudioInfo Info { get; }

        /// <summary>
        /// Renames the audio file.
        /// </summary>
        /// <param name="name">The new file name, not including the extension.</param>
        /// <param name="replace">if set to <c>true</c> and the new file name already exists, replace it.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name" /> is null or empty.</exception>
        /// <exception cref="IOException">Thrown if the new file already exists, and <paramref name="replace"/> is
        /// <c>false</c>.</exception>
        void Rename([NotNull] string name, bool replace);
    }
}