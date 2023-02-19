using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class UIProperties
    {
        public UIProperties(IElement source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public IElement Source { get; }

        internal UIManager handle;
        public UIManager Handle => handle;
        public Window Window => handle?.Window;
        public IElement RootElement => handle?.Root;
        public TextRenderer TextRenderer => handle?.TextRenderer;

        internal IElement parent;
        public IElement Parent => parent;

        internal Box bounds;
        public Box Bounds => bounds;

        /// <summary>
        /// The style of the cursor when hovering over this element.
        /// </summary>
        public Cursor CursorStyle { get; set; }

        internal bool hover = false;
        public bool Hover => hover;

        internal bool focus = false;
        public bool Focused => focus;

        internal bool selected = false;
        public bool MouseSelect => selected;

        internal Vector2 mousePos;
        public Vector2 MouseLocation
        {
            get
            {
                // Mouse pos already calculated
                if (hover || parent == null) { return mousePos; }

                return ((parent.Properties.MouseLocation - parent.Properties.ViewPan) / parent.Properties.ViewScale) - bounds.Centre;
            }
            set
            {
                if (parent == null)
                {
                    Window.MouseLocation = (value.X, -value.Y) + (Window.Size * 0.5);
                    return;
                }

                parent.Properties.MouseLocation = ((value + bounds.Centre) * parent.Properties.ViewScale) + parent.Properties.ViewPan;
            }
        }

        internal int elementIndex = -1;
        public int ElementIndex => elementIndex;

        public bool Visable { get; set; } = true;

        /// <summary>
        /// Determines whether this element can become the hover element or focus element.
        /// Child elements are not affected.
        /// </summary>
        public bool Interactable { get; set; } = true;

        public bool TabShifting { get; set; } = true;

        private Vector2 _viewPan = 0d;
        /// <summary>
        /// The view panning of the element - applies to child elements.
        /// </summary>
        public Vector2 ViewPan
        {
            get => _viewPan;
            set
            {
                _viewPan = value;
                // Trigger calculate hover
                handle.Window.GraphicsContext.Actions.Push(() =>
                {
                    handle.MouseMove(new MouseEventArgs(handle.Root.Properties.mousePos));
                });

                Source.Graphics.SetView();
            }
        }
        private double _viewScale = 1d;
        /// <summary>
        /// The view scale of the element - applies to child elements.
        /// </summary>
        public double ViewScale
        {
            get => _viewScale;
            set
            {
                _viewScale = value;
                // Trigger calculate hover
                handle.Window.GraphicsContext.Actions.Push(() =>
                {
                    handle.MouseMove(new MouseEventArgs(handle.Root.Properties.mousePos));
                });

                Source.Graphics.SetView();
            }
        }

        internal void OnLayoutChange(object sender, EventArgs e) => Source.Properties.handle?.LayoutElement(Source);
    }
}
