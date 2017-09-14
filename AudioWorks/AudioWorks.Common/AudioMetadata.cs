﻿using JetBrains.Annotations;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AudioWorks.Common
{
    [PublicAPI]
    public sealed class AudioMetadata
    {
        [NotNull] string _title = string.Empty;
        [NotNull] string _artist = string.Empty;
        [NotNull] string _album = string.Empty;
        [NotNull] string _comment = string.Empty;
        [NotNull] string _year = string.Empty;
        [NotNull] string _trackNumber = string.Empty;
        [NotNull] string _trackCount = string.Empty;

        [NotNull]
        public string Title
        {
            get => _title;
            set => _title = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        public string Artist
        {
            get => _artist;
            set => _artist = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        public string Album
        {
            get => _album;
            set => _album = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        public string Comment
        {
            get => _comment;
            set => _comment = value ?? throw new ArgumentNullException(nameof(value));
        }

        [NotNull]
        public string Year
        {
            get => _year;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value != string.Empty && !Regex.IsMatch(value, "^[1-9][0-9]{3}$"))
                    throw new ArgumentException("Year must be between 1000 and 9999.", nameof(value));
                _year = value;
            }
        }

        [NotNull]
        public string TrackNumber
        {
            get => _trackNumber;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value == string.Empty)
                    _trackNumber = value;
                else
                {
                    if (!int.TryParse(value, out var intValue) || intValue < 1 || intValue > 99)
                        throw new ArgumentException("TrackNumber must be between 1 and 99.", nameof(value));
                    _trackNumber = intValue.ToString("00", CultureInfo.InvariantCulture);
                }
            }
        }

        [NotNull]
        public string TrackCount
        {
            get => _trackCount;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value == string.Empty)
                    _trackCount = value;
                else
                {
                    if (!int.TryParse(value, out var intValue) || intValue < 1 || intValue > 99)
                        throw new ArgumentException("TrackCount must be between 1 and 99.", nameof(value));
                    _trackCount = intValue.ToString("00", CultureInfo.InvariantCulture);
                }
            }
        }
    }
}
