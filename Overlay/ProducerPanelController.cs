using System;
using System.Collections.Generic;

using IRSDKSharper;

namespace IRSDKSharper.Overlay
{
        /// <summary>
        /// Lightweight controller for wiring a producer control panel to overlay widgets.
        /// </summary>
        public sealed class ProducerPanelController
        {
                private readonly OverlayHost host;
                private IReadOnlyCollection<int> trackedCars = Array.Empty<int>();

                public ProducerPanelController( OverlayHost host )
                {
                        this.host = host ?? throw new ArgumentNullException( nameof( host ) );
                }

                public int FocusCarIndex { get; private set; } = -1;

                public IReadOnlyCollection<int> TrackedCarIndices => trackedCars;

                public void SetFocusCar( int carIdx )
                {
                        FocusCarIndex = carIdx;
                }

                public void SetTrackedCars( IReadOnlyCollection<int> carIndices )
                {
                        trackedCars = carIndices ?? Array.Empty<int>();
                }

                public void ShowWidget( string name ) => SetWidgetVisibility( name, true );

                public void HideWidget( string name ) => SetWidgetVisibility( name, false );

                public void ToggleWidget( string name )
                {
                        var widget = FindWidget( name );
                        if ( widget == null )
                        {
                                return;
                        }

                        widget.ToggleVisibility();
                }

                public bool Pump( IRacingSdkData data )
                {
                        var context = new OverlayContext( data, FocusCarIndex, trackedCars );
                        return host.TryUpdate( context );
                }

                private OverlayWidget FindWidget( string name )
                {
                        foreach ( var widget in host.Widgets )
                        {
                                if ( string.Equals( widget.Name, name, StringComparison.OrdinalIgnoreCase ) )
                                {
                                        return widget;
                                }
                        }

                        return null;
                }

                private void SetWidgetVisibility( string name, bool visible )
                {
                        var widget = FindWidget( name );
                        if ( widget == null )
                        {
                                return;
                        }

                        if ( visible )
                        {
                                widget.Show();
                        }
                        else
                        {
                                widget.Hide();
                        }
                }
        }
}
