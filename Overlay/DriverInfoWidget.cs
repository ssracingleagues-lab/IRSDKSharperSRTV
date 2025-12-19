using System.Collections.Generic;
using System.Linq;

using IRSDKSharper;
using static IRSDKSharper.Overlay.OverlayTelemetryReader;

namespace IRSDKSharper.Overlay
{
        /// <summary>
        /// Widget that exposes spotlight information for a selected driver.
        /// </summary>
        public sealed class DriverInfoWidget : OverlayWidget
        {
                private DriverInfoSnapshot snapshot = new();

                private readonly List<float> lapDist = new();
                private readonly List<int> position = new();
                private readonly List<int> classPosition = new();
                private readonly List<float> lastLap = new();
                private readonly List<float> bestLap = new();
                private readonly List<int> onPitRoad = new();

                public DriverInfoWidget() : base( "DriverInfo" )
                {
                }

                public DriverInfoSnapshot Snapshot => snapshot;

                public override void Update( OverlayContext context )
                {
                        var focusCarIdx = context.FocusCarIndex >= 0 ? context.FocusCarIndex : context.SessionInfo?.DriverInfo?.DriverCarIdx ?? -1;
                        if ( focusCarIdx < 0 )
                        {
                                snapshot = new DriverInfoSnapshot();
                                return;
                        }

                        var drivers = context.SessionInfo?.DriverInfo?.Drivers ?? Enumerable.Empty<IRacingSdkSessionInfo.DriverInfoModel.DriverModel>();
                        var driver = drivers.FirstOrDefault( d => d.CarIdx == focusCarIdx );
                        if ( driver == null )
                        {
                                snapshot = new DriverInfoSnapshot();
                                return;
                        }

                        var data = context.Data;
                        var lapPct = ReadFloatArray( data, "CarIdxLapDistPct", lapDist );
                        var positions = ReadIntArray( data, "CarIdxPosition", position );
                        var classPositions = ReadIntArray( data, "CarIdxClassPosition", classPosition );
                        var lastLapTimes = ReadFloatArray( data, "CarIdxLastLapTime", lastLap );
                        var bestLapTimes = ReadFloatArray( data, "CarIdxBestLapTime", bestLap );
                        var pitRoad = ReadIntArray( data, "CarIdxOnPitRoad", onPitRoad );

                        snapshot = new DriverInfoSnapshot
                        {
                                CarIdx = focusCarIdx,
                                DriverName = driver.UserName,
                                TeamName = driver.TeamName,
                                CarNumber = driver.CarNumber,
                                CarClass = driver.CarClassShortName,
                                Position = GetInt( positions, focusCarIdx ),
                                ClassPosition = GetInt( classPositions, focusCarIdx ),
                                LapDistPct = GetFloat( lapPct, focusCarIdx ),
                                LastLapTime = GetFloat( lastLapTimes, focusCarIdx ),
                                BestLapTime = GetFloat( bestLapTimes, focusCarIdx ),
                                IsOnPitRoad = GetInt( pitRoad, focusCarIdx ) == 1
                        };
                }
        }
}
