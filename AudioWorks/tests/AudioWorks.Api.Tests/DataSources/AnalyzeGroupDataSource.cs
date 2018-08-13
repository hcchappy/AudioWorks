﻿using System.Collections.Generic;
using AudioWorks.Api.Tests.DataTypes;
using JetBrains.Annotations;

namespace AudioWorks.Api.Tests.DataSources
{
    public static class AnalyzeGroupDataSource
    {
        [NotNull, ItemNotNull] static readonly List<object[]> _data = new List<object[]>
        {
            // 8000Hz Stereo, default (simple) peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 8-bit 8000Hz Stereo.wav",
                    "Track 2 LPCM 8-bit 8000Hz Stereo.wav",
                    "Track 3 LPCM 8-bit 8000Hz Stereo.wav"
                },
                "ReplayGain",
                null,
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.820313",
                        AlbumPeak = "1.000000",
                        TrackGain = "-1.36",
                        AlbumGain = "-6.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000000",
                        AlbumPeak = "1.000000",
                        TrackGain = "-6.49",
                        AlbumGain = "-6.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000000",
                        AlbumPeak = "1.000000",
                        TrackGain = "-7.22",
                        AlbumGain = "-6.11"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.820313",
                        AlbumPeak = "1.000000",
                        TrackGain = "-1.36",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000000",
                        AlbumPeak = "1.000000",
                        TrackGain = "-6.49",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000000",
                        AlbumPeak = "1.000000",
                        TrackGain = "-7.22",
                        AlbumGain = "-5.90"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.820313",
                        AlbumPeak = "1.000000",
                        TrackGain = "-1.36",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000000",
                        AlbumPeak = "1.000000",
                        TrackGain = "-6.49",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000000",
                        AlbumPeak = "1.000000",
                        TrackGain = "-7.22",
                        AlbumGain = "-5.90"
                    }
                }
#endif
            },

            // 8000Hz Stereo, interpolated peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 8-bit 8000Hz Stereo.wav",
                    "Track 2 LPCM 8-bit 8000Hz Stereo.wav",
                    "Track 3 LPCM 8-bit 8000Hz Stereo.wav"
                },
                "ReplayGain",
                new TestSettingDictionary
                {
                    ["PeakAnalysis"] = "Interpolated"
                },
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.820708",
                        AlbumPeak = "1.001060",
                        TrackGain = "-1.36",
                        AlbumGain = "-6.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.001060",
                        AlbumPeak = "1.001060",
                        TrackGain = "-6.49",
                        AlbumGain = "-6.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.001014",
                        AlbumPeak = "1.001060",
                        TrackGain = "-7.22",
                        AlbumGain = "-6.11"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.820409",
                        AlbumPeak = "1.001346",
                        TrackGain = "-1.36",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.001346",
                        AlbumPeak = "1.001346",
                        TrackGain = "-6.49",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000070",
                        AlbumPeak = "1.001346",
                        TrackGain = "-7.22",
                        AlbumGain = "-5.90"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.820409",
                        AlbumPeak = "1.001346",
                        TrackGain = "-1.36",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.001346",
                        AlbumPeak = "1.001346",
                        TrackGain = "-6.49",
                        AlbumGain = "-5.90"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000070",
                        AlbumPeak = "1.001346",
                        TrackGain = "-7.22",
                        AlbumGain = "-5.90"
                    }
                }
#endif
            },

            // 44100Hz Mono, default (simple) peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 44100Hz Mono.wav",
                    "Track 2 LPCM 16-bit 44100Hz Mono.wav",
                    "Track 3 LPCM 16-bit 44100Hz Mono.wav"
                },
                "ReplayGain",
                null,
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.590515",
                        AlbumPeak = "0.965790",
                        TrackGain = "4.24",
                        AlbumGain = "-2.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.914001",
                        AlbumPeak = "0.965790",
                        TrackGain = "-2.03",
                        AlbumGain = "-2.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.965790",
                        AlbumPeak = "0.965790",
                        TrackGain = "-3.84",
                        AlbumGain = "-2.11"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.590515",
                        AlbumPeak = "0.965790",
                        TrackGain = "4.24",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.914001",
                        AlbumPeak = "0.965790",
                        TrackGain = "-2.03",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.965790",
                        AlbumPeak = "0.965790",
                        TrackGain = "-3.84",
                        AlbumGain = "-1.83"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.590515",
                        AlbumPeak = "0.965790",
                        TrackGain = "4.24",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.914001",
                        AlbumPeak = "0.965790",
                        TrackGain = "-2.03",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.965790",
                        AlbumPeak = "0.965790",
                        TrackGain = "-3.84",
                        AlbumGain = "-1.83"
                    }
                }
#endif
            },

            // 44100Hz Mono, interpolated peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 44100Hz Mono.wav",
                    "Track 2 LPCM 16-bit 44100Hz Mono.wav",
                    "Track 3 LPCM 16-bit 44100Hz Mono.wav"
                },
                "ReplayGain",
                new TestSettingDictionary
                {
                    ["PeakAnalysis"] = "Interpolated"
                },
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.590625",
                        AlbumPeak = "0.965800",
                        TrackGain = "4.24",
                        AlbumGain = "-2.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.914001",
                        AlbumPeak = "0.965800",
                        TrackGain = "-2.03",
                        AlbumGain = "-2.11"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.965800",
                        AlbumPeak = "0.965800",
                        TrackGain = "-3.84",
                        AlbumGain = "-2.11"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.591049",
                        AlbumPeak = "0.966563",
                        TrackGain = "4.24",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.914502",
                        AlbumPeak = "0.966563",
                        TrackGain = "-2.03",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.966563",
                        AlbumPeak = "0.966563",
                        TrackGain = "-3.84",
                        AlbumGain = "-1.83"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.591049",
                        AlbumPeak = "0.966563",
                        TrackGain = "4.24",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.914502",
                        AlbumPeak = "0.966563",
                        TrackGain = "-2.03",
                        AlbumGain = "-1.83"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.966563",
                        AlbumPeak = "0.966563",
                        TrackGain = "-3.84",
                        AlbumGain = "-1.83"
                    }
                }
#endif
            },

            // 44100Hz Stereo, default (simple) peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 44100Hz Stereo.wav",
                    "Track 2 LPCM 16-bit 44100Hz Stereo.wav",
                    "Track 3 LPCM 16-bit 44100Hz Stereo.wav"
                },
                "ReplayGain",
                null,
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.813904",
                        AlbumPeak = "0.999664",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999664",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999664",
                        AlbumPeak = "0.999664",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.91"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.813904",
                        AlbumPeak = "0.999664",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999664",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999664",
                        AlbumPeak = "0.999664",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.70"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.813904",
                        AlbumPeak = "0.999664",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999664",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999664",
                        AlbumPeak = "0.999664",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.70"
                    }
                }
#endif
            },

            // 44100Hz Stereo, interpolated peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 44100Hz Stereo.wav",
                    "Track 2 LPCM 16-bit 44100Hz Stereo.wav",
                    "Track 3 LPCM 16-bit 44100Hz Stereo.wav"
                },
                "ReplayGain",
                new TestSettingDictionary
                {
                    ["PeakAnalysis"] = "Interpolated"
                },
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814064",
                        AlbumPeak = "0.999686",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999686",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999686",
                        AlbumPeak = "0.999686",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.91"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814763",
                        AlbumPeak = "1.000577",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.998429",
                        AlbumPeak = "1.000577",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000577",
                        AlbumPeak = "1.000577",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.70"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814763",
                        AlbumPeak = "1.000577",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.998429",
                        AlbumPeak = "1.000577",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000577",
                        AlbumPeak = "1.000577",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.70"
                    }
                }
#endif
            },

            // 48000Hz Stereo, default (simple) peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 48000Hz Stereo.wav",
                    "Track 2 LPCM 16-bit 48000Hz Stereo.wav",
                    "Track 3 LPCM 16-bit 48000Hz Stereo.wav"
                },
                "ReplayGain",
                null,
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814056",
                        AlbumPeak = "0.999634",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997467",
                        AlbumPeak = "0.999634",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999634",
                        AlbumPeak = "0.999634",
                        TrackGain = "-7.02",
                        AlbumGain = "-5.91"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814056",
                        AlbumPeak = "0.999634",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997467",
                        AlbumPeak = "0.999634",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999634",
                        AlbumPeak = "0.999634",
                        TrackGain = "-7.02",
                        AlbumGain = "-5.69"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814056",
                        AlbumPeak = "0.999634",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997467",
                        AlbumPeak = "0.999634",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999634",
                        AlbumPeak = "0.999634",
                        TrackGain = "-7.02",
                        AlbumGain = "-5.69"
                    }
                }
#endif
            },

            // 48000Hz Stereo, interpolated peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 48000Hz Stereo.wav",
                    "Track 2 LPCM 16-bit 48000Hz Stereo.wav",
                    "Track 3 LPCM 16-bit 48000Hz Stereo.wav"
                },
                "ReplayGain",
                new TestSettingDictionary
                {
                    ["PeakAnalysis"] = "Interpolated"
                },
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814071",
                        AlbumPeak = "0.999712",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997490",
                        AlbumPeak = "0.999712",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999712",
                        AlbumPeak = "0.999712",
                        TrackGain = "-7.02",
                        AlbumGain = "-5.91"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814716",
                        AlbumPeak = "1.000568",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.998379",
                        AlbumPeak = "1.000568",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000568",
                        AlbumPeak = "1.000568",
                        TrackGain = "-7.02",
                        AlbumGain = "-5.69"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814716",
                        AlbumPeak = "1.000568",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.998379",
                        AlbumPeak = "1.000568",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.69"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "1.000568",
                        AlbumPeak = "1.000568",
                        TrackGain = "-7.02",
                        AlbumGain = "-5.69"
                    }
                }
#endif
            },

            // 96000Hz Stereo, default (simple) peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 24-bit 96000Hz Stereo.wav",
                    "Track 2 LPCM 24-bit 96000Hz Stereo.wav",
                    "Track 3 LPCM 24-bit 96000Hz Stereo.wav"
                },
                "ReplayGain",
                null,
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814049",
                        AlbumPeak = "0.999651",
                        TrackGain = "-1.13",
                        AlbumGain = "-5.89"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997602",
                        AlbumPeak = "0.999651",
                        TrackGain = "-6.26",
                        AlbumGain = "-5.89"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999651",
                        AlbumPeak = "0.999651",
                        TrackGain = "-7.00",
                        AlbumGain = "-5.89"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814049",
                        AlbumPeak = "0.999651",
                        TrackGain = "-1.13",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997602",
                        AlbumPeak = "0.999651",
                        TrackGain = "-6.26",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999651",
                        AlbumPeak = "0.999651",
                        TrackGain = "-7.00",
                        AlbumGain = "-5.67"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814049",
                        AlbumPeak = "0.999651",
                        TrackGain = "-1.13",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997602",
                        AlbumPeak = "0.999651",
                        TrackGain = "-6.26",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999651",
                        AlbumPeak = "0.999651",
                        TrackGain = "-7.00",
                        AlbumGain = "-5.67"
                    }
                }
#endif
            },

            // 96000Hz Stereo, interpolated peaks
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 24-bit 96000Hz Stereo.wav",
                    "Track 2 LPCM 24-bit 96000Hz Stereo.wav",
                    "Track 3 LPCM 24-bit 96000Hz Stereo.wav"
                },
                "ReplayGain",
                new TestSettingDictionary
                {
                    ["PeakAnalysis"] = "Interpolated"
                },
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814095",
                        AlbumPeak = "0.999651",
                        TrackGain = "-1.13",
                        AlbumGain = "-5.89"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997607",
                        AlbumPeak = "0.999651",
                        TrackGain = "-6.26",
                        AlbumGain = "-5.89"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999651",
                        AlbumPeak = "0.999651",
                        TrackGain = "-7.00",
                        AlbumGain = "-5.89"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814189",
                        AlbumPeak = "0.999758",
                        TrackGain = "-1.13",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997720",
                        AlbumPeak = "0.999758",
                        TrackGain = "-6.26",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999758",
                        AlbumPeak = "0.999758",
                        TrackGain = "-7.00",
                        AlbumGain = "-5.67"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.814189",
                        AlbumPeak = "0.999758",
                        TrackGain = "-1.13",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997720",
                        AlbumPeak = "0.999758",
                        TrackGain = "-6.26",
                        AlbumGain = "-5.67"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999758",
                        AlbumPeak = "0.999758",
                        TrackGain = "-7.00",
                        AlbumGain = "-5.67"
                    }
                }
#endif
            },

            // 44100Hz Stereo, simple peaks (explicit)
            new object[]
            {
                new[]
                {
                    "Track 1 LPCM 16-bit 44100Hz Stereo.wav",
                    "Track 2 LPCM 16-bit 44100Hz Stereo.wav",
                    "Track 3 LPCM 16-bit 44100Hz Stereo.wav"
                },
                "ReplayGain",
                new TestSettingDictionary
                {
                    ["PeakAnalysis"] = "Simple"
                },
#if LINUX
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.813904",
                        AlbumPeak = "0.999664",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999664",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.91"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999664",
                        AlbumPeak = "0.999664",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.91"
                    }
                },
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.813904",
                        AlbumPeak = "0.999664",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999664",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999664",
                        AlbumPeak = "0.999664",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.70"
                    }
                }
#else
                new[]
                {
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.813904",
                        AlbumPeak = "0.999664",
                        TrackGain = "-1.15",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.997620",
                        AlbumPeak = "0.999664",
                        TrackGain = "-6.28",
                        AlbumGain = "-5.70"
                    },
                    new TestAudioMetadata
                    {
                        TrackPeak = "0.999664",
                        AlbumPeak = "0.999664",
                        TrackGain = "-7.03",
                        AlbumGain = "-5.70"
                    }
                }
#endif
            }
        };

        [NotNull, ItemNotNull]
        public static IEnumerable<object[]> Data
        {
            [UsedImplicitly] get => _data;
        }
    }
}
