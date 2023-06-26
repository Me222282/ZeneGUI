using System;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public sealed class RootElement : UIManager, IElement
    {
        private class RootLayout : ILayout
        {
            public event EventHandler Change
            {
                add => throw new NotSupportedException();
                remove => throw new NotSupportedException();
            }

            public Box GetBounds(LayoutArgs args) => new Box(0d, args.Size);
        }

        public RootElement(Window window)
            : base(window)
        {
            Layout = new RootLayout();
            Properties = new UIProperties(this)
            {
                ScrollBar = new ScrollBar()
            };
            Events = new EventListener(this);

            // Add events listeners
            window.MouseMove += (_, e) => OnMouseMove(e);
            window.MouseDown += (_, e) => MouseDown(new MouseEventArgs(MouseLocation, e.Button, e.Modifier));
            window.MouseUp += (_, e) => MouseUp(new MouseEventArgs(MouseLocation, e.Button, e.Modifier));
            window.Scroll += (_, e) => Scroll(e);
            window.KeyDown += (_, e) => KeyDown(e);
            window.KeyUp += (_, e) => Focus.OnKeyUp(e);
            window.TextInput += (_, e) => Focus.OnTextInput(e);
            window.SizePixelChange += (_, e) => SetFrameSize(e.Value);
            window.Focus += (_, e) => WindowFocus(e);

            window.Start += (_, _) => SetFrameSize(Window.Size);

            Root_(this);
        }

        public UIProperties Properties { get; }
        ElementList IElement.Children => Elements;

        public ILayout Layout { get; }
        private ILayoutManager _layoutManager = GUI.LayoutManager.Empty;
        public ILayoutManager LayoutManager
        {
            get => _layoutManager;
            set
            {
                if (value == null)
                {
                    value = GUI.LayoutManager.Empty;
                }

                if (_layoutManager != null)
                {
                    _layoutManager.Change -= Properties.OnLayoutChange;
                }

                _layoutManager = value;

                _layoutManager.Change += Properties.OnLayoutChange;

                TriggerChange();
            }
        }

        public EventListener Events { get; }
        public GraphicsManager Graphics => null;

        public Vector2 MouseLocation => Properties.mousePos;

        public string Id => "@root";
        public bool OverrideScroll => false;

        public bool IsMouseHover(Vector2 mousePos) => true;

        private void WindowFocus(FocusedEventArgs e) => Focus.OnFocus(e.Focus);

        private void OnMouseMove(MouseEventArgs e)
        {
            Vector2 ml = e.Location - (Window.Size * 0.5);
            MouseMove(new MouseEventArgs(ml));
        }
        private void MouseDown(MouseEventArgs e)
        {
            Focus = Hover;
            Hover.OnMouseDown(new MouseEventArgs(Hover.Properties.mousePos, e.Button, e.Modifier));

            UIProperties prop = Hover.Properties;
            if (prop.scrollBarHover == ScrollBarHover.None) { return; }
            
            if (prop.scrollBarHover == ScrollBarHover.XAxis)
            {
                prop.initScrollPerc = prop.ScrollBar.GetScrollPercentage(prop.scrollMoveRange.X, e.Location, false);
                return;
            }
            if (prop.scrollBarHover == ScrollBarHover.YAxis)
            {
                prop.initScrollPerc = prop.ScrollBar.GetScrollPercentage(prop.scrollMoveRange.Y, e.Location, true);
            }

            ManageMouseScroll(Hover, e.Location);
        }
        private void MouseUp(MouseEventArgs e)
        {
            Hover.OnMouseUp(new MouseEventArgs(Hover.Properties.mousePos, e.Button, e.Modifier));

            MouseMove(e);
        }
        private void KeyDown(KeyEventArgs e)
        {
            // Shift focus from tab
            if (Focus.Properties.TabShifting && e[Keys.Tab])
            {
                if (e[Mods.Shift])
                {
                    ShiftFocusLeft();
                    return;
                }

                ShiftFocusRight();
                return;
            }

            Focus.OnKeyDown(e);

            if (e[Keys.Enter])
            {
                Focus.OnMouseDown(new MouseEventArgs(MouseButton.Left, e.Modifier));
                Focus.OnMouseUp(new MouseEventArgs(MouseButton.Left, e.Modifier));
                return;
            }
        }
        private void Scroll(ScrollEventArgs e)
        {
            Hover.OnScroll(e);
            if (Hover.OverrideScroll) { return; }

            bool success = ManageScrollCon(uninteractHover, e.DeltaY, true);
            if (success) { return; }

            ManageScrollCon(Hover, e.DeltaY, false);
        }
        private bool ManageScrollCon(IElement e, double delta, bool uninteract)
        {
            if (e == null) { return false; }
            if (!(e.Properties.scrollX || e.Properties.scrollY) || e.Properties.ScrollBar == null)
            {
                return ManageScrollCon(e.Properties.parent, delta, uninteract);
            }

            if (uninteract)
            {
                return ManageScroll(e, delta);
            }

            if (!ManageScroll(e, delta))
            {
                return ManageScrollCon(e.Properties.parent, delta, false);
            }

            return true;
        }
        private bool ManageScroll(IElement e, double delta)
        {
            double offset = e.Properties.ScrollBar.ScrollSpeed * delta;

            bool shift = Window[Mods.Shift];
            if (e.Properties.scrollX && shift)
            {
                double panX = e.Properties.ViewPan.X + offset;
                panX = Math.Clamp(panX,
                    -e.Properties.scrollBounds.Right,
                    -e.Properties.scrollBounds.Left);
                // No change in pan
                if (panX == e.Properties.ViewPan.X) { return false; }

                e.Properties.ViewPan = new Vector2(panX, e.Properties.ViewPan.Y);
                return true;
            }

            if (!e.Properties.scrollY || shift || Window[Mods.Alt] || Window[Mods.Control]) { return false; }

            double panY = e.Properties.ViewPan.Y - offset;
            panY = Math.Clamp(panY,
                -e.Properties.scrollBounds.Top,
                -e.Properties.scrollBounds.Bottom);
            // No change in pan
            if (panY == e.Properties.ViewPan.Y) { return false; }

            e.Properties.ViewPan = new Vector2(e.Properties.ViewPan.X, panY);

            return true;
        }

        public void AddChild(IElement element) => Elements.Add(element);
        public void RemoveChild(IElement element) => Elements.Remove(element);

        public void ClearChildren() => Elements.Clear();

        public void SwapChildren(int indexA, int indexB) => Elements.Swap(indexA, indexB);
        public void SwapChildren(IElement a, IElement b) => Elements.Swap(a, b);

        public T Find<T>() where T : class, IElement
            => Elements.RecursiveFind<T>();
        public T Find<T>(string id) where T : class, IElement
            => Elements.RecursiveFind<T>(id);
        public IElement Find(string id) => Elements.RecursiveFind(id);
    }
}
