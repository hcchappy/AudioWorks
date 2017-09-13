﻿using AudioWorks.Common;
using System.IO;

namespace AudioWorks.Extensions.Flac
{
    [AudioInfoDecoderExport(".flac")]
    sealed class FlacAudioInfoDecoder : IAudioInfoDecoder
    {
        public AudioInfo ReadAudioInfo(FileStream stream)
        {
            using (var decoder = new NativeAudioInfoDecoder(stream))
            {
                decoder.Initialize();
                if (!decoder.ProcessMetadata())
                    throw new AudioInvalidException(
                        $"libFLAC was unable to read the audio information: {decoder.GetState()}.", stream.Name);
                decoder.Finish();

                return decoder.AudioInfo ??
                       throw new AudioInvalidException("Audio information was not provided by libFLAC.", stream.Name);
            }
        }
    }
}