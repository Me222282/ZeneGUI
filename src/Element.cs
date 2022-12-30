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
            _hover = this;
            _finalHover = this;
        }
        /// <summary>
        /// Creates an element from a layout.
        /// </summary>
        /// <param name="layout">The layout of the element.</param>
        /// <param name="framebuffer">Whether to create a framebuffer.</param>
        public Element(ILayout layout)
            : this()
        {
            Layout = layout;
        }

        protected internal Element(TextRenderer textRenderer)
        {
            _textRender = textRenderer;
        }

        internal Window _window;
        /// <summary>
        /// The parent element.
        /// </summary>
        public Element Parent { get; private set; }
        /// <summary>
        /// The root element of this GUI instance.
        /// </summary>
        public RootElement RootElement { get; internal set; } = null;

        private TextRenderer _textRender;
        /// <summary>
        /// Text rendering object.
        /// </summary>
        public TextRenderer TextRenderer => _textRender;

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

        private Vector2 CalculateMousePos(Vector2 rootPos)
        {
            if (Parent == null) { return rootPos; }

            return ((Parent.CalculateMousePos(rootPos) - Parent._viewPan) / Parent._viewScale) - _bounds.Centre;
        }
        /// <summary>
        /// The local mouse position.
        /// </summary>
        public Vector2 MouseLocation
        {
            get
            {
                // Mouse pos already calculated
                if (_mouseOver) { return _mousePos; }

                if (Parent == null)
                {
                    Vector2 wml = _window.MouseLocation;

                    return (wml.X, -wml.Y) - (_window.Size * (0.5, -0.5));
                }

                return ((Parent.MouseLocation - Parent._viewPan) / Parent._viewScale) - _bounds.Centre;
            }
            set
            {
                if (Parent == null)
                {
                    _window.MouseLocation = (value.X, -value.Y) + (_window.Size * 0.5);
                    return;
                }

                Parent.MouseLocation = ((value + _bounds.Centre) * Parent._viewScale) + Parent._viewPan;
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
        /// Can only be set if <see cref="Layout"/> is of type <see cref="FixedLayout"/>.
        /// </remarks>
        public Box Bounds
        {
            get => _bounds;
            set
            {
                if (Layout is not FixedLayout fl) { return; }

                fl.Bounds = value;
            }
        }
        /// <summary>
        /// The size of the element.
        /// </summary>
        /// <remarks>
        /// Can only be set if <see cref="Layout"/> is of type  <see cref="FixedLayout"/>.
        /// </remarks>
        public Vector2 Size
        {
            get => _bounds.Size;
            set
            {
                if (Layout is not FixedLayout fl) { return; }

                fl.Size = value;
            }
        }
        /// <summary>
        /// The position of the element.
        /// </summary>
        /// <remarks>
        /// Can only be set if <see cref="Layout"/> is of type  <see cref="FixedLayout"/>.
        /// </remarks>
        public Vector2 Location
        {
            get => _bounds.Location;
            set
            {
                if (Layout is not FixedLayout fl) { return; }

                fl.Location = value;
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
                SizeChangeListener(new VectorEventArgs(_bounds.Size));
            }
        }

        /// <summary>
        /// Determines whether the mouse is hovering over this element.
        /// </summary>
        public bool MouseHover
        {
            get
            {
                if (Parent == null)
                {
                    return _hover == this;
                }

                return _hover == this && _mouseOver;
            }
        }
        /// <summary>
        /// Determines whether the mouse cursor is inside the bounds of this element.
        /// </summary>
        public bool MouseInBounds => _mouseOver;
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

        public bool TabShifting { get; set; } = true;

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
        /// Gets the number of children this element has.
        /// </summary>
        public int ChildCount => _elements.Count;
        /// <summary>
        /// Get the child element at an index.
        /// </summary>
        /// <param name="index">The index of the retrieved element.</param>
        /// <returns></returns>
        public Element this[int index] => _elements[index];

        private void SetRoots()
        {
            _window = Parent?._window;
            RootElement = Parent?.RootElement;
            _textRender = Parent?._textRender;

            // Set child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].SetRoots();
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
            if (_window != null) { e.SetRoots(); }

            lock (_elementRef)
            {
                _elementIndex = _elements.Count;

                _elements.Add(e);
            }

            // Has layout
            if (e.Layout != null) { TriggerLayout(); }
        }

        private static void ResetElement(Element e)
        {
            e._elementIndex = -1;
            e._mouseOver = false;
            e.Focused = false;

            e.Parent = null;
            e.SetRoots();
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

            lock (_elementRef)
            {
                // Element not a child of this
                if (!_elements.Remove(e)) { return false; }
            }

            ResetElement(e);
            TriggerLayout();

            return true;
        }
        /// <summary>
        /// Removes all child elements.
        /// </summary>
        public void ClearChildren()
        {
            foreach (Element e in _elements)
            {
                ResetElement(e);
            }

            lock (_elementRef)
            {
                _elements.Clear();
            }

            TriggerLayout();
        }

        /// <summary>
        /// Finds the first element of a certain type.
        /// </summary>
        /// <typeparam name="T">The type of element to search for.</typeparam>
        /// <returns></returns>
        public T Find<T>()
            where T : Element
        {
            if (this is T)
            {
                return this as T;
            }

            lock (_elements)
            {
                foreach (Element e in _elements)
                {
                    T test = e.Find<T>();

                    if (test != null)
                    {
                        return test;
                    }
                }
            }

            return null;
        }

        private Vector2 _renderOffset = Vector2.Zero;
        private double _renderScale = 1d;
        private void RenderOffsetScale()
        {
            if (Parent == null) { return; }

            _renderScale = Parent._renderScale * Parent._viewScale;

            _renderOffset = Parent._renderOffset + ((Parent._bounds.Centre + Parent._viewPan) * Parent._renderScale);
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

            RenderOffsetScale();
            Box drawingBounds = new Box(_bounds.Centre * _renderScale, (_bounds.Size + _boundOffset) * _renderScale);

            _viewReference = new RectangleI((
                    // Location
                    (_window.Width * 0.5) + drawingBounds.Left,
                    (_window.Height * 0.5) + drawingBounds.Bottom
                ) + _renderOffset,
                // Size
                drawingBounds.Size);
        }
        private void SetViewport()
        {
            RootElement._framebuffer.Viewport.View = _viewReference;
        }
        private void SetScissor()
        {
            if (Parent == null)
            {
                RootElement._framebuffer.Scissor.View = _viewReference;
                _scissorView = _viewReference;
                return;
            }

            RectangleI scissor = ViewClamp(_viewReference, Parent._scissorView);

            // Element outside bounds of parent
            if (scissor.Width < 0 || scissor.Height < 0)
            {
                scissor.Size = Vector2I.Zero;
            }

            RootElement._framebuffer.Scissor.View = scissor;
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
                MouseMoveListener(new MouseEventArgs(_mousePos), false);
                _window.CursorStyle = _currentCursor;
            }

            _triggerMouseMove = false;

            RootElement._framebuffer.Bind();
            _inRender = true;

            SetViewRef();
            // Scissor
            SetScissor();
            // Element won't be drawn
            if (RootElement._framebuffer.Scissor.Width == 0 ||
                RootElement._framebuffer.Scissor.Height == 0)
            {
                return;
            }

            // Set viewport
            SetViewport();
            OnUpdate(new FrameEventArgs(RootElement._framebuffer));

            _inRender = false;

            // Draw child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    if (!span[i].Visable) { continue; }

                    State.DepthTesting = false;

                    span[i].SetView();

                    // Set text render projection
                    _textRender.Projection = span[i].Projection;
                    _textRender.View = Matrix4.Identity;
                    _textRender.Model = Matrix4.Identity;
                    // Default colour
                    _textRender.Colour = new Colour(255, 255, 255);

                    span[i].Render(_viewRef * view, projection);
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

        protected internal virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);

            // Not container for hover element
            if (_hover == this) { return; }

            _finalHover.OnScroll(e);
        }

        //private int _hoverIndex = -1;
        //private readonly List<Element> _hover = new List<Element>();
        private Element _hover;
        private Element _finalHover;
        public Element Hover => _finalHover;
        protected virtual void OnMouseEnter(EventArgs e)
        {
            MouseEnter?.Invoke(this, e);
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            _mouseOver = false;

            MouseLeave?.Invoke(this, e);
        }

        private bool _triggerMouseMove = false;
        private void TriggerFullMouseMove() => RootElement._triggerMouseMove = true;

        internal Element MouseMoveListener(MouseEventArgs e, bool check = true)
        {
            if (check && _mousePos == e.Location) { return _finalHover; }
            _mousePos = e.Location;

            if (_window.MouseButton != MouseButton.None && _hover != this)
            {
                Element hover = _finalHover;

                Vector2 hoverMP = hover.CalculateMousePos(e.Location);
                if (hover._bounds.Contains(hoverMP))
                {
                    hover._mouseOver = true;
                    hover._mousePos = hoverMP;
                    hover.OnMouseMove(new MouseEventArgs(hoverMP));
                }
                else
                {
                    hover._mouseOver = false;
                }
                return hover;
            }

            Vector2 mouseLocal = (e.Location - _viewPan) / _viewScale;

            bool elementHover = false;

            Element oldHover = _hover;
            _hover = this;
            _finalHover = this;

            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                //for (int i = 0; i < span.Length; i++)
                // Reverse for loop - first element found backwards is the one in front
                for (int i = span.Length - 1; i >= 0; i--)
                {
                    if (!span[i].Visable) { continue; }

                    bool hover = ManageMouseMove(span[i], mouseLocal);
                    // _hover has been set - element hover
                    if (!hover) { continue; }

                    elementHover = true;
                    break;
                }
            }

            if (oldHover != _hover)
            {
                if (oldHover != null)
                {
                    EventArgs mlE = new EventArgs();

                    oldHover.OnMouseLeave(mlE);
                    
                    if (oldHover._hover != oldHover)
                    {
                        oldHover._finalHover.OnMouseLeave(mlE);
                    }
                }

                _hover._mouseOver = true;
                _hover.OnMouseEnter(new EventArgs());
            }

            // Could have been set to false in previous if statement
            _mouseOver = true;

            if (!elementHover)
            {
                _currentCursor = CursorStyle;
                OnMouseMove(e);
            }
            else
            {
                _finalHover = _hover.MouseMoveListener(new MouseEventArgs(mouseLocal - _hover._bounds.Centre), check);

                _currentCursor = _hover._currentCursor;
            }

            // Set cursor if this is root element
            if (Parent == null)
            {
                _window.CursorStyle = _currentCursor;
            }

            return _finalHover;
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseMove?.Invoke(this, e);
        }
        private bool ManageMouseMove(Element e, Vector2 mousePos)
        {
            bool mouseOverNew = e._bounds.Contains(mousePos);

            // Mouse not in bounds
            if (!mouseOverNew) { return false; }

            // Mouse inside bounds
            _hover = e;
            return true;
        }

        internal bool _mouseOver = false;
        protected internal virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseSelect = true;

            MouseDown?.Invoke(this, e);

            // Not container for hover element
            if (_hover == this) { return; }

            _finalHover.OnMouseDown(new MouseEventArgs(_finalHover.MouseLocation, e.Button, e.Modifier));
        }
        protected internal virtual void OnMouseUp(MouseEventArgs e)
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
            if (_hover == this) { return; }

            _finalHover.OnMouseUp(new MouseEventArgs(_finalHover.MouseLocation, e.Button, e.Modifier));

            if (!MouseSelect && Parent == null)
            {
                TriggerFullMouseMove();
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
                BoundsSet(Layout.GetBounds(new LayoutArgs(this, Parent.Size, _elementIndex, Parent._elements)));
            }
            TriggerFullMouseMove();
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
                _triggerMouseMove = true;

                if (_inRender)
                {
                    SetView();
                    _textRender.Projection = Projection;
                }
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
                _triggerMouseMove = true;

                if (_inRender)
                {
                    SetView();
                    _textRender.Projection = Projection;
                }
            }
        }

        private Matrix4 _viewRef = Matrix4.Identity;
        private void SetView()
        {
            _viewRef = Matrix4.CreateBox(new Box(_viewPan, _viewScale));
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
                _textRender.Projection = Projection;

                // Set reference for viewport when rendered
                SetViewRef();
                SetScissor();
                SetViewport();
            }

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

                    span[i].BoundsSet(span[i].Layout.GetBounds(new LayoutArgs(span[i], e.Value, i, _elements)));
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

        internal Element LowestFirstElement()
        {
            if (_elements.Count == 0) { return this; }

            return _elements[0].LowestFirstElement();
        }
        internal Element LowestLastElement()
        {
            if (_elements.Count == 0) { return this; }

            return _elements[^1].LowestLastElement();
        }

        internal Element NextElement()
        {
            if (Parent == null) { return null; }

            Element next;

            // This is last element
            if (_elementIndex == (Parent._elements.Count - 1))
            {
                next = Parent.NextElement();

                // Loop round to first element
                if (next == null)
                {
                    next = RootElement;
                }
            }
            else
            {
                next = Parent._elements[_elementIndex + 1];
            }

            return next?.LowestFirstElement();
        }
        internal Element PreviousElement()
        {
            if (Parent == null) { return null; }

            Element next;

            // This is fisrt element
            if (_elementIndex == 0)
            {
                next = Parent.PreviousElement();

                // Loop round to first element
                if (next == null)
                {
                    next = RootElement;
                }
            }
            else
            {
                next = Parent._elements[_elementIndex - 1];
            }

            return next?.LowestLastElement();
        }
    }
}