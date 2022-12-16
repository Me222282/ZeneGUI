using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class Element
    {
        public Element(IBox bounds, bool framebuffer = true)
        {
            _bounds = new RectangleI(bounds);

            // Dont create framebuffer
            if (!framebuffer) { return; }

            _framebuffer = new TextureRenderer(BaseFramebuffer.Properties.Width, BaseFramebuffer.Properties.Height);
            _framebuffer.SetColourAttachment(0, TextureFormat.Rgba);
            _framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
        }
        public Element(ILayout layout, bool framebuffer = true)
        {
            Layout = layout;

            // Dont create framebuffer
            if (!framebuffer) { return; }

            _framebuffer = new TextureRenderer(BaseFramebuffer.Properties.Width, BaseFramebuffer.Properties.Height);
            _framebuffer.SetColourAttachment(0, TextureFormat.Rgba);
            _framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
        }

        internal Window _window;
        public Element Parent { get; private set; }

        internal virtual TextRenderer textRender => Parent.textRender;
        public TextRenderer TextRenderer => textRender;

        public bool HasFramebuffer
        {
            get => _framebuffer != null;
            protected set => _framebuffer = null;
        }
        private TextureRenderer _framebuffer = null;
        public TextureRenderer Framebuffer
        {
            get
            {
                if (HasFramebuffer) { return _framebuffer; }
                if (Parent == null) { return null; }
                return Parent.Framebuffer;
            }
        }
        public virtual IBasicShader Shader => Parent.Shader;

        private readonly object _boundsRef = new object();
        private RectangleI _bounds;
        private readonly object _mousePosRef = new object();
        private Vector2 _mousePos;

        private ILayout _layout = null;
        public ILayout Layout
        {
            get => _layout;
            set
            {
                ILayout old = _layout;
                _layout = value;

                if (Parent == null) { return; }

                if (value == null && old != null)
                {
                    Parent.RemoveLayout(this);
                    return;
                }

                if (old == null && value != null)
                {
                    Parent.AddLayout(this);
                    return;
                }

                // && old != null
                if (value != null)
                {
                    BoundsSet(value.GetBounds(this, Parent.Size));
                    return;
                }
            }
        }
        public bool UsingLayout => _layout != null;

        public Vector2 MouseLocation
        {
            get => _mousePos;
            set
            {
                if (Parent == null)
                {
                    _window.MouseLocation = (value.X, -value.Y) + (_window.Size * 0.5);
                    return;
                }

                Parent.MouseLocation = value + _bounds.Center;
            }
        }

        private void BoundsSet(RectangleI b)
        {
            OnSizeChange(new SizeChangeEventArgs(b.Size));
            OnElementMove(new PositionEventArgs(b.Location));
        }
        public RectangleI Bounds
        {
            get => _bounds;
            set
            {
                if (Layout != null) { return; }

                BoundsSet(value);
            }
        }
        public Vector2I Size
        {
            get => _bounds.Size;
            set
            {
                if (Layout != null) { return; }

                OnSizeChange(new SizeChangeEventArgs(value));
            }
        }
        public Vector2I Location
        {
            get => _bounds.Location;
            set
            {
                OnElementMove(new PositionEventArgs(value));
            }
        }

        public bool MouseHover => _mouseOver;
        public bool MouseSelect { get; private set; } = false;

        internal Cursor _currentCursor;
        public Cursor CursorStyle { get; set; }

        public bool UserResizable { get; protected set; }

        public event TextInputEventHandler TextInput;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event ScrolEventHandler Scroll;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event SizeChangeEventHandler SizeChange;
        public event PositionEventHandler ElementMove;

        public event FrameEventHandler Update;

        private readonly object _elementRef = new object();
        private readonly List<Element> _elements = new List<Element>();

        /// <summary>
        /// Determines whether <paramref name="key"/> is currently being pressed.
        /// </summary>
        /// <param name="key">THe key to query</param>
        /// <returns></returns>
        public bool this[Keys key] => _window[key];
        /// <summary>
        /// Determines whether <paramref name="mod"/> is currently active.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="NotSupportedException"/> if <paramref name="mod"/> is <see cref="Mods.CapsLock"/> or <see cref="Mods.NumLock"/>.
        /// </remarks>
        /// <param name="mod">The modifier to query.</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public bool this[Mods mod] => _window[mod];
        /// <summary>
        /// Determines whether <paramref name="button"/> is currently pressed.
        /// </summary>
        /// <param name="button">The mouse button to query.</param>
        /// <returns></returns>
        public bool this[MouseButton button] => _window[button];

        private void SetWindow(Window w)
        {
            _window = w;

            // Set child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].SetWindow(_window);
                }
            }
        }
        public void AddChild(Element e)
        {
            if (e.Parent != null)
            {
                throw new ArgumentException("Element already managed.", nameof(e));
            }

            e.Parent = this;
            if (_window != null) { e.SetWindow(_window); }

            lock (_elementRef)
            {
                _elements.Add(e);
            }

            // Has layout
            if (e.Layout != null) { AddLayout(e); }

            if (_window != null && _window.Running) { e.OnStart(); }
        }
        public bool RemoveChild(Element e)
        {
            // This element is not e.Parent - cannot be removed
            if (e.Parent != this) { return false; }

            _elements.Remove(e);
            _layouts.Remove(e);
            _hover.Remove(e);

            e.Parent = null;
            e.SetWindow(null);

            return true;
        }
        
        private Vector2I RenderOffset
        {
            get
            {
                if (Parent == null)
                {
                    return Vector2I.Zero;
                }
                
                return Parent.RenderOffset + Parent._bounds.Center;
            }
        }
        
        internal void Render(IFramebuffer framebuffer, Matrix4 projection, IDrawable draw)
        {
            if (!_render) { return; }
            if (HasFramebuffer && Shader == null) { return; }

            Framebuffer.Bind();
            //Vector2 vs = Framebuffer.ViewSize;

            Framebuffer.ViewSize = _bounds.Size;
            if (!HasFramebuffer)
            {
                Parent.Framebuffer.ViewLocation =
                    (
                        (_bounds.Height / 2) + _bounds.Bottom,
                        (_bounds.Width / 2) + _bounds.Left
                    ) + RenderOffset;
            }
            OnUpdate(new FrameEventArgs(Framebuffer));
            
            State.DepthTesting = false;

            if (HasFramebuffer)
            {
                framebuffer.Bind();
                Shader.Bind();
                Vector2I offset = RenderOffset;
                Shader.SetMatrices(
                    Matrix4.CreateBox(new Rectangle(_bounds.Left + offset.X, _bounds.Top + offset.Y, _bounds.Width, _bounds.Height)),
                    Matrix4.Identity,
                    projection);
                Shader.ColourSource = ColourSource.Texture;
                Shader.TextureSlot = 0;
                _framebuffer.GetTexture(FrameAttachment.Colour0).Bind();
                draw.Draw();
            }
            
            // Draw child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    State.DepthTesting = false;

                    textRender.Projection = span[i].Projection;
                    // Default colour
                    textRender.Colour = new Colour(255, 255, 255);
                    
                    span[i].Render(framebuffer, projection, draw);
                }
            }
        }

        internal void OnStart()
        {
            RectangleI rect = _bounds;
            _bounds = new RectangleI();

            OnSizeChange(new SizeChangeEventArgs(rect.Size));
            OnElementMove(new PositionEventArgs(rect.Location));
            
            // Trigger first OnSizeChange for child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnStart();
                }
            }
        }

        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            TextInput?.Invoke(this, e);

            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnTextInput(e);
                }
            }
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);

            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnKeyDown(e);
                }
            }
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);

            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnKeyUp(e);
                }
            }
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);

            // Update child elements
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);

            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnScroll(e);
            }
        }

        private readonly List<Element> _hover = new List<Element>();
        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            _mouseOver = false;
            
            MouseLeave?.Invoke(this, e);
            
            // Set _mouseOver to false
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);

            for (int i = 0; i < _hover.Count; i++)
            {
                if (span[i]._mouseOver)
                {
                    span[i].OnMouseLeave(e);
                    
                    // Remove element
                    if (MouseSelect) { continue; }
                    _hover.RemoveAt(i);
                    i--;
                }
            }
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            lock (_mousePosRef)
            {
                if (_mousePos == e.Location) { return; }

                _mousePos = e.Location;
            }
            MouseMove?.Invoke(this, e);

            lock (_elementRef)
            {
                Cursor newCursor = CursorStyle;

                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    bool mouseOverNew = span[i]._bounds.Contains(e.Location);

                    // Mouse not in bounds
                    if ((mouseOverNew == span[i]._mouseOver) && (span[i]._mouseOver == false)) { continue; }
                    // mouseOverNew will also equal true
                    // Mouse moves inside bounds
                    if (mouseOverNew == span[i]._mouseOver)
                    {
                        span[i].OnMouseMove(new MouseEventArgs(e.Location - span[i]._bounds.Center));
                        newCursor = span[i]._currentCursor;
                        continue;
                    }

                    // Mouse enters bounds
                    if (mouseOverNew)
                    {
                        span[i]._mouseOver = true;
                        span[i].OnMouseEnter(new EventArgs());
                        span[i].OnMouseMove(new MouseEventArgs(e.Location - span[i]._bounds.Center));

                        // Add to hover
                        // Will be false if element is already in hover
                        if (!MouseSelect)
                        {
                            _hover.Add(span[i]);
                        }
                        newCursor = span[i]._currentCursor;
                        continue;
                    }

                    // Mouse leaves bounds
                    span[i].OnMouseLeave(new EventArgs());
                    // Remove to hover
                    // Will be false if element should stay in hover until OnMouseUp
                    if (!MouseSelect)
                    {
                        _hover.Remove(span[i]);
                    }
                }

                _currentCursor = newCursor;
            }
        }
        private bool _mouseOver = false;
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseSelect = true;

            MouseDown?.Invoke(this, e);

            // Update child elements
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);

            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnMouseDown(e);
            }
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (_window.MouseButton == MouseButton.None)
            {
                MouseSelect = false;
            }

            MouseUp?.Invoke(this, e);

            // Update child elements
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);

            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnMouseUp(e);

                // Mouse not hovering over and no mouse buttons pressed
                if (!span[i]._mouseOver && !MouseSelect)
                {
                    _hover.RemoveAt(i);
                    i--;
                }
            }
        }

        private void AddLayout(Element e)
        {
            lock (_elLayoutRef)
            {
                _layouts.Add(e);
            }

            e.BoundsSet(e.Layout.GetBounds(e, _bounds.Size));
        }
        private void RemoveLayout(Element e)
        {
            lock (_elLayoutRef)
            {
                _layouts.Remove(e);
            }
        }

        private readonly object _elLayoutRef = new object();
        private readonly List<Element> _layouts = new List<Element>();

        public Matrix4 Projection { get; private set; }
        private bool _render = true;
        protected virtual void OnSizeChange(SizeChangeEventArgs e)
        {
            if (e.Width <= 0 || e.Height <= 0)
            {
                _render = false;
                return;
            }
            else
            {
                _render = true;
            }

            lock (_boundsRef)
            {
                if (_bounds.Size == e.Size) { return; }

                _bounds.Size = e.Size;
            }
            if (HasFramebuffer)
            {
                //_framebuffer.ViewSize = e.Size;
                _framebuffer.Size = e.Size;
            }
            Projection = Matrix4.CreateOrthographic(e.Width, e.Height, 0d, 1d);

            SizeChange?.Invoke(this, e);

            // Update child elements
            lock (_elLayoutRef)
            {
                Vector2 multiplier = e.Size * 0.5;
                Span<Element> span = CollectionsMarshal.AsSpan(_layouts);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].BoundsSet(span[i].Layout.GetBounds(span[i], e.Size));
                    //span[i].BoundsSet();
                }
            }
        }
        protected virtual void OnElementMove(PositionEventArgs e)
        {
            lock (_boundsRef)
            {
                if (_bounds.Location == e.Location) { return; }

                _bounds.Location = e.Location;
            }
            ElementMove?.Invoke(this, e);
        }

        protected virtual void OnUpdate(FrameEventArgs e)
        {
            Update?.Invoke(this, e);
        }
    }
}