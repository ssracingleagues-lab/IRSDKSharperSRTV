using System.Collections.Generic;
using System.Linq;

using IRSDKSharper;

namespace IRSDKSharper.Overlay
{
        /// <summary>
        /// Represents the data required to update overlay widgets for a single frame.
        /// </summary>
        public sealed class OverlayContext
        {
                public IRacingSdkData Data { get; }
                public IRacingSdkSessionInfo SessionInfo => Data.SessionInfo;
                public int FocusCarIndex { get; }
                public IReadOnlyCollection<int> TrackedCarIndices { get; }

                public OverlayContext( IRacingSdkData data, int focusCarIndex = -1, IReadOnlyCollection<int>? trackedCarIndices = null )
                {
                        Data = data;
                        FocusCarIndex = focusCarIndex;
                        TrackedCarIndices = trackedCarIndices ?? Enumerable.Empty<int>().ToArray();
                }

                public OverlayContext WithFocusCar( int carIdx ) => new OverlayContext( Data, carIdx, TrackedCarIndices );
        }
}
