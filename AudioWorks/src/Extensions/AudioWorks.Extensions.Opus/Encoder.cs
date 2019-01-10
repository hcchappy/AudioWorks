﻿/* Copyright © 2019 Jeremy Herbison

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
using System.IO;
using System.Runtime.InteropServices;
using AudioWorks.Common;
using JetBrains.Annotations;

namespace AudioWorks.Extensions.Opus
{
    sealed class Encoder : IDisposable
    {
        [NotNull] readonly OpusCommentsHandle _commentsHandle;
        [NotNull] readonly OpusEncoderHandle _handle;
        readonly int _channels;

        internal Encoder([NotNull] Stream stream, int sampleRate, int channels)
        {
            var callbacks = InitializeCallbacks(stream);
            _commentsHandle = SafeNativeMethods.OpusEncoderCommentsCreate();
            _handle = SafeNativeMethods.OpusEncoderCreateCallbacks(ref callbacks, IntPtr.Zero, _commentsHandle,
                sampleRate, channels, 0, out var error);
            if (error != 0)
                throw new AudioEncodingException($"Opus encountered error '{error}' during initialization.");
            _channels = channels;
        }

        internal void SetSerialNumber(int serialNumber)
        {
            var error = SafeNativeMethods.OpusEncoderControl(_handle, EncoderControlRequest.SetSerialNumber, serialNumber);
            if (error != 0)
                throw new AudioEncodingException($"Opus encountered error '{error}' while setting the serial number.");
        }

        internal void Write(ReadOnlySpan<float> interleavedSamples)
        {
            var error = SafeNativeMethods.OpusEncoderWriteFloat(
                _handle,
                MemoryMarshal.GetReference(interleavedSamples),
                interleavedSamples.Length / _channels);
            if (error != 0)
                throw new AudioEncodingException($"Opus encountered error '{error}' while encoding.");
        }

        internal void Drain()
        {
            var error = SafeNativeMethods.OpusEncoderDrain(_handle);
            if (error != 0)
                throw new AudioEncodingException($"Opus encountered error '{error}' while finishing encoding.");
        }

        public void Dispose()
        {
            _handle.Dispose();
            _commentsHandle.Dispose();
        }

        static OpusEncoderCallbacks InitializeCallbacks([NotNull] Stream stream)
        {
            return new OpusEncoderCallbacks
            {
                Write = (userData, buffer, length) =>
                {
                    stream.Write(buffer, 0, length);
                    return 0;
                },

                // Leave the stream open
                Close = userData => 0
            };
        }
    }
}
