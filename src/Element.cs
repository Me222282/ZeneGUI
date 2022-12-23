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
            _bounds = new Box(bounds);

            // Dont create framebuffer
            if (!framebuffer) { return; }

            SetFramebuffer();
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

            SetFramebuffer();
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
            protected set
            {
                if (!value)
                {
                    _framebuffer = null;
                    return;
                }
                if (_framebuffer != null) { return; }

                SetFramebuffer();
            }
        }
        private void SetFramebuffer()
        {
            int w = _bounds.Width <= 0 ? BaseFramebuffer.Properties.Width : (int)_bounds.Width;
            int h = _bounds.Height <= 0 ? BaseFramebuffer.Properties.Height : (int)_bounds.Height;

            _framebuffer = new TextureRenderer(w, h);
            _framebuffer.SetColourAttachment(0, TextureFormat.Rgba);
            _framebuffer.GetTexture(FrameAttachment.Colour0).MinFilter = TextureSampling.Nearest;
            _framebuffer.GetTexture(FrameAttachment.Colour0).MagFilter = TextureSampling.Nearest;
            _framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
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

        private Box _bounds;
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
            get
            {
                if (Parent == null)
                {
                    Vector2 wml = _window.MouseLocation;

                    return (wml.X, -wml.Y) - (_window.Size * 0.5);
                }

                return Parent.MouseLocation - _bounds.Centre;
            }
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

        private void BoundsSet(Box b)
        {
            OnSizeChange(new VectorEventArgs(b.Size));
            OnElementMove(new VectorEventArgs(b.Location));
        }
        /// <summary>
        /// The bounds of the element.
        /// </summary>
        /// <remarks>
        /// Cannot be set if <see cref="UsingLayout"/> is <see cref="true"/>.
        /// </remarks>
        public Box Bounds
        {
            get => _bounds;
            set
            {
                if (Layout != null) { return; }

                BoundsSet(value);

                if (Parent == null) { return; }
                Parent._triggerMouseMove = true;
            }
        }
        /// <summary>
        /// The size of the element.
        /// </summary>
        /// <remarks>
        /// Cannot be set if <see cref="UsingLayout"/> is <see cref="true"/>.
        /// </remarks>
        public Vector2 Size
        {
            get => _bounds.Size;
            set
            {
                if (Layout != null) { return; }

                OnSizeChange(new VectorEventArgs(value));

                if (Parent == null) { return; }
                Parent._triggerMouseMove = true;
            }
        }
        /// <summary>
        /// The position of the element.
        /// </summary>
        /// <remarks>
        /// Cannot be set if <see cref="UsingLayout"/> is <see cref="true"/>.
        /// </remarks>
        public Vector2 Location
        {
            get => _bounds.Location;
            set
            {
                if (Layout != null) { return; }

                OnElementMove(new VectorEventArgs(value));

                if (Parent == null) { return; }
                Parent._triggerMouseMove = true;
            }
        }
        private Vector2 _boundOffset;
        /// <summary>
        /// The offset added the the drawing bound's size.
        /// </summary>
        protected Vector2 DrawingBoundOffset
        {
            get => _boundOffset;
            set
            {
                if (_boundOffset == value) { return; }
                _boundOffset = value;

                //SetProjection();
                OnSizeChange(new VectorEventArgs(_bounds.Size));
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
        public event MouseEventHandler Click;

        public event VectorEventHandler SizeChange;
        public event VectorEventHandler ElementMove;

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

        /// <summary>
        /// Get the child element at an index.
        /// </summary>
        /// <param name="index">The index of the retrieved element.</param>
        /// <returns></returns>
        public Element this[int index] => _elements[index];

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
            // Trigger OnStart if window already executing
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

            // Element not a child of this
            if (!_elements.Remove(e)) { return false; }
            
            _layouts.Remove(e);
            e._hoverIndex = -1;
            _hover.Remove(e);
            e._mouseOver = false;

            e.Parent = null;
            e.SetWindow(null);

            return true;
        }
        
        private Vector2 RenderOffset
        {
            get
            {
                if (Parent == null)
                {
                    return Vector2.Zero;
                }
                
                return Parent.RenderOffset + Parent._bounds.Centre;
            }
        }
        
        private void SetViewSize()
        {
            Framebuffer.ViewSize = (Vector2I)(_bounds.Size + _boundOffset);
        }
        private void SetViewPos()
        {
            if (HasFramebuffer) { return; }

            Box drawingBounds = new Box(_bounds.Centre, _bounds.Size + _boundOffset);

            // Create viewport offset if this element is drawing
            // straight to the parents framebuffer
            Parent.Framebuffer.ViewLocation = (Vector2I)
                ((
                    (Parent._bounds.Width * 0.5) + drawingBounds.Left,
                    (Parent._bounds.Height * 0.5) + drawingBounds.Bottom
                ) + RenderOffset);
        }

        private bool _inRender = false;
        internal void Render(IFramebuffer framebuffer, Matrix4 projection)
        {
            if (!_render) { return; }
            if (HasFramebuffer && Shader == null) { return; }

            // Caused by a change in size or position of a child element
            // Thus mouse hover may become inaccurate
            if (_triggerMouseMove && MouseHover)
            {
                Vector2 mp = _mousePos;
                _mousePos = double.NaN;
                OnMouseMove(new MouseEventArgs(mp));
                _triggerMouseMove = false;
                _window.CursorStyle = _currentCursor;
            }

            _triggerMouseMove = false;

            Framebuffer.Bind();
            _inRender = true;

            // Set viewport
            SetViewPos();
            SetViewSize();
            OnUpdate(new FrameEventArgs(Framebuffer));

            _inRender = false;

            State.DepthTesting = false;

            // Draw element framebuffer if one exists
            if (HasFramebuffer && Parent != null)
            {
                // Set viewport to normal
                Parent.Framebuffer.ViewLocation = 0;
                Parent.Framebuffer.ViewSize = Parent.Framebuffer.Size;
                // Create drawing context
                framebuffer.Bind();
                Shader.Bind();
                Vector2 offset = RenderOffset;
                /*Shader.SetMatrices(
                    Matrix4.CreateBox(new Box(drawingBounds.Location + offset, drawingBounds.Size)),
                    Matrix4.Identity,
                    projection);*/
                Shader.Matrix1 = Matrix4.CreateBox(new Box(_bounds.Location + offset, _bounds.Size + _boundOffset));
                Shader.Matrix2 = Matrix4.Identity;
                Shader.Matrix3 = projection;
                Shader.ColourSource = ColourSource.Texture;
                Shader.TextureSlot = 0;
                // Framebuffer texture
                _framebuffer.GetTexture(FrameAttachment.Colour0).Bind();
                // Draw element
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

                    // Set text render projection
                    textRender.Projection = span[i].Projection;
                    // Default colour
                    textRender.Colour = new Colour(255, 255, 255);

                    span[i].Render(framebuffer, projection);
                }
            }
        }

        internal void OnStart()
        {
            // Set bounds to 0 so event methods are not ignored
            Box rect = _bounds;
            _bounds = new Box();

            OnSizeChange(new VectorEventArgs(rect.Size));
            OnElementMove(new VectorEventArgs(rect.Location));
            
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
                // Remove element if no longer a valid item
                if (!span[i].Visable)
                {
                    span[i]._hoverIndex = -1;
                    _hover.RemoveAt(i);
                    i--;
                    continue;
                }

                span[i].OnScroll(e);
            }
        }

        private int _hoverIndex = -1;
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
                    span[i]._hoverIndex = -1;
                    _hover.RemoveAt(i);
                    i--;
                }
            }
        }

        private bool _triggerMouseMove = false;
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            if (_mousePos == e.Location) { return; }
            _mousePos = e.Location;

            MouseMove?.Invoke(this, e);

            lock (_elementRef)
            {
                Cursor newCursor = CursorStyle;

                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].Visable) { continue; }

                    Cursor c = ManageMouseMove(span[i], e);
                    // No curser set - mouse not over element
                    if (c == null) { continue; }

                    newCursor = c;
                }

                _currentCursor = newCursor;
            }
        }
        private Cursor ManageMouseMove(Element e, MouseEventArgs m)
        {
            bool mouseOverNew = e._bounds.Contains(m.Location);

            // Mouse not in bounds
            if ((mouseOverNew == e._mouseOver) && (e._mouseOver == false)) { return null; }
            // mouseOverNew will also equal true
            // Mouse moves inside bounds
            if (mouseOverNew == e._mouseOver)
            {
                e.OnMouseMove(new MouseEventArgs(m.Location - e._bounds.Centre));
                return e._currentCursor;
            }

            // Mouse enters bounds
            if (mouseOverNew)
            {
                e._mouseOver = true;
                e.OnMouseEnter(new EventArgs());
                e.OnMouseMove(new MouseEventArgs(e.Location - e._bounds.Centre));

                // Add to hover
                // Will be false if element is already in hover
                if (e._hoverIndex < 0)
                {
                    e._hoverIndex = _hover.Count;
                    _hover.Add(e);
                }
                return e._currentCursor;
            }

            // Mouse leaves bounds
            e.OnMouseLeave(new EventArgs());
            
            // Element should stay in hover until OnMouseUp
            // Removed if not currently being clicked
            if (!MouseSelect)
            {
                e._hoverIndex = -1;
                _hover.Remove(e);
            }

            return null;
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
                // Remove element if no longer a valid item
                if (!span[i].Visable)
                {
                    span[i]._hoverIndex = -1;
                    _hover.RemoveAt(i);
                    i--;
                    continue;
                }

                span[i].OnMouseDown(e);
            }
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (MouseSelect)
            {
                Click?.Invoke(this, e);
            }

            if (_window.MouseButton == MouseButton.None)
            {
                MouseSelect = false;
            }

            MouseUp?.Invoke(this, e);

            // Update child elements
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);

            for (int i = 0; i < _hover.Count; i++)
            {
                // Remove element if no longer a valid item
                if (!span[i].Visable)
                {
                    span[i]._hoverIndex = -1;
                    _hover.RemoveAt(i);
                    i--;
                    continue;
                }

                span[i].OnMouseUp(e);

                // Mouse not hovering over and no mouse buttons pressed
                if (!span[i]._mouseOver && !MouseSelect)
                {
                    span[i]._hoverIndex = -1;
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

        protected void TriggerLayout()
        {
            if (Layout == null || Parent == null) { return; }

            BoundsSet(Layout.GetBounds(this, Parent.Size));
            Parent._triggerMouseMove = true;
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
        private Vector2 _boundOffsetReference;
        protected virtual void OnSizeChange(VectorEventArgs e)
        {
            // Cannot render an object this no inner content
            if (e.X <= 0 || e.Y <= 0)
            {
                _render = false;
                return;
            }
            else { _render = true; }

            // No bounds have changed - false call
            if (_bounds.Size == e.Value && _boundOffset == _boundOffsetReference) { return; }
            bool sameBounds = _bounds.Size == e.Value;
            bool sameBoundOffset = _boundOffset == _boundOffsetReference;
            _bounds.Size = e.Value;
            _boundOffsetReference = _boundOffset;

            SetProjection();

            // Resize framebuffer if one is being managed
            if (HasFramebuffer)
            {
                Actions.Push(() =>
                {
                    _framebuffer.Size = (Vector2I)(e.Value + _boundOffset);
                });
            }
            // If this call comes from inside the render function, on that thread
            // Set framebuffer viewport and set text projection
            if (_inRender && Actions.CurrentThread)
            {
                textRender.Projection = Projection;

                SetViewSize();

                if (!sameBoundOffset) { SetViewPos(); }
            }

            SizeChange?.Invoke(this, e);

            // Only change child elements if this OnSizeChange was caused
            // by a change in _bounds, not _boundOffset
            if (sameBounds) { return; }

            // Update child elements
            lock (_elLayoutRef)
            {
                Vector2 multiplier = e.Value * 0.5;
                Span<Element> span = CollectionsMarshal.AsSpan(_layouts);

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].Visable) { continue; }

                    span[i].BoundsSet(span[i].Layout.GetBounds(span[i], e.Value));
                }
            }
        }
        protected virtual void OnElementMove(VectorEventArgs e)
        {
            if (_bounds.Location == e.Value) { return; }
            _bounds.Location = e.Value;

            ElementMove?.Invoke(this, e);

            // Set viewport if this was called from inside the render function
            if (_inRender && Actions.CurrentThread)
            {
                SetViewPos();
            }
        }

        protected virtual void OnUpdate(FrameEventArgs e)
        {
            Update?.Invoke(this, e);
        }
    }
}