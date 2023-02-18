﻿using System;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public sealed class RootElement : UIManager, IElement
    {
        private class RootLayout : ILayout
        {
            public event EventHandler Change;

            public Box GetBounds(LayoutArgs args) => new Box(0d, args.Size);
        }

        public RootElement(Window window)
            : base(window)
        {
            Layout = new RootLayout();
            Properties = new UIProperties(this);
            Events = new UIEvents(this,
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { },
                (_) => { });

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

                _layoutManager = value;

                TriggerChange();
            }
        }

        public UIEvents Events { get; }
        public GraphicsManager Graphics => null;

        public Vector2 MouseLocation => Properties.mousePos;

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
    }
}
