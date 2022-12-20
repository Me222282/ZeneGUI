using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    /// <summary>
    /// The base class for all GUI elements.
    /// </summary>
    public abstract class Element
    {
        /// <summary>
        /// Craetes an element from its bounds.
        /// </summary>
        /// <param name="bounds">The bounding box of the element.</param>
        /// <param name="framebuffer">Whether to create a framebuffer.</param>
        public Element(IBox bounds, bool framebuffer = true)
        {
            _bounds = new RectangleI(bounds);

            // Dont create framebuffer
            if (!framebuffer) { return; }

            _framebuffer = new TextureRenderer(BaseFramebuffer.Properties.Width, BaseFramebuffer.Properties.Height);
            _framebuffer.SetColourAttachment(0, TextureFormat.Rgba);
            _framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
        }
        /// <summary>
        /// Craetes an element from a layout.
        /// </summary>
        /// <param name="layout">The layout of the element.</param>
        /// <param name="framebuffer">Whether to create a framebuffer.</param>
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
        /// <summary>
        /// The parent element.
        /// </summary>
        public Element Parent { get; private set; }

        internal virtual TextRenderer textRender => Parent.textRender;
        /// <summary>
        /// Text rendering object.
        /// </summary>
        public TextRenderer TextRenderer => textRender;

        protected ActionManager Actions
        {
            get
            {
                if (_window == null)
                {
                    return ActionManager.Temporary;
                }

                return _window.GraphicsContext.Actions;
            }
        }

        /// <summary>
        /// Determines whether this object renders to its own framebuffer.
        /// </summary>
        public bool HasFramebuffer
        {
            get => _framebuffer != null;
            protected set => _framebuffer = null;
        }
        private TextureRenderer _framebuffer = null;
        /// <summary>
        /// The framebuffer this object renders to.
        /// </summary>
        public TextureRenderer Framebuffer
        {
            get
            {
                if (HasFramebuffer) { return _framebuffer; }
                if (Parent == null) { return null; }
                return Parent.Framebuffer;
            }
        }
        /// <summary>
        /// The shader object used to render this object to its parent.
        /// </summary>
        /// <remarks>
        /// This only applies when <see cref="HasFramebuffer"/> is <see cref="true"/>.
        /// </remarks>
        public virtual IBasicShader Shader => Parent.Shader;

        private readonly object _boundsRef = new object();
        private RectangleI _bounds;
        private readonly object _mousePosRef = new object();
        private Vector2 _mousePos;

        private ILayout _layout = null;
        /// <summary>
        /// The position layout of the element. <see cref="null"/> if no layout is used.
        /// </summary>
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
        /// <summary>
        /// Determines whether this element uses a layout.
        /// </summary>
        public bool UsingLayout => _layout != null;

        /// <summary>
        /// The local mouse position.
        /// </summary>
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

                Parent.MouseLocation = value + _bounds.Centre;
            }
        }

        private void BoundsSet(RectangleI b)
        {
            OnSizeChange(new SizeChangeEventArgs(b.Size));
            OnElementMove(new PositionEventArgs(b.Location));
        }
        /// <summary>
        /// The bounds of the element.
        /// </summary>
        /// <remarks>
        /// Cannot be set if <see cref="UsingLayout"/> is <see cref="true"/>.
        /// </remarks>
        public RectangleI Bounds
        {
            get => _bounds;
            set
            {
                if (Layout != null) { return; }

                BoundsSet(value);
            }
        }
        /// <summary>
        /// The size of the element.
        /// </summary>
        /// <remarks>
        /// Cannot be set if <see cref="UsingLayout"/> is <see cref="true"/>.
        /// </remarks>
        public Vector2I Size
        {
            get => _bounds.Size;
            set
            {
                if (Layout != null) { return; }

                OnSizeChange(new SizeChangeEventArgs(value));
            }
        }
        /// <summary>
        /// The position of the element.
        /// </summary>
        /// <remarks>
        /// Cannot be set if <see cref="UsingLayout"/> is <see cref="true"/>.
        /// </remarks>
        public Vector2I Location
        {
            get => _bounds.Location;
            set
            {
                OnElementMove(new PositionEventArgs(value));
            }
        }
        private Vector2I _boundOffset;
        /// <summary>
        /// The offset added the the drawing bound's size.
        /// </summary>
        protected Vector2I DrawingBoundOffset
        {
            get => _boundOffset;
            set
            {
                if (_boundOffset == value) { return; }

                _boundOffset = value;

                SetProjection();
                OnSizeChange(new SizeChangeEventArgs(_bounds.Size));
            }
        }

        /// <summary>
        /// Determines whether the mouse is hovering over this element.
        /// </summary>
        public bool MouseHover => _mouseOver;
        /// <summary>
        /// Determines whether the mouse is selecting this element.
        /// </summary>
        public bool MouseSelect { get; private set; } = false;

        internal Cursor _currentCursor;
        /// <summary>
        /// The style of the cursor when hovering over this element.
        /// </summary>
        public Cursor CursorStyle { get; set; }

        /// <summary>
        /// Determines whether the element is visable and interacts with the mouse.
        /// </summary>
        public bool Visable { get; set; } = true;

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
        /// <summary>
        /// Adds a child element.
        /// </summary>
        /// <param name="e">The element to add.</param>
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
        /// <summary>
        /// Removes a child element.
        /// </summary>
        /// <param name="e">The element to remove.</param>
        /// <returns><see cref="true"/> if the element was found and removed; otherwise <see cref="false"/>.</returns>
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
                
                return Parent.RenderOffset + Parent._bounds.Centre;
            }
        }
        
        internal void Render(IFramebuffer framebuffer, Matrix4 projection)
        {
            if (!_render) { return; }
            if (HasFramebuffer && Shader == null) { return; }

            Framebuffer.Bind();

            Box drawingBounds = new Box(_bounds.Centre, _bounds.Size + DrawingBoundOffset);

            Framebuffer.ViewSize = (Vector2I)drawingBounds.Size;
            if (!HasFramebuffer)
            {
                Parent.Framebuffer.ViewLocation =
                    (
                        (Parent._bounds.Width / 2) + (int)drawingBounds.Left,
                        (Parent._bounds.Height / 2) + (int)drawingBounds.Bottom
                    ) + RenderOffset;
            }
            OnUpdate(new FrameEventArgs(Framebuffer));
            
            State.DepthTesting = false;

            if (HasFramebuffer && Parent != null)
            {
                Parent.Framebuffer.ViewLocation = 0;
                Parent.Framebuffer.ViewSize = Parent.Framebuffer.Size;
                framebuffer.Bind();
                Shader.Bind();
                Vector2I offset = RenderOffset;
                Shader.SetMatrices(
                    Matrix4.CreateBox(new Box(drawingBounds.Location + offset, drawingBounds.Size)),
                    Matrix4.Identity,
                    projection);
                Shader.ColourSource = ColourSource.Texture;
                Shader.TextureSlot = 0;
                _framebuffer.GetTexture(FrameAttachment.Colour0).Bind();
                Shapes.Square.Draw();
            }
            
            // Draw child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].Visable) { continue; }

                    State.DepthTesting = false;

                    textRender.Projection = span[i].Projection;
                    // Default colour
                    textRender.Colour = new Colour(255, 255, 255);

                    span[i].Render(framebuffer, projection);
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

            for (int i = 0; i < _hover.Count; i++)
            {
                if (!span[i].Visable)
                {
                    _hover.RemoveAt(i);
                    i--;
                    continue;
                }

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
                    if (!span[i].Visable) { continue; }

                    bool mouseOverNew = span[i]._bounds.Contains(e.Location);

                    // Mouse not in bounds
                    if ((mouseOverNew == span[i]._mouseOver) && (span[i]._mouseOver == false)) { continue; }
                    // mouseOverNew will also equal true
                    // Mouse moves inside bounds
                    if (mouseOverNew == span[i]._mouseOver)
                    {
                        span[i].OnMouseMove(new MouseEventArgs(e.Location - span[i]._bounds.Centre));
                        newCursor = span[i]._currentCursor;
                        continue;
                    }

                    // Mouse enters bounds
                    if (mouseOverNew)
                    {
                        span[i]._mouseOver = true;
                        span[i].OnMouseEnter(new EventArgs());
                        span[i].OnMouseMove(new MouseEventArgs(e.Location - span[i]._bounds.Centre));

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

            for (int i = 0; i < _hover.Count; i++)
            {
                if (!span[i].Visable)
                {
                    _hover.RemoveAt(i);
                    i--;
                    continue;
                }

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

            for (int i = 0; i < _hover.Count; i++)
            {
                if (!span[i].Visable)
                {
                    _hover.RemoveAt(i);
                    i--;
                    continue;
                }

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

        private void SetProjection()
        {
            Projection = Matrix4.CreateOrthographic(_bounds.Width + _boundOffset.X, _bounds.Height + _boundOffset.Y, 0d, 1d);
        }

        /// <summary>
        /// The projection matrix used to render objects to this element.
        /// </summary>
        public Matrix4 Projection { get; private set; } = Matrix4.Identity;
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
                Actions.Push(() =>
                {
                    //_framebuffer.ViewSize = e.Size;
                    _framebuffer.Size = e.Size;
                });
            }
            SetProjection();

            SizeChange?.Invoke(this, e);

            // Update child elements
            lock (_elLayoutRef)
            {
                Vector2 multiplier = e.Size * 0.5;
                Span<Element> span = CollectionsMarshal.AsSpan(_layouts);

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].Visable) { continue; }

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