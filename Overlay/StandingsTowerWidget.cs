using System;
using System.Collections.Generic;
using System.Linq;

using IRSDKSharper;
using static IRSDKSharper.Overlay.OverlayTelemetryReader;

namespace IRSDKSharper.Overlay
{
        /// <summary>
        /// Real-time leaderboard widget designed for the broadcast overlay layer.
        /// </summary>
        public sealed class StandingsTowerWidget : OverlayWidget
        {
                private readonly List<StandingsEntry> entries = new();
                private readonly List<int> scratchPosition = new();
                private readonly List<int> scratchClassPosition = new();
                private readonly List<float> scratchLastLap = new();
                private readonly List<float> scratchBestLap = new();
                private readonly List<float> scratchLapDist = new();
                private readonly List<int> scratchOnPitRoad = new();

                public StandingsTowerWidget( int maxEntries = 20 ) : base( "StandingsTower" )
                {
                        MaxEntries = maxEntries;
                }

                public int MaxEntries { get; }

                public IReadOnlyList<StandingsEntry> Entries => entries;

                public override void Update( OverlayContext context )
                {
                        entries.Clear();

                        var data = context.Data;
                        var positions = ReadIntArray( data, "CarIdxPosition", scratchPosition );
                        var classPositions = ReadIntArray( data, "CarIdxClassPosition", scratchClassPosition );
                        var lastLapTimes = ReadFloatArray( data, "CarIdxLastLapTime", scratchLastLap );
                        var bestLapTimes = ReadFloatArray( data, "CarIdxBestLapTime", scratchBestLap );
                        var lapDistPct = ReadFloatArray( data, "CarIdxLapDistPct", scratchLapDist );
                        var onPitRoad = ReadIntArray( data, "CarIdxOnPitRoad", scratchOnPitRoad );

                        var leaderLapPct = lapDistPct.Any() ? lapDistPct.First() : 0f;
                        var tracked = context.TrackedCarIndices;
                        var trackedLookup = tracked.Count > 0 ? new HashSet<int>( tracked ) : null;

                        foreach ( var driver in context.SessionInfo?.DriverInfo?.Drivers ?? Enumerable.Empty<IRacingSdkSessionInfo.DriverInfoModel.DriverModel>() )
                        {
                                if ( driver.CarIdx < 0 || driver.CarIdx >= positions.Count )
                                {
                                        continue;
                                }

                                if ( trackedLookup != null && !trackedLookup.Contains( driver.CarIdx ) )
                                {
                                        continue;
                                }

                                var position = positions[ driver.CarIdx ];
                                if ( position <= 0 )
                                {
                                        continue;
                                }

                                var entry = new StandingsEntry
                                {
                                        CarIdx = driver.CarIdx,
                                        Position = position,
                                        ClassPosition = GetInt( classPositions, driver.CarIdx ),
                                        DriverName = driver.UserName,
                                        CarNumber = driver.CarNumber,
                                        LastLapTime = GetFloat( lastLapTimes, driver.CarIdx ),
                                        BestLapTime = GetFloat( bestLapTimes, driver.CarIdx ),
                                        IntervalSeconds = CalculateInterval( leaderLapPct, lapDistPct, driver.CarIdx ),
                                        GapToLeaderSeconds = CalculateGap( leaderLapPct, lapDistPct, driver.CarIdx ),
                                        IsOnPitRoad = GetInt( onPitRoad, driver.CarIdx ) == 1
                                };

                                entries.Add( entry );
                        }

                        entries.Sort( ( a, b ) => a.Position.CompareTo( b.Position ) );

                        if ( entries.Count > MaxEntries )
                        {
                                entries.RemoveRange( MaxEntries, entries.Count - MaxEntries );
                        }
                }

                private static float CalculateInterval( float leaderLapPct, IReadOnlyList<float> lapPct, int carIdx )
                {
                        if ( lapPct.Count == 0 || carIdx >= lapPct.Count )
                        {
                                return 0f;
                        }

                        return lapPct[ carIdx ] - leaderLapPct;
                }

                private static float CalculateGap( float leaderLapPct, IReadOnlyList<float> lapPct, int carIdx )
                {
                        if ( lapPct.Count == 0 || carIdx >= lapPct.Count )
                        {
                                return 0f;
                        }

                        var deltaPct = leaderLapPct - lapPct[ carIdx ];
                        if ( Math.Abs( deltaPct ) < 0.0001f )
                        {
                                return 0f;
                        }

                        return deltaPct;
                }
        }
}
