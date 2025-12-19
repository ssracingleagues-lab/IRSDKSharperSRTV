using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IRSDKSharper;
using IRSDKSharper.Overlay;

namespace IRSDKSharper.OverlayConsoleApp
{
        internal static class Program
        {
                private static readonly OverlayHost Host = new();
                private static readonly StandingsTowerWidget Standings = new( 20 );
                private static readonly DriverInfoWidget DriverInfo = new();
                private static readonly ProducerPanelController Producer = new( Host );

                private static readonly object RenderLock = new();
                private static DateTime lastRender = DateTime.MinValue;

                private const int RenderThrottleMs = 250;

                private static async Task Main()
                {
                        Host.RegisterWidget( Standings );
                        Host.RegisterWidget( DriverInfo );

                        Console.WriteLine( "iRacing overlay console" );
                        Console.WriteLine( "Press Ctrl+C to exit." );
                        Console.WriteLine();

                        using var cts = new CancellationTokenSource();
                        Console.CancelKeyPress += ( _, e ) =>
                        {
                                e.Cancel = true;
                                cts.Cancel();
                        };

                        var sdk = new IRacingSdk
                        {
                                UpdateInterval = 10 // ~6 updates per second keeps the console readable
                        };

                        HookSdkEvents( sdk, cts );

                        sdk.Start();

                        try
                        {
                                await Task.Delay( Timeout.Infinite, cts.Token );
                        }
                        catch ( TaskCanceledException )
                        {
                        }

                        Console.WriteLine( "Stopping iRacing SDK..." );
                        sdk.Stop();
                }

                private static void HookSdkEvents( IRacingSdk sdk, CancellationTokenSource cts )
                {
                        sdk.OnConnected += () => Console.WriteLine( "Connected to iRacing. Waiting for session/telemetry..." );
                        sdk.OnDisconnected += () => Console.WriteLine( "Disconnected from iRacing." );
                        sdk.OnException += ex => Console.WriteLine( $"SDK exception: {ex.Message}" );
                        sdk.OnStopped += () => cts.Cancel();

                        sdk.OnSessionInfo += () =>
                        {
                                var info = sdk.Data?.SessionInfo?.WeekendInfo;
                                Console.WriteLine( $"Session info: {info?.TrackDisplayName ?? "Unknown track"} - {info?.EventType ?? "Unknown event"}" );
                        };

                        sdk.OnTelemetryData += () => HandleTelemetry( sdk );
                }

                private static void HandleTelemetry( IRacingSdk sdk )
                {
                        if ( !Producer.Pump( sdk.Data ) )
                        {
                                return;
                        }

                        var now = DateTime.UtcNow;
                        if ( ( now - lastRender ).TotalMilliseconds < RenderThrottleMs )
                        {
                                return;
                        }

                        lastRender = now;

                        lock ( RenderLock )
                        {
                                Console.Clear();
                                Console.WriteLine( "iRacing Broadcast Overlay (console)" );
                                Console.WriteLine( $"Last update: {DateTime.Now:HH:mm:ss}" );
                                Console.WriteLine();

                                RenderDriverInfo( DriverInfo.Snapshot );
                                Console.WriteLine();
                                RenderStandings( Standings.Entries );
                        }
                }

                private static void RenderDriverInfo( DriverInfoSnapshot snapshot )
                {
                        Console.WriteLine( "=== Focus Driver ===" );
                        if ( string.IsNullOrWhiteSpace( snapshot.DriverName ) )
                        {
                                Console.WriteLine( "Waiting for driver info..." );
                                return;
                        }

                        Console.WriteLine( $"Car #{snapshot.CarNumber} – {snapshot.DriverName}" );
                        if ( !string.IsNullOrWhiteSpace( snapshot.TeamName ) )
                        {
                                Console.WriteLine( $"Team: {snapshot.TeamName}" );
                        }

                        Console.WriteLine( $"Class: {snapshot.CarClass} | Pos: {snapshot.Position} (Class {snapshot.ClassPosition})" );
                        Console.WriteLine( $"Lap dist: {snapshot.LapDistPct:P1} | Last: {FormatLap( snapshot.LastLapTime )} | Best: {FormatLap( snapshot.BestLapTime )}" );
                        Console.WriteLine( $"Pit Road: {( snapshot.IsOnPitRoad ? "YES" : "No" )}" );
                }

                private static void RenderStandings( IReadOnlyList<StandingsEntry> entries )
                {
                        Console.WriteLine( "=== Standings Tower ===" );
                        if ( entries.Count == 0 )
                        {
                                Console.WriteLine( "Waiting for leaderboard..." );
                                return;
                        }

                        Console.WriteLine( $"{"Pos",3} {"Car",4} {"Driver",-24} {"Last",8} {"Best",8} {"Gap",8} {"Pit",4}" );
                        foreach ( var entry in entries )
                        {
                                var pit = entry.IsOnPitRoad ? "PIT" : string.Empty;
                                Console.WriteLine( $"{entry.Position,3} {entry.CarNumber,4} {TrimName( entry.DriverName ),-24} {FormatLap( entry.LastLapTime ),8} {FormatLap( entry.BestLapTime ),8} {entry.GapToLeaderSeconds,6:0.00} {pit,4}" );
                        }
                }

                private static string FormatLap( float seconds ) => seconds <= 0 ? "--.--" : seconds.ToString( "0.000" );

                private static string TrimName( string name )
                {
                        const int max = 24;
                        if ( string.IsNullOrEmpty( name ) || name.Length <= max )
                        {
                                return name;
                        }

                        return name[..max];
                }
        }
}
