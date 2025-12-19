namespace IRSDKSharper.Overlay
{
        public sealed class StandingsEntry
        {
                public int CarIdx { get; init; }
                public int Position { get; init; }
                public int ClassPosition { get; init; }
                public string DriverName { get; init; } = string.Empty;
                public string CarNumber { get; init; } = string.Empty;
                public float IntervalSeconds { get; init; }
                public float LastLapTime { get; init; }
                public float BestLapTime { get; init; }
                public float GapToLeaderSeconds { get; init; }
                public bool IsOnPitRoad { get; init; }
        }

        public sealed class DriverInfoSnapshot
        {
                public int CarIdx { get; init; }
                public string DriverName { get; init; } = string.Empty;
                public string TeamName { get; init; } = string.Empty;
                public string CarNumber { get; init; } = string.Empty;
                public int Position { get; init; }
                public int ClassPosition { get; init; }
                public float LapDistPct { get; init; }
                public float LastLapTime { get; init; }
                public float BestLapTime { get; init; }
                public string CarClass { get; init; } = string.Empty;
                public bool IsOnPitRoad { get; init; }
        }
}
