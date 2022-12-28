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
        /// Creates an element.
        /// </summary>
        public Element()
        {

        }
        /// <summary>
        /// Creates an element from a layout.
        /// </summary>
        /// <param name="layout">The layout of the element.</param>
        /// <param name="framebuffer">Whether to create a framebuffer.</param>
        public Element(ILayout layout)
        {
            Layout = layout;
        }

        internal Element(Box bounds)
        {
            _bounds = bounds;
        }

        internal Window _window;
        /// <summary>
        /// The parent element.
        /// </summary>
        public Element Parent { get; private set; }
        /// <summary>
        /// The root element of this GUI instance.
        /// </summary>
        public Element RootElement
        {
            get
            {
                if (Parent == null) { return this; }

                return Parent.RootElement;
            }
        }

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

        internal virtual IFramebuffer framebuffer => Parent.framebuffer;
        /// <summary>
        /// The framebuffer objects are drawn to.
        /// </summary>
        public IFramebuffer Framebuffer => framebuffer;

        private Box _bounds;
        private Vector2 _mousePos;

        private ILayout _layout = null;
        /// <summary>
        /// The layout of the element. Used to calculate the element's bounds.
        /// </summary>
        public ILayout Layout
        {
            get => _layout;
            set
            {
                if (_layout == value) { return; }

                if (_layout != null)
                {
                    _layout.Change -= LayoutChange;
                }

                _layout = value;
                _layout.Change += LayoutChange;

                if (Parent == null) { return; }

                if (value == null)
                {
                    BoundsSet(new Box());
                    return;
                }

                TriggerLayout();
            }
        }

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
            SizeChangeListener(new VectorEventArgs(b.Size));
            ElementMoveListener(new VectorEventArgs(b.Location));
        }
        /// <summary>
        /// The bounds of the element.
        /// </summary>
        /// <remarks>
        /// The set function sets <see cref="Layout"/> to a <see cref="FixedLayout"/> of the value.
        /// </remarks>
        public Box Bounds
        {
            get => _bounds;
            set => Layout = new FixedLayout(value);
        }
        /// <summary>
        /// The size of the element.
        /// </summary>
        public Vector2 Size => _bounds.Size;
        /// <summary>
        /// The position of the element.
        /// </summary>
        public Vector2 Location => _bounds.Location;
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
                SizeChangeListener(new VectorEventArgs(_bounds.Size));
            }
        }

        /// <summary>
        /// Determines whether the mouse is hovering over this element.
        /// </summary>
        public bool MouseHover => _mouseOver && _hover == null;
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
        /// <summary>
        /// Determines whether the element has input focus.
        /// </summary>
        public bool Focused { get; internal set; } = false;

        public bool UserResizable { get; protected set; }

        private int _elementIndex = -1;

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
        public event FocusedEventHandler Focus;

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
                _elementIndex = _elements.Count;

                _elements.Add(e);
            }

            // Has layout
            if (e.Layout != null) { TriggerLayout(); }
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

            _elementIndex = -1;
            e._mouseOver = false;

            e.Parent = null;
            e.SetWindow(null);

            TriggerLayout();

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
                
                return Parent.RenderOffset + Parent._bounds.Centre + Parent._viewPan;
            }
        }

        private static RectangleI ViewClamp(RectangleI a, RectangleI b)
        {
            int x = Math.Max(a.X, b.X);
            int y = Math.Max(a.Y, b.Y);

            int w = Math.Min(a.Right, b.Right) - x;
            int h = Math.Min(a.Y + a.Height, b.Y + b.Height) - y;

            return new RectangleI(x, y, w, h);
        }

        private RectangleI _viewReference;
        private RectangleI _scissorView;
        private void SetViewRef()
        {
            if (Parent == null || _window == null)
            {
                _viewReference = new RectangleI(Vector2.Zero, _bounds.Size);
                return;
            }

            Box drawingBounds = new Box(_bounds.Centre, _bounds.Size + _boundOffset);

            _viewReference = new RectangleI((
                // Location
                    (_window.Width * 0.5) + drawingBounds.Left,
                    (_window.Height * 0.5) + drawingBounds.Bottom
                ) + RenderOffset,
                // Size
                drawingBounds.Size);
        }
        private void SetViewport()
        {
            framebuffer.Viewport.View = _viewReference;
        }
        private void SetScissor()
        {
            if (Parent == null)
            {
                framebuffer.Scissor.View = _viewReference;
                _scissorView = _viewReference;
                return;
            }

            RectangleI scissor = ViewClamp(_viewReference, Parent._scissorView);

            // Element outside bounds of parent
            if (scissor.Width < 0 || scissor.Height < 0)
            {
                scissor.Size = Vector2I.Zero;
            }

            framebuffer.Scissor.View = scissor;
            _scissorView = scissor;
        }

        private bool _inRender = false;
        internal void Render(Matrix4 view, Matrix4 projection)
        {
            if (!_render) { return; }

            // Caused by a change in size or position of a child element
            // Thus mouse hover may become inaccurate
            if (_triggerMouseMove && _mouseOver)
            {
                Vector2 mp = _mousePos;
                _mousePos = double.NaN;
                MouseMoveListener(new MouseEventArgs(mp));
                _window.CursorStyle = _currentCursor;
            }

            _triggerMouseMove = false;

            framebuffer.Bind();
            _inRender = true;

            SetViewRef();
            // Scissor
            SetScissor();
            // Element won't be drawn
            if (framebuffer.Scissor.Width == 0 ||
                framebuffer.Scissor.Height == 0)
            {
                return;
            }

            // Set viewport
            SetViewport();
            OnUpdate(new FrameEventArgs(framebuffer));

            _inRender = false;
            
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

                    span[i].Render(_viewRef * view, projection);
                }
            }
        }

        internal void OnStart()
        {
            // Set bounds to 0 so event methods are not ignored
            Box rect = _bounds;
            _bounds = new Box();

            SizeChangeListener(new VectorEventArgs(rect.Size));
            ElementMoveListener(new VectorEventArgs(rect.Location));
            
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

        protected internal virtual void OnTextInput(TextInputEventArgs e)
        {
            TextInput?.Invoke(this, e);
            /*
            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnTextInput(e);
                }
            }*/
        }
        protected internal virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
            /*
            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnKeyDown(e);
                }
            }*/
        }
        protected internal virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
            /*
            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnKeyUp(e);
                }
            }*/
        }

        protected internal virtual void OnFocus(FocusedEventArgs e)
        {
            Focus?.Invoke(this, e);
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);

            // Not container for hover element
            if (_hover == null) { return; }

            Hover.OnScroll(e);
        }

        //private int _hoverIndex = -1;
        //private readonly List<Element> _hover = new List<Element>();
        private Element _hover = null;
        public Element Hover
        {
            get
            {
                if (_hover == null) { return this; }

                return _hover.Hover;
            }
        }
        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            _mouseOver = false;
            
            MouseLeave?.Invoke(this, e);

            // Not container for hover element
            if (_hover == null) { return; }

            Hover.OnMouseLeave(e);
            _hover = null;
        }

        private bool _triggerMouseMove = false;
        internal void MouseMoveListener(MouseEventArgs e)
        {
            if (_mousePos == e.Location) { return; }
            _mousePos = e.Location;

            if (MouseSelect && _hover != null)
            {
                //_hover.OnMouseMove(e);
                //ManageMouseMove(_hover, e);
                if (_hover._bounds.Contains(e.Location))
                {
                    _hover.OnMouseMove(e);
                }
                return;
            }

            lock (_elementRef)
            {
                Cursor newCursor = CursorStyle;

                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                bool elementHover = false;

                _hover = null;

                //for (int i = 0; i < span.Length; i++)
                // Reverse for loop - first element found backwards is the one in front
                for (int i = span.Length - 1; i >= 0; i--)
                {
                    if (!span[i].Visable) { continue; }

                    Cursor c = ManageMouseMove(span[i], e);
                    // _hover has been set - element hover
                    if (_hover == null) { continue; }

                    elementHover = true;

                    newCursor = c;
                    break;
                }

                if (!elementHover)
                {
                    _hover = null;
                    OnMouseMove(e);
                }

                _currentCursor = newCursor;
            }
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
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
                _hover = e;
                e.MouseMoveListener(new MouseEventArgs(m.Location - e._bounds.Centre));
                return e._currentCursor;
            }

            // Mouse enters bounds
            if (mouseOverNew)
            {
                e._mouseOver = true;
                _hover = e;
                e.OnMouseEnter(new EventArgs());
                e.MouseMoveListener(new MouseEventArgs(e.Location - e._bounds.Centre));

                return e._currentCursor;
            }

            // Mouse leaves bounds
            e.OnMouseLeave(new EventArgs());
            e._mouseOver = false;
            return null;
        }
        
        private bool _mouseOver = false;
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseSelect = true;

            MouseDown?.Invoke(this, e);

            // Not container for hover element
            if (_hover == null) { return; }

            Hover.OnMouseDown(e);
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            bool mouseDidSelect = MouseSelect;

            if (_window.MouseButton == MouseButton.None)
            {
                MouseSelect = false;
            }

            MouseUp?.Invoke(this, e);

            if (mouseDidSelect)
            {
                Click?.Invoke(this, e);
            }

            // Not container for hover element
            if (_hover == null) { return; }

            Hover.OnMouseUp(e);

            if (!_hover._mouseOver)
            {
                _mousePos = double.NaN;
                OnMouseMove(e);
            }
        }

        private void LayoutChange(object sender, EventArgs e) => TriggerLayout();
        /// <summary>
        /// Causes layout to be recalculated
        /// </summary>
        protected void TriggerLayout()
        {
            if (Layout == null || Parent == null) { return; }

            lock (Parent._elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(Parent._elements);

                BoundsSet(Layout.GetBounds(this, Parent.Size, _elementIndex, span));
            }
            Parent._triggerMouseMove = true;
        }

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

                SetView();
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

                SetView();
            }
        }

        private Matrix4 _viewRef = Matrix4.Identity;
        private void SetView()
        {
            _viewRef = Matrix4.CreateBox(new Box(_viewPan, ViewScale));
            CalculateProjMat();
        }
        private void SetProjection()
        {
            _projRef = Matrix4.CreateOrthographic(_bounds.Width + _boundOffset.X, _bounds.Height + _boundOffset.Y, 0d, 1d);
            CalculateProjMat();
        }
        private void CalculateProjMat() => Projection = _viewRef * _projRef;
        private Matrix4 _projRef = Matrix4.Identity;
        /// <summary>
        /// The projection matrix used to render objects to this element.
        /// </summary>
        public Matrix4 Projection { get; private set; } = Matrix4.Identity;
        /// <summary>
        /// The projection matrix used to render rixed objects to this element,
        /// like borders that shouldn't be affected by zoom and panning.
        /// </summary>
        public Matrix4 FixedProjection => _projRef;

        private bool _render = true;
        private Vector2 _boundOffsetReference;
        internal void SizeChangeListener(VectorEventArgs e)
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
            //bool sameBoundOffset = _boundOffset == _boundOffsetReference;
            _bounds.Size = e.Value;
            _boundOffsetReference = _boundOffset;

            SetProjection();

            // If this call comes from inside the render function, on that thread
            // Set framebuffer viewport and scissor and set text projection
            if (_inRender && Actions.CurrentThread)
            {
                textRender.Projection = Projection;

                SetViewRef();
                SetScissor();
                SetViewport();
            }
            // Set reference for viewport when rendered
            SetViewRef();

            OnSizeChange(e);

            // Only change child elements if this OnSizeChange was caused
            // by a change in _bounds, not _boundOffset
            if (sameBounds) { return; }

            // Update child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i]._elementIndex = i;
                    if (!span[i].Visable ||
                        span[i].Layout == null)// ||
                        //span[i].Layout is FixedLayout)
                    {
                        continue;
                    }

                    span[i].BoundsSet(span[i].Layout.GetBounds(span[i], e.Value, i, span));
                }
            }
        }
        protected virtual void OnSizeChange(VectorEventArgs e)
        {
            SizeChange?.Invoke(this, e);
        }
        private void ElementMoveListener(VectorEventArgs e)
        {
            if (_bounds.Location == e.Value) { return; }
            _bounds.Location = e.Value;

            // Set viewport and scissor if this is being called from render thread
            // And element is currently being rendered
            if (_inRender && Actions.CurrentThread)
            {
                SetViewRef();
                SetScissor();
                SetViewport();
            }

            OnElementMove(e);
        }
        protected virtual void OnElementMove(VectorEventArgs e)
        {
            ElementMove?.Invoke(this, e);
        }

        protected virtual void OnUpdate(FrameEventArgs e)
        {
            Update?.Invoke(this, e);
        }
    }
}