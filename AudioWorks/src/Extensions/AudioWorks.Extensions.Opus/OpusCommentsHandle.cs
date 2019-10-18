﻿/* Copyright © 2019 Jeremy Herbison

This file is part of AudioWorks.

AudioWorks is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public
License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later
version.

AudioWorks is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied
warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
details.

You should have received a copy of the GNU Affero General Public License along with AudioWorks. If not, see
<https://www.gnu.org/licenses/>. */

using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using Microsoft.Win32.SafeHandles;

namespace AudioWorks.Extensions.Opus
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    sealed class OpusCommentsHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal OpusCommentsHandle()
            : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected override bool ReleaseHandle()
        {
            SafeNativeMethods.OpusEncoderCommentsDestroy(handle);
            return true;
        }
    }
}