namespace IRSDKSharper.Overlay
{
        /// <summary>
        /// Base class for overlay widgets that can be rendered on the broadcast layer.
        /// </summary>
        public abstract class OverlayWidget
        {
                protected OverlayWidget( string name )
                {
                        Name = name;
                }

                public string Name { get; }

                public bool IsVisible { get; private set; } = true;

                public void Show() => IsVisible = true;

                public void Hide() => IsVisible = false;

                public void ToggleVisibility() => IsVisible = !IsVisible;

                public abstract void Update( OverlayContext context );
        }
}
