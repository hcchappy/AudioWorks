﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioWorks.Common;
using AudioWorks.Extensions;
using JetBrains.Annotations;

namespace AudioWorks.Api
{
    /// <summary>
    /// Encodes one or more audio files in a new audio format.
    /// </summary>
    [PublicAPI]
    public sealed class AudioFileEncoder
    {
        [CanBeNull] readonly MetadataSubstituter _fileNameSubstituter;
        [CanBeNull] readonly MetadataSubstituter _directoryNameSubstituter;
        [NotNull] readonly ExportFactory<IAudioEncoder> _encoderFactory;
        [NotNull] readonly SettingDictionary _settings;
        readonly bool _overwrite;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFileEncoder"/> class.
        /// </summary>
        /// <param name="name">The name of the encoder.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="encodedDirectoryName">The encoded directory name, or null.</param>
        /// <param name="encodedFileName">The encode file name, or null.</param>
        /// <param name="overwrite">if set to <c>true</c>, any existing file will be overwritten.</param>
        /// <exception cref="ArgumentNullException">Thrown if <see paramref="name"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <see paramref="name"/> is not the name of an available encoder.
        /// </exception>
        public AudioFileEncoder(
            [NotNull] string name,
            [CanBeNull] SettingDictionary settings = null,
            [CanBeNull] string encodedDirectoryName = null,
            [CanBeNull] string encodedFileName = null,
            bool overwrite = false)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _encoderFactory = ExtensionProvider.GetFactories<IAudioEncoder>("Name", name).SingleOrDefault() ??
                              throw new ArgumentException($"No '{name}' encoder is available.", nameof(name));

            if (settings != null)
            {
                // Make sure the provided settings are clean
                AudioEncoderManager.GetSettingInfo(name).ValidateSettings(settings);
                _settings = settings;
            }
            else
                _settings = new SettingDictionary();

            if (encodedDirectoryName != null)
                _directoryNameSubstituter = new DirectoryNameSubstituter(encodedDirectoryName);
            if (encodedFileName != null)
                _fileNameSubstituter = new FileNameSubstituter(encodedFileName);
            _overwrite = overwrite;
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="audioFiles">The audio files.</param>
        /// <returns>A new audio file.</returns>
        [NotNull, ItemNotNull]
        public async Task<IEnumerable<ITaggedAudioFile>> EncodeAsync(
            [NotNull, ItemNotNull] params ITaggedAudioFile[] audioFiles)
        {
            return await EncodeAsync(CancellationToken.None, audioFiles).ConfigureAwait(false);
        }

        /// <summary>
        /// Encodes the specified audio files.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="audioFiles">The audio files.</param>
        /// <returns>A new audio file.</returns>
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

            var encoderExports = new Export<IAudioEncoder>[audioFiles.Length];
            var tempOutputPaths = new string[audioFiles.Length];
            var finalOutputPaths = new string[audioFiles.Length];
            var outputStreams = new FileStream[audioFiles.Length];
            var audioFilesCompleted = 0;
            var totalFramesCompleted = 0;

            try
            {
                try
                {
                    var processTasks = new Task[audioFiles.Length];

                    for (var i = 0; i < audioFiles.Length; i++)
                    {
                        // The output directory defaults to the audiofile's current directory
                        var outputDirectory = _directoryNameSubstituter?.Substitute(audioFiles[i].Metadata) ??
                                              Path.GetDirectoryName(audioFiles[i].Path);

                        // ReSharper disable once AssignNullToNotNullAttribute
                        Directory.CreateDirectory(outputDirectory);

                        // The output file names default to the input file names
                        var outputFileName = _fileNameSubstituter?.Substitute(audioFiles[i].Metadata) ??
                                             Path.GetFileNameWithoutExtension(audioFiles[i].Path);

                        encoderExports[i] = _encoderFactory.CreateExport();

                        tempOutputPaths[i] = finalOutputPaths[i] = Path.Combine(outputDirectory,
                            outputFileName + encoderExports[i].Value.FileExtension);

                        // If the output file already exists, write to a temporary file first
                        if (File.Exists(finalOutputPaths[i]))
                        {
                            if (!_overwrite)
                                throw new IOException($"The file '{finalOutputPaths[i]}' already exists.");

                            tempOutputPaths[i] = Path.Combine(outputDirectory, Path.GetRandomFileName());
                        }

                        outputStreams[i] = File.Open(tempOutputPaths[i], FileMode.OpenOrCreate);

                        // Copy the source metadata, so it can't be modified
                        encoderExports[i].Value.Initialize(
                            outputStreams[i],
                            audioFiles[i].Info,
                            new AudioMetadata(audioFiles[i].Metadata),
                            _settings);

                        var i1 = i;
                        var itemProgress = progress == null
                            ? null
                            : new SimpleProgress<int>(framesCompleted => progress.Report(new ProgressToken
                            {
                                AudioFilesCompleted = audioFilesCompleted,
                                FramesCompleted = Interlocked.Add(ref totalFramesCompleted, framesCompleted)
                            }));

                        processTasks[i] = Task.Run(() =>
                        {
                            encoderExports[i1].Value.ProcessSamples(
                                audioFiles[i1].Path,
                                itemProgress,
                                cancellationToken);

                            encoderExports[i1].Value.Finish();

                            Interlocked.Increment(ref audioFilesCompleted);
                        }, cancellationToken);
                    }

                    await Task.WhenAll(processTasks).ConfigureAwait(false);
                }
                finally
                {
                    // Dispose the encoders before closing the streams
                    foreach (var encoderExport in encoderExports)
                        encoderExport?.Dispose();
                    foreach (var outputStream in outputStreams)
                        outputStream?.Dispose();
                }
            }
            catch (Exception)
            {
                // Clean up on error
                foreach (var tempOutputPath in tempOutputPaths.Where(path => path != null))
                    File.Delete(tempOutputPath);
                throw;
            }

            // If writing to temporary files, replace the originals now
            for (var i = 0; i < audioFiles.Length; i++)
                if (!tempOutputPaths[i].Equals(finalOutputPaths[i], StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(finalOutputPaths[i]);
                    File.Move(tempOutputPaths[i], finalOutputPaths[i]);
                }

            progress?.Report(new ProgressToken
            {
                AudioFilesCompleted = audioFilesCompleted,
                FramesCompleted = totalFramesCompleted
            });

            return finalOutputPaths.Select(path => new TaggedAudioFile(path));
        }
    }
}
