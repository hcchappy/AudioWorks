﻿using AudioWorks.Common;
using System;
using System.IO;
using System.Linq;

namespace AudioWorks.Extensions.Mp4
{
    [AudioInfoDecoderExport(".m4a")]
    sealed class Mp4AudioInfoDecoder : IAudioInfoDecoder
    {
        public AudioInfo ReadAudioInfo(FileStream stream)
        {
            try
            {
                var mp4 = new Mp4(stream);

                var dataSize = mp4.GetChildAtomInfo().Single(atom => atom.FourCc == "mdat").Size;

                mp4.DescendToAtom("moov", "trak", "mdia", "minf", "stbl", "stts");
                var stts = new SttsAtom(mp4.ReadAtom(mp4.CurrentAtom));

                var sampleCount = stts.PacketCount * stts.PacketSize;

                mp4.DescendToAtom("moov", "trak", "mdia", "minf", "stbl", "stsd", "mp4a", "esds");
                var esds = new EsdsAtom(mp4.ReadAtom(mp4.CurrentAtom));
                if (esds.IsAac)
                    return AudioInfo.CreateForLossy(
                        "AAC",
                        esds.Channels,
                        (int) esds.SampleRate,
                        sampleCount,
                        CalculateBitRate(dataSize, sampleCount, esds.SampleRate));

                // Apple Lossless files have their own atom for storing audio info
                if (!mp4.DescendToAtom("moov", "trak", "mdia", "minf", "stbl", "stsd", "alac"))
                    throw new AudioUnsupportedException("Only AAC and ALAC files are supported.", stream.Name);

                var alac = new AlacAtom(mp4.ReadAtom(mp4.CurrentAtom));
                return AudioInfo.CreateForLossless(
                    "ALAC",
                    alac.Channels,
                    alac.BitsPerSample,
                    (int) alac.SampleRate,
                    sampleCount);
            }
            catch (InvalidOperationException)
            {
                throw new AudioInvalidException("The mdat atom is missing.", stream.Name);
            }
            catch (EndOfStreamException e)
            {
                throw new AudioInvalidException(e.Message, stream.Name);
            }
        }

        static int CalculateBitRate(uint byteCount, uint sampleCount, uint sampleRate)
        {
            return (int) Math.Round(byteCount * 8 / (sampleCount / (double) sampleRate));
        }
    }
}
