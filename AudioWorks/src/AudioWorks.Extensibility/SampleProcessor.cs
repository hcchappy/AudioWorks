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
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AudioWorks.Extensibility
{
    static class SampleProcessor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<float> source, Span<byte> destination, int bitsPerSample)
        {
            var multiplier = (int) Math.Pow(2, bitsPerSample - 1);
            var max = multiplier - 1;

            // Optimization - Vectorized implementation is significantly faster with AVX2 (256-bit SIMD)
            if (Vector.IsHardwareAccelerated)
            {
                var maxVector = new Vector<int>(max);
                var adjustmentVector = new Vector<int>(multiplier);
                var sourceVectors = MemoryMarshal.Cast<float, Vector<float>>(source);
                sourceVectors = sourceVectors.Slice(0, sourceVectors.Length - sourceVectors.Length % 4);
                var destinationVectors = MemoryMarshal.Cast<byte, Vector<byte>>(destination);

                for (int sourceIndex = 0, destinationIndex = 0; sourceIndex < sourceVectors.Length; destinationIndex++)
                {
                    var intVector1 =
                        (Vector<uint>) (Vector.Min(Vector.ConvertToInt32(sourceVectors[sourceIndex++] * multiplier),
                                            maxVector) - adjustmentVector);
                    var intVector2 =
                        (Vector<uint>) (Vector.Min(Vector.ConvertToInt32(sourceVectors[sourceIndex++] * multiplier),
                                            maxVector) - adjustmentVector);
                    var intVector3 =
                        (Vector<uint>) (Vector.Min(Vector.ConvertToInt32(sourceVectors[sourceIndex++] * multiplier),
                                            maxVector) - adjustmentVector);
                    var intVector4 =
                        (Vector<uint>) (Vector.Min(Vector.ConvertToInt32(sourceVectors[sourceIndex++] * multiplier),
                                            maxVector) - adjustmentVector);

                    var shortVector1 = Vector.Narrow(intVector1, intVector2);
                    var shortVector2 = Vector.Narrow(intVector3, intVector4);
                    destinationVectors[destinationIndex] = Vector.Narrow(shortVector1, shortVector2);
                }

                for (var sampleIndex = sourceVectors.Length * Vector<float>.Count;
                    sampleIndex < source.Length;
                    sampleIndex++)
                    destination[sampleIndex] = (byte) Math.Min(source[sampleIndex] * multiplier - multiplier, max);
            }
            else
                for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                    destination[sampleIndex] = (byte) Math.Min(source[sampleIndex] * multiplier - multiplier, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<float> source, Span<short> destination, int bitsPerSample)
        {
            var multiplier = (int) Math.Pow(2, bitsPerSample - 1);
            var max = multiplier - 1;

            // Optimization - Vectorized implementation is significantly faster with AVX2 (256-bit SIMD)
            if (Vector.IsHardwareAccelerated)
            {
                var maxVector = new Vector<int>(max);
                var sourceVectors = MemoryMarshal.Cast<float, Vector<float>>(source);
                sourceVectors = sourceVectors.Slice(0, sourceVectors.Length - sourceVectors.Length % 2);
                var destinationVectors = MemoryMarshal.Cast<short, Vector<short>>(destination);

                for (int sourceIndex = 0, destinationIndex = 0; sourceIndex < sourceVectors.Length; destinationIndex++)
                {
                    var intVector1 = Vector.Min(Vector.ConvertToInt32(sourceVectors[sourceIndex++] * multiplier),
                        maxVector);
                    var intVector2 = Vector.Min(Vector.ConvertToInt32(sourceVectors[sourceIndex++] * multiplier),
                        maxVector);

                    destinationVectors[destinationIndex] = Vector.Narrow(intVector1, intVector2);
                }

                for (var sampleIndex = sourceVectors.Length * Vector<float>.Count;
                    sampleIndex < source.Length;
                    sampleIndex++)
                    destination[sampleIndex] = (short) Math.Min(source[sampleIndex] * multiplier, max);
            }
            else
                for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                    destination[sampleIndex] = (short) Math.Min(source[sampleIndex] * multiplier, max);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<float> source, Span<Int24> destination, int bitsPerSample)
        {
            var multiplier = (int) Math.Pow(2, bitsPerSample - 1);
            var max = multiplier - 1;

            for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                destination[sampleIndex] = new Int24(Math.Min(source[sampleIndex] * multiplier, max));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<float> source, Span<int> destination, int bitsPerSample)
        {
            // Optimization - Vectorized implementation is significantly faster with AVX2 (256-bit SIMD)
            if (Vector.IsHardwareAccelerated && bitsPerSample < 32)
            {
                var multiplier = (int) Math.Pow(2, bitsPerSample - 1);
                var max = multiplier - 1;
                var maxVector = new Vector<int>(max);
                var sourceVectors = MemoryMarshal.Cast<float, Vector<float>>(source);
                var destinationVectors = MemoryMarshal.Cast<int, Vector<int>>(destination);

                for (var vectorIndex = 0; vectorIndex < sourceVectors.Length; vectorIndex++)
                    destinationVectors[vectorIndex] =
                        Vector.Min(Vector.ConvertToInt32(sourceVectors[vectorIndex] * multiplier), maxVector);

                for (var sampleIndex = sourceVectors.Length * Vector<float>.Count;
                    sampleIndex < source.Length;
                    sampleIndex++)
                    destination[sampleIndex] = (short) Math.Min(source[sampleIndex] * multiplier, max);
            }
            else
            {
                // At 32 bits per sample, multiplier > int.MaxValue
                var multiplier = (uint) Math.Pow(2, bitsPerSample - 1);
                var max = (int) multiplier - 1;

                for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                    try
                    {
                        destination[sampleIndex] = Math.Min(checked((int) (source[sampleIndex] * multiplier)), max);
                    }
                    catch (OverflowException)
                    {
                        // Can occur at 32 bitsPerSample and +1.0
                        destination[sampleIndex] = max;
                    }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<byte> source, Span<float> destination, int bitsPerSample)
        {
            var adjustment = (float) Math.Pow(2, bitsPerSample - 1);
            var multiplier = 1 / (float) Math.Pow(2, bitsPerSample - 1);

            // Optimization - Vectorized implementation is significantly faster with AVX2 (256-bit SIMD)
            if (Vector.IsHardwareAccelerated)
            {
                var adjustmentVector = new Vector<float>(adjustment);
                var sourceVectors = MemoryMarshal.Cast<byte, Vector<byte>>(source);
                var destinationVectors = MemoryMarshal.Cast<float, Vector<float>>(destination);

                for (int sourceIndex = 0, destinationIndex = 0; sourceIndex < sourceVectors.Length; sourceIndex++)
                {
                    Vector.Widen(sourceVectors[sourceIndex], out var shortVector1, out var shortVector2);
                    Vector.Widen(shortVector1, out var intVector1, out var intVector2);
                    Vector.Widen(shortVector2, out var intVector3, out var intVector4);

                    destinationVectors[destinationIndex++] =
                        (Vector.ConvertToSingle(intVector1) - adjustmentVector) * multiplier;
                    destinationVectors[destinationIndex++] =
                        (Vector.ConvertToSingle(intVector2) - adjustmentVector) * multiplier;
                    destinationVectors[destinationIndex++] =
                        (Vector.ConvertToSingle(intVector3) - adjustmentVector) * multiplier;
                    destinationVectors[destinationIndex++] =
                        (Vector.ConvertToSingle(intVector4) - adjustmentVector) * multiplier;
                }

                for (var sampleIndex = sourceVectors.Length * Vector<byte>.Count;
                    sampleIndex < source.Length;
                    sampleIndex++)
                    destination[sampleIndex] = (source[sampleIndex] - adjustment) * multiplier;
            }
            else
                for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                    destination[sampleIndex] = (source[sampleIndex] - adjustment) * multiplier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<short> source, Span<float> destination, int bitsPerSample)
        {
            var multiplier = 1 / (float) Math.Pow(2, bitsPerSample - 1);

            // Optimization - Vectorized implementation is significantly faster with AVX2 (256-bit SIMD)
            if (Vector.IsHardwareAccelerated)
            {
                var sourceVectors = MemoryMarshal.Cast<short, Vector<short>>(source);
                var destinationVectors = MemoryMarshal.Cast<float, Vector<float>>(destination);

                for (int sourceIndex = 0, destinationIndex = 0; sourceIndex < sourceVectors.Length; sourceIndex++)
                {
                    Vector.Widen(sourceVectors[sourceIndex], out var destinationVector1, out var destinationVector2);

                    destinationVectors[destinationIndex++] = Vector.ConvertToSingle(destinationVector1) * multiplier;
                    destinationVectors[destinationIndex++] = Vector.ConvertToSingle(destinationVector2) * multiplier;
                }

                for (var sampleIndex = sourceVectors.Length * Vector<short>.Count;
                    sampleIndex < source.Length;
                    sampleIndex++)
                    destination[sampleIndex] = source[sampleIndex] * multiplier;
            }
            else
                for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                    destination[sampleIndex] = source[sampleIndex] * multiplier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<Int24> source, Span<float> destination, int bitsPerSample)
        {
            var multiplier = 1 / (float) Math.Pow(2, bitsPerSample - 1);

            for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                destination[sampleIndex] = source[sampleIndex] * multiplier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Convert(ReadOnlySpan<int> source, Span<float> destination, int bitsPerSample)
        {
            var multiplier = 1 / (float) Math.Pow(2, bitsPerSample - 1);

            // Optimization - Vectorized implementation is significantly faster with AVX2 (256-bit SIMD)
            if (Vector.IsHardwareAccelerated)
            {
                var sourceVectors = MemoryMarshal.Cast<int, Vector<int>>(source);
                var destinationVectors = MemoryMarshal.Cast<float, Vector<float>>(destination);

                for (var vectorIndex = 0; vectorIndex < sourceVectors.Length; vectorIndex++)
                    destinationVectors[vectorIndex] = Vector.ConvertToSingle(sourceVectors[vectorIndex]) * multiplier;

                for (var sampleIndex = sourceVectors.Length * Vector<int>.Count;
                    sampleIndex < source.Length;
                    sampleIndex++)
                    destination[sampleIndex] = source[sampleIndex] * multiplier;
            }
            else
                for (var sampleIndex = 0; sampleIndex < source.Length; sampleIndex++)
                    destination[sampleIndex] = source[sampleIndex] * multiplier;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void Interleave(
            ReadOnlySpan<float> leftSource,
            ReadOnlySpan<float> rightSource,
            Span<float> destination)
        {
            // Optimization - Unsafe implementation is faster
            fixed (float* destinationAddress = destination)
            {
                var destinationPointer = destinationAddress;

                for (var frameIndex = 0; frameIndex < leftSource.Length; frameIndex++)
                {
                    *destinationPointer++ = leftSource[frameIndex];
                    *destinationPointer++ = rightSource[frameIndex];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe void DeInterleave(
            ReadOnlySpan<float> source,
            Span<float> leftDestination,
            Span<float> rightDestination)
        {
            // Optimization - Unsafe implementation is faster
            fixed (float* sourceAddress = source)
            {
                var sourcePointer = sourceAddress;

                for (var frameIndex = 0; frameIndex < source.Length / 2; frameIndex++)
                {
                    leftDestination[frameIndex] = *sourcePointer++;
                    rightDestination[frameIndex] = *sourcePointer++;
                }
            }
        }
    }
}