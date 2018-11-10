﻿/* Copyright © 2018 Jeremy Herbison

This file is part of AudioWorks.

AudioWorks is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later
version.

AudioWorks is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
details.

You should have received a copy of the GNU Lesser General Public License along with AudioWorks. If not, see
<https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using AudioWorks.Common;
using AudioWorks.Extensibility;
using JetBrains.Annotations;

namespace AudioWorks.Api
{
    /// <summary>
    /// Encodes one or more audio files in a new audio format.
    /// </summary>
    [PublicAPI]
    public sealed class AudioFileEncoder
    {
        [CanBeNull] readonly EncodedPath _encodedFileName;
        [CanBeNull] readonly EncodedPath _encodedDirectoryName;
        [NotNull] readonly ExportFactory<IAudioEncoder> _encoderFactory;

        /// <summary>
        /// Gets or sets a value indicating whether existing files should be overwritten.
        /// </summary>
        /// <value><c>true</c> if files should be overwritten; otherwise, <c>false</c>.</value>
        public bool Overwrite { get; set; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        /// <value>The settings.</value>
        [NotNull]
        public SettingDictionary Settings { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileEncoder"/> class.
        /// </summary>
        /// <param name="name">The name of the encoder.</param>
        /// <param name="encodedDirectoryName">The encoded directory name, or null.</param>
        /// <param name="encodedFileName">The encode file name, or null.</param>
        /// <param name="settings">The settings.</param>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <see paramref="name"/> is not the name of an available encoder.
        /// </exception>
        public AudioFileEncoder(
            [NotNull] string name,
            [CanBeNull] string encodedDirectoryName = null,
            [CanBeNull] string encodedFileName = null,
            [CanBeNull] SettingDictionary settings = null)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _encoderFactory = ExtensionProviderWrapper.GetFactories<IAudioEncoder>("Name", name).SingleOrDefault() ??
                              throw new ArgumentException($"No '{name}' encoder is available.", nameof(name));

            if (encodedDirectoryName != null)
                _encodedDirectoryName = new EncodedPath(encodedDirectoryName);
            if (encodedFileName != null)
                _encodedFileName = new EncodedPath(encodedFileName);

            using (var export = _encoderFactory.CreateExport())
                Settings = new ValidatingSettingDictionary(export.Value.SettingInfo, settings);
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="audioFiles">The audio files.</param>
        /// <returns>A new audio file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="audioFiles"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if one or more audio files are null.</exception>
        [NotNull, ItemNotNull]
        public async Task<IEnumerable<ITaggedAudioFile>> EncodeAsync(
            [NotNull, ItemNotNull] IEnumerable<ITaggedAudioFile> audioFiles)
        {
            if (audioFiles == null) throw new ArgumentNullException(nameof(audioFiles));

            return await EncodeAsync(audioFiles.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="audioFiles">The audio files.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="progress">The progress queue, or <c>null</c>.</param>
        /// <returns>A new audio file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="audioFiles"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if one or more audio files are null.</exception>
        [NotNull, ItemNotNull]
        public async Task<IEnumerable<ITaggedAudioFile>> EncodeAsync(
            [NotNull, ItemNotNull] IEnumerable<ITaggedAudioFile> audioFiles,
            CancellationToken cancellationToken,
            [CanBeNull] IProgress<ProgressToken> progress = null)
        {
            if (audioFiles == null) throw new ArgumentNullException(nameof(audioFiles));

            return await EncodeAsync(progress, cancellationToken, audioFiles.ToArray()).ConfigureAwait(false);
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="audioFiles">The audio files.</param>
        /// <returns>A new audio file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="audioFiles"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if one or more audio files are null.</exception>
        [NotNull, ItemNotNull]
        public async Task<IEnumerable<ITaggedAudioFile>> EncodeAsync(
            [NotNull, ItemNotNull] params ITaggedAudioFile[] audioFiles)
        {
            return await EncodeAsync(null, CancellationToken.None, audioFiles).ConfigureAwait(false);
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="audioFiles">The audio files.</param>
        /// <returns>A new audio file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="audioFiles"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if one or more audio files are null.</exception>
        [NotNull, ItemNotNull]
        public async Task<IEnumerable<ITaggedAudioFile>> EncodeAsync(
            CancellationToken cancellationToken,
            [NotNull, ItemNotNull] params ITaggedAudioFile[] audioFiles)
        {
            return await EncodeAsync(null, cancellationToken, audioFiles).ConfigureAwait(false);
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="progress">The progress queue, or <c>null</c>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="audioFiles">The audio files.</param>
        /// <returns>A new audio file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="audioFiles"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if one or more audio files are null.</exception>
        [NotNull, ItemNotNull]
        public async Task<IEnumerable<ITaggedAudioFile>> EncodeAsync(
            [CanBeNull] IProgress<ProgressToken> progress,
            CancellationToken cancellationToken,
            [NotNull, ItemNotNull] params ITaggedAudioFile[] audioFiles)
        {
            if (audioFiles == null) throw new ArgumentNullException(nameof(audioFiles));
            if (audioFiles.Any(audioFile => audioFile == null))
                throw new ArgumentException("One or more audio files are null.", nameof(audioFiles));

            progress?.Report(new ProgressToken
            {
                AudioFilesCompleted = 0,
                FramesCompleted = 0
            });

            var audioFilesCompleted = 0;
            var totalFramesCompleted = 0L;

            var encodeBlock = new TransformBlock<ITaggedAudioFile, ITaggedAudioFile>(audioFile =>
                {
                    // The output directory defaults to the AudioFile's current directory
                    var outputDirectory = _encodedDirectoryName?.ReplaceWith(audioFile.Metadata) ??
                                          Path.GetDirectoryName(audioFile.Path);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    var outputDirectoryInfo = Directory.CreateDirectory(outputDirectory);

                    // The output file names default to the input file names
                    var outputFileName = _encodedFileName?.ReplaceWith(audioFile.Metadata) ??
                                         Path.GetFileNameWithoutExtension(audioFile.Path);

                    var itemProgress = progress == null
                        ? null
                        : new SimpleProgress<int>(framesCompleted => progress.Report(new ProgressToken
                        {
                            // ReSharper disable once AccessToModifiedClosure
                            AudioFilesCompleted = audioFilesCompleted,
                            FramesCompleted = Interlocked.Add(ref totalFramesCompleted, framesCompleted)
                        }));

                    string tempOutputPath = null;
                    string finalOutputPath;
                    var encodingStarted = false;

                    try
                    {
                        var encoderExport = _encoderFactory.CreateExport();
                        FileStream outputStream = null;

                        try
                        {
                            tempOutputPath = finalOutputPath = Path.Combine(outputDirectoryInfo.FullName,
                                outputFileName + encoderExport.Value.FileExtension);

                            // If the output file already exists, write to a temporary file first
                            if (File.Exists(finalOutputPath))
                            {
                                if (!Overwrite)
                                    throw new IOException($"The file '{finalOutputPath}' already exists.");

                                tempOutputPath = Path.Combine(outputDirectoryInfo.FullName,
                                    Path.GetRandomFileName());
                            }

                            outputStream = File.Open(tempOutputPath, FileMode.OpenOrCreate);
                            encodingStarted = true;

                            // Copy the source metadata, so it can't be modified
                            encoderExport.Value.Initialize(
                                outputStream,
                                audioFile.Info,
                                new AudioMetadata(audioFile.Metadata),
                                Settings);

                            encoderExport.Value.ProcessSamples(
                                audioFile.Path,
                                itemProgress,
                                cancellationToken);

                            encoderExport.Value.Finish();
                        }
                        finally
                        {
                            // Dispose the encoder before closing the stream
                            encoderExport.Dispose();
                            outputStream?.Dispose();

                            Interlocked.Increment(ref audioFilesCompleted);
                        }
                    }
                    catch (Exception)
                    {
                        // Clean up output
                        if (encodingStarted)
                            File.Delete(tempOutputPath);
                        throw;
                    }

                    // If writing to temporary files, replace the originals now
                    if (!tempOutputPath.Equals(finalOutputPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(finalOutputPath);
                        File.Move(tempOutputPath, finalOutputPath);
                    }

                    return new TaggedAudioFile(finalOutputPath);
                },
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    SingleProducerConstrained = true,
                    CancellationToken = cancellationToken
                });

            var batchBlock = new BatchBlock<ITaggedAudioFile>(audioFiles.Length);
            encodeBlock.LinkTo(batchBlock, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var audioFile in audioFiles)
                await encodeBlock.SendAsync(audioFile, cancellationToken).ConfigureAwait(false);

            try
            {
                var result = await batchBlock.ReceiveAsync(cancellationToken).ConfigureAwait(false);

                progress?.Report(new ProgressToken
                {
                    AudioFilesCompleted = audioFilesCompleted,
                    FramesCompleted = totalFramesCompleted
                });

                return result;
            }
            catch (InvalidOperationException)
            {
                // Throw the real exception that caused the pipeline to cancel
                if (batchBlock.Completion.Exception != null)
                    throw batchBlock.Completion.Exception.GetBaseException();
                throw;
            }
        }
    }
}
