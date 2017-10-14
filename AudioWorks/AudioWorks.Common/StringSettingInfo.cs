﻿using System.Collections.Generic;
using JetBrains.Annotations;

namespace AudioWorks.Common
{
    [PublicAPI]
    public sealed class StringSettingInfo : SettingInfo
    {
        [NotNull, ItemNotNull]
        public IEnumerable<string> AcceptedValues { get; }

        public StringSettingInfo([NotNull, ItemNotNull] params string[] acceptedValues)
            : base(typeof(string))
        {
            AcceptedValues = acceptedValues;
        }
    }
}