using System.Collections.Generic;

using IRSDKSharper;

namespace IRSDKSharper.Overlay
{
        internal static class OverlayTelemetryReader
        {
                public static IReadOnlyList<int> ReadIntArray( IRacingSdkData data, string name, List<int> scratch )
                {
                        scratch.Clear();

                        if ( !data.TelemetryDataProperties.TryGetValue( name, out var datum ) )
                        {
                                return scratch;
                        }

                        for ( var i = 0; i < datum.Count; i++ )
                        {
                                scratch.Add( data.GetInt( datum, i ) );
                        }

                        return scratch;
                }

                public static IReadOnlyList<float> ReadFloatArray( IRacingSdkData data, string name, List<float> scratch )
                {
                        scratch.Clear();

                        if ( !data.TelemetryDataProperties.TryGetValue( name, out var datum ) )
                        {
                                return scratch;
                        }

                        for ( var i = 0; i < datum.Count; i++ )
                        {
                                scratch.Add( data.GetFloat( datum, i ) );
                        }

                        return scratch;
                }

                public static int GetInt( IReadOnlyList<int> source, int index )
                {
                        if ( index < 0 || source.Count == 0 || index >= source.Count )
                        {
                                return 0;
                        }

                        return source[ index ];
                }

                public static float GetFloat( IReadOnlyList<float> source, int index )
                {
                        if ( index < 0 || source.Count == 0 || index >= source.Count )
                        {
                                return 0f;
                        }

                        return source[ index ];
                }
        }
}
