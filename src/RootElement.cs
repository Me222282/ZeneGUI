using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    /// <summary>
    /// The root parent element.
    /// </summary>
    public class RootElement : Element
    {
        public RootElement(Window w)
            : this(w, true)
        {
            
        }
        internal RootElement(Window w, bool events = true)
            : base(new TextRenderer())
        {
            _window = w;
            RootElement = this;

            _mouseOver = true;

            if (events)
            {
                // Add events listeners
                _window.MouseMove += MouseMove;
                _window.MouseDown += (_, e) => OnMouseDown(new MouseEventArgs(MouseLocation, e.Button, e.Modifier));
                _window.MouseUp += (_, e) => OnMouseUp(new MouseEventArgs(MouseLocation, e.Button, e.Modifier));
                _window.Scroll += (_, e) => OnScroll(e);
                _window.KeyDown += (_, e) => OnKeyDown(e);
                _window.KeyUp += (_, e) => _focus?.OnKeyUp(e);
                _window.TextInput += (_, e) => _focus?.OnTextInput(e);
                _window.SizePixelChange += (_, e) => SizeChangeListener((VectorEventArgs)e);

                _window.Start += (_, _) => SizeChangeListener(new VectorEventArgs(_window.Size));
            }

            _focus = this;

            _framebuffer = new TextureRenderer(w.Width, w.Height);
            _framebuffer.SetColourAttachment(0, TextureFormat.Rgba8);
            _framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
            _framebuffer.Scissor = new Scissor(new RectangleI(Vector2I.Zero, _framebuffer.Size));
        }

        private Element _focus;
        public Element FocusElement => _focus;

        internal readonly TextureRenderer _framebuffer;

        internal new void MouseMove(object s, MouseEventArgs e)
        {
            Vector2 ml = e.Location - (_window.Size * 0.5);
            ml.Y = -ml.Y;

            MouseMoveListener(new MouseEventArgs(ml));
        }

        /// <summary>
        /// Renders all visable elements to the current context.
        /// </summary>
        public void Render()
        {
            IFramebuffer current = State.GetBoundFramebuffer(FrameTarget.Draw);

            _framebuffer.Clear(BufferBit.Colour | BufferBit.Depth);
            _framebuffer.Scissor.Enabled = true;

            Render(Matrix4.Identity, Projection);

            _framebuffer.Scissor.Enabled = false;

            // Copy data to main framebuffer
            _framebuffer.CopyFrameBuffer(current, BufferBit.Colour, TextureSampling.Nearest);
        }

        protected override void OnSizeChange(VectorEventArgs e)
        {
            base.OnSizeChange(e);

            Actions.Push(() =>
            {
                _framebuffer.Size = (Vector2I)e.Value;
            });
        }

        private void SetFocus(Element e)
        {
            if (_focus != null)
            {
                _focus.Focused = false;
                _focus.OnFocus(_focusFalse);
            }

            _focus = e;

            if (_focus == null) { return; }
            _focus.Focused = true;
            _focus.OnFocus(_focusTrue);
        }

        private readonly FocusedEventArgs _focusTrue = new FocusedEventArgs(true);
        private readonly FocusedEventArgs _focusFalse = new FocusedEventArgs(false);
        protected internal override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            SetFocus(Hover);
        }

        internal new void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // No focus, no event
            if (_focus == null) { return; }

            // Shift focus from tab
            if (_focus.TabShifting && e[Keys.Tab])
            {
                if (e[Mods.Shift])
                {
                    FocusShiftLeft();
                    return;
                }

                FocusShiftRight();
                return;
            }

            _focus?.OnKeyDown(e);

            // Simulate click event
            if (e[Keys.Enter])
            {
                _focus.OnMouseDown(new MouseEventArgs(MouseButton.Left, e.Modifier));
                _focus.OnMouseUp(new MouseEventArgs(MouseButton.Left, e.Modifier));
                return;
            }
        }

        private void FocusShiftRight()
        {
            if (_focus == null) { return; }

            Element next = _focus.LowestFirstElement();
            if (_focus != next)
            {
                SetFocus(next);
                return;
            }

            // Only one element in GUI tree
            if (_focus.Parent == null) { return; }

            next = _focus.NextElement();
            SetFocus(next);
        }
        private void FocusShiftLeft()
        {
            if (_focus == null) { return; }

            Element previous;

            // Only one element in GUI tree
            if (_focus.Parent == null)
            {
                previous = _focus.LowestLastElement();
            }
            else
            {
                previous = _focus.PreviousElement();
            }
            
            SetFocus(previous);
        }
    }
}