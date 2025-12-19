using System;
using System.Collections.Generic;
using System.Linq;

namespace IRSDKSharper.Overlay
{
        /// <summary>
        /// Coordinates overlay widgets and updates them from telemetry data.
        /// </summary>
        public sealed class OverlayHost
        {
                private readonly List<OverlayWidget> widgets = new();

                public IReadOnlyCollection<OverlayWidget> Widgets => widgets.AsReadOnly();

                public void RegisterWidget( OverlayWidget widget )
                {
                        if ( widget == null )
                        {
                                throw new ArgumentNullException( nameof( widget ) );
                        }

                        if ( widgets.Any( w => w.Name == widget.Name ) )
                        {
                                throw new InvalidOperationException( $"A widget named {widget.Name} is already registered." );
                        }

                        widgets.Add( widget );
                }

                public T GetWidget<T>() where T : OverlayWidget => widgets.OfType<T>().FirstOrDefault();

                public bool TryUpdate( OverlayContext context )
                {
                        if ( context?.Data == null || !context.Data.TelemetryDataPropertiesReady )
                        {
                                return false;
                        }

                        foreach ( var widget in widgets.Where( w => w.IsVisible ) )
                        {
                                widget.Update( context );
                        }

                        return true;
                }
        }
}
