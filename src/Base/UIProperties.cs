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
            scrollViewBox = new Box(Vector2.Zero, bounds.Size);
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

        private Cursor _cursorStyle;
        /// <summary>
        /// The style of the cursor when hovering over this element.
        /// </summary>
        public Cursor CursorStyle
        {
            get => _cursorStyle;
            set
            {
                _cursorStyle = value;

                if (hover)
                {
                    handle.Window.CursorStyle = value;
                }
            }
        }

        internal bool hover = false;
        public bool Hover => hover;

        internal bool focus = false;
        public bool Focused => focus;

        internal bool selected = false;
        public bool MouseSelect => selected;

        /// <summary>
        /// Determines whether the mouse position updated through this element's events are affected by scale and pan.
        /// </summary>
        public bool ShiftInternalMouse { get; set; } = false;

        internal Vector2 mousePos;
        public Vector2 MouseLocation
        {
            get
            {
                // Mouse pos already calculated
                if (hover || parent == null) { return mousePos; }

                return ((parent.Properties.MouseLocation - parent.Properties.ViewPan) / parent.Properties.ViewScale) - bounds.Location;
            }
            set
            {
                if (parent == null)
                {
                    Window.MouseLocation = (value.X, -value.Y) + (Window.Size * 0.5);
                    return;
                }

                parent.Properties.MouseLocation = ((value + bounds.Location) * parent.Properties.ViewScale) + parent.Properties.ViewPan;
            }
        }

        internal int elementIndex = -1;
        public int ElementIndex => elementIndex;

        public bool Visable { get; set; } = true;

        public floatv Depth { get; set; } = -1;

        /// <summary>
        /// Determines whether this element can become the hover element or focus element.
        /// Child elements are not affected.
        /// </summary>
        public bool Interactable { get; set; } = true;

        private bool _selectable = true;
        public bool Selectable
        {
            get => _selectable && Interactable;
            set => _selectable = value;
        }

        public bool TabShifting { get; set; } = true;

        private Vector2 _viewPan = 0;
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

                Source.Graphics?.SetView();
            }
        }
        private floatv _viewScale = 1;
        /// <summary>
        /// The view scale of the element - applies to child elements.
        /// </summary>
        public floatv ViewScale
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

                Source.Graphics?.SetView();
            }
        }

        internal bool scrollX = false;
        internal bool scrollY = false;
        internal ScrollBarHover scrollBarHover;
        internal floatv initScrollPerc;
        internal Box scrollBounds = new Box();
        internal Vector2 scrollMoveRange;
        public ScrollInfo GetScrollInfo()
        {
            if (!Source.HasChildren || ScrollBar == null) { return new ScrollInfo(); }

            Box vb = viewBounds;
            Box scrollBox = ScrollBox;

            if (scrollBox.Left < vb.Left || scrollBox.Right > vb.Right)
            {
                vb.Bottom += ScrollBar.Width;
            }
            if (scrollBox.Bottom < vb.Bottom || scrollBox.Top > vb.Top)
            {
                vb.Right -= ScrollBar.Width;
            }

            scrollX = scrollBox.Left < vb.Left || scrollBox.Right > vb.Right || _viewPan.X != 0;
            scrollY = scrollBox.Bottom < vb.Bottom || scrollBox.Top > vb.Top || _viewPan.Y != 0;
            if (!(scrollX || scrollY))
            {
                return new ScrollInfo();
            }

            Box scrollView = scrollBox.Add(vb);
            scrollBounds = Box.FromBounds(
                Math.Min(scrollView.Left - vb.Left, 0),
                Math.Max(scrollView.Right - vb.Right, 0),
                Math.Max(scrollView.Top - vb.Top, 0),
                Math.Min(scrollView.Bottom - vb.Bottom, 0));

            scrollMoveRange = vb.Size - ((vb.Size * vb.Size) / scrollView.Size);

            return new ScrollInfo(scrollX, scrollY, vb.Size, scrollView.Size);
        }

        internal Vector2 GetScrollPercent()
        {
            return new Vector2(
                ViewPan.X.InvLerp(-scrollBounds.Right, -scrollBounds.Left),
                ViewPan.Y.InvLerp(-scrollBounds.Top, -scrollBounds.Bottom));
        }
        public void SetXScroll(floatv percent)
        {
            _viewPan.X = (-scrollBounds.Right).Lerp(-scrollBounds.Left, Math.Clamp(percent, 0, 1));
        }
        public void SetYScroll(floatv percent)
        {
            _viewPan.Y = (-scrollBounds.Top).Lerp(-scrollBounds.Bottom, Math.Clamp(percent, 0, 1));
        }

        internal Box viewBounds => new Box(Vector2.Zero, Source.GetRenderSize());
        internal Box scrollViewBox;
        public Box ScrollBox => scrollViewBox * _viewScale;//((Bounds)scrollViewBox) * _viewScale;
        public ScrollBar ScrollBar { get; set; } = null;
        public bool HoverOnScroll => scrollBarHover != ScrollBarHover.None;

        internal void PushViewBox(Box bounds, Box oldBounds)
        {
            if (UnshareBounds(oldBounds, scrollViewBox, bounds))
            {
                UIManager.RecalculateScrollBounds(this);
                return;
            }

            scrollViewBox = scrollViewBox + bounds;
        }
        private static bool UnshareBounds(Box old, Box bounds, Box newB)
        {
            return (old.Left == bounds.Left && newB.Left > old.Left) ||
                (old.Right == bounds.Right && newB.Right < old.Right) ||
                (old.Top == bounds.Top && newB.Top < old.Top) ||
                (old.Bottom == bounds.Bottom && newB.Bottom > old.Bottom);
        }

        internal void OnLayoutChange(object sender, EventArgs e) => Source.Properties.handle?.LayoutElement(Source);
    }
}
