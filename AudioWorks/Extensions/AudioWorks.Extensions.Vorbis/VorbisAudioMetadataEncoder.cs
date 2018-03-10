﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Extensions.Vorbis
{
    [AudioMetadataEncoderExport(".ogg")]
    public sealed class VorbisAudioMetadataEncoder : IAudioMetadataEncoder
    {
        public SettingInfoDictionary SettingInfo { get; } = new SettingInfoDictionary();

        public void WriteMetadata(FileStream stream, AudioMetadata metadata, SettingDictionary settings)
        {
            // This buffer is used for both reading and writing:
            var buffer = new byte[4096];

            using (var tempStream = new MemoryStream())
            {
                OggStream inputOggStream = null;
                OggStream outputOggStream = null;

                try
                {
                    using (var sync = new OggSync())
                    {
                        OggPage inPage;

                        do
                        {
                            // Read from the buffer into a page
                            while (!sync.PageOut(out inPage))
                            {
                                var nativeBuffer = sync.Buffer(buffer.Length);
                                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                                Marshal.Copy(buffer, 0, nativeBuffer, bytesRead);
                                sync.Wrote(bytesRead);
                            }

                            if (inputOggStream == null)
                                inputOggStream = new OggStream(SafeNativeMethods.OggPageSerialNo(ref inPage));
                            if (outputOggStream == null)
                                outputOggStream = new OggStream(inputOggStream.SerialNumber);

                            inputOggStream.PageIn(ref inPage);

                            while (inputOggStream.PacketOut(out var packet))
                            {
                                // Substitute the new comment packet
                                if (packet.PacketNumber == 1)
                                    using (var adapter = new MetadataToVorbisCommentAdapter(metadata))
                                    {
                                        adapter.HeaderOut(out var commentPacket);
                                        outputOggStream.PacketIn(ref commentPacket);
                                    }
                                else
                                    outputOggStream.PacketIn(ref packet);

                                // Page out each packet, flushing at the end of the header
                                if (packet.PacketNumber == 2)
                                    while (outputOggStream.Flush(out var outPage))
                                        WritePage(outPage, tempStream, buffer);
                                else
                                    while (outputOggStream.PageOut(out var outPage))
                                        WritePage(outPage, tempStream, buffer);
                            }
                        } while (!SafeNativeMethods.OggPageEos(ref inPage));

                        // Once the end of the input is reached, overwrite the original file and return
                        stream.Position = 0;
                        stream.SetLength(tempStream.Length);
                        tempStream.Position = 0;
                        tempStream.CopyTo(stream);
                    }
                }
                finally
                {
                    inputOggStream?.Dispose();
                    outputOggStream?.Dispose();
                }
            }
        }

        static void WritePage(OggPage page, [NotNull] Stream stream, [NotNull] byte[] buffer)
        {
#if (WINDOWS)
            WriteFromUnmanaged(page.Header, page.HeaderLength, stream, buffer);
            WriteFromUnmanaged(page.Body, page.BodyLength, stream, buffer);
#else
            WriteFromUnmanaged(page.Header, (int) page.HeaderLength, stream, buffer);
            WriteFromUnmanaged(page.Body, (int) page.BodyLength, stream, buffer);
#endif
        }

        static void WriteFromUnmanaged(IntPtr location, int length, [NotNull] Stream stream, [NotNull] byte[] buffer)
        {
            var offset = 0;
            while (offset < length)
            {
                var bytesCopied = Math.Min(length - offset, buffer.Length);
                Marshal.Copy(IntPtr.Add(location, offset), buffer, 0, bytesCopied);
                stream.Write(buffer, 0, bytesCopied);
                offset += bytesCopied;
            }
        }
    }
}
