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
            Properties = new UIProperties(this);
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

        public bool IsMouseHover(Vector2 mousePos) => true;

        private void OnMouseMove(MouseEventArgs e)
        {
            Vector2 ml = e.Location - (Window.Size * 0.5);
            MouseMove(new MouseEventArgs(ml));
        }
        private void MouseDown(MouseEventArgs e)
        {
            Focus = Hover;
            Hover.OnMouseDown(e);
        }
        private void MouseUp(MouseEventArgs e)
        {
            Hover.OnMouseUp(e);

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
        private void Scroll(ScrollEventArgs e) => Hover.OnScroll(e);

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
