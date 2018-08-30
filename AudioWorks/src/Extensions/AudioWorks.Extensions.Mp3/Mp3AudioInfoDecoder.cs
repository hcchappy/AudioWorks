﻿using System;
using System.IO;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Extensions.Mp3
{
    [AudioInfoDecoderExport(".mp3")]
    public sealed class Mp3AudioInfoDecoder : IAudioInfoDecoder
    {
        public AudioInfo ReadAudioInfo(FileStream stream)
        {
            using (var reader = new FrameReader(stream))
            {
                try
                {
                    var frameHeader = ReadFrameHeader(reader);

                    // Frame count is found in the optional Xing (most common) or VBRI header
                    var optionalHeader = ReadXingHeader(reader, frameHeader);
                    if (optionalHeader.Incomplete)
                        optionalHeader = ReadVbriHeader(reader);

                    return AudioInfo.CreateForLossy(
                        "MP3",
                        frameHeader.Channels,
                        frameHeader.SampleRate,
                        optionalHeader.FrameCount * frameHeader.SamplesPerFrame,
                        DetermineBitRate(frameHeader, optionalHeader));
                }
                catch (EndOfStreamException e)
                {
                    // If a frame sync couldn't be located, this isn't an MP3
                    throw new AudioInvalidException(e.Message, stream.Name);
                }
            }
        }

        [NotNull]
        static FrameHeader ReadFrameHeader([NotNull] FrameReader reader)
        {
#if NETCOREAPP2_1
            Span<byte> buffer = stackalloc byte[4];
#else
            var buffer = new byte[4];
#endif

            // Seek to the first valid frame header:
            FrameHeader result = null;
            do
            {
                reader.SeekToNextFrame();
#if NETCOREAPP2_1
                if (reader.Read(buffer) < 4)
#else
                if (reader.Read(buffer, 0, 4) < 4)
#endif
                    throw new AudioInvalidException("File is unexpectedly truncated.",
                        ((FileStream) reader.BaseStream).Name);

                try
                {
                    result = new FrameHeader(buffer);
                }
                catch (AudioException)
                {
                    // If the frame header appears wrong, it is probably just a bad sync
                }
            } while (result == null || !reader.VerifyFrameSync(result));
            return result;
        }

        static OptionalHeader ReadXingHeader([NotNull] FrameReader reader, [NotNull] FrameHeader header)
        {
            // Xing header (if present) is located after the side info
            reader.BaseStream.Position = reader.FrameStart + 4 + header.SideInfoLength;

            var result = new OptionalHeader();

            var headerId = reader.ReadHeaderId();
            if (!headerId.Equals("Xing", StringComparison.Ordinal) &&
                !headerId.Equals("Info", StringComparison.Ordinal))
                return result;

            // Both fields are optional, even if the header is present
            var flags = reader.ReadUInt32BigEndian();
            if ((flags & 1) == 1)
                result.FrameCount = reader.ReadUInt32BigEndian();
            if ((flags >> 1 & 1) == 1)
                result.ByteCount = reader.ReadUInt32BigEndian();

            return result;
        }

        static OptionalHeader ReadVbriHeader([NotNull] FrameReader reader)
        {
            // VBRI header (if present) is located 32 bytes past the frame header
            reader.BaseStream.Position = reader.FrameStart + 36;

            var result = new OptionalHeader();

            if (!reader.ReadHeaderId().Equals("VBRI", StringComparison.Ordinal)) return result;

            reader.BaseStream.Seek(6, SeekOrigin.Current);
            result.ByteCount = reader.ReadUInt32BigEndian();
            result.FrameCount = reader.ReadUInt32BigEndian();

            return result;
        }

        static int DetermineBitRate([NotNull] FrameHeader frameHeader, OptionalHeader optionalHeader)
        {
            // If the BitRate can't be calculated because of an incomplete or missing VBR header, assume CBR
            if (optionalHeader.Incomplete)
                return frameHeader.BitRate * 1000;

            return (int) Math.Round(
                optionalHeader.ByteCount * 8 /
                (optionalHeader.FrameCount * frameHeader.SamplesPerFrame / (double) frameHeader.SampleRate));
        }
    }
}
