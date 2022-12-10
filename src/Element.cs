using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public abstract class Element
    {
        public Element(IBox bounds)
        {
            _bounds = new RectangleI(bounds);
            
            Framebuffer = new TextureRenderer(_bounds.Width, _bounds.Height);
            Framebuffer.SetColourAttachment(0, TextureFormat.Rgba);
            Framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
        }

        internal Window _window;
        public Element Parent { get; private set; }

        internal virtual TextRenderer textRender => Parent.textRender;
        public TextRenderer TextRenderer => textRender;

        public TextureRenderer Framebuffer { get; }
        public abstract IBasicShader Shader { get; }
        
        private readonly object _boundsRef = new object();
        private RectangleI _bounds;
        private readonly object _mousePosRef = new object();
        private Vector2 _mousePos;
        
        public Box Layout { get; set; }
        private bool _useLayout = false;
        public bool UseLayout
        {
            get => _useLayout;
            set
            {
                _useLayout = value;
                
                if (Parent == null) { return; }

                if (_useLayout)
                {
                    Parent.AddLayout(this);
                    return;
                }

                Parent.RemoveLayout(this);
            }
        }
        
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
        
        public RectangleI Bounds
        {
            get => _bounds;
            set
            {
                OnSizeChange(new SizeChangeEventArgs(value.Size));
                OnElementMove(new PositionEventArgs(value.Location));
            }
        }
        public Vector2I Size
        {
            get => _bounds.Size;
            set
            {
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

        internal Cursor _currentCursor;
        public Cursor CursorStyle { get; set; }

        public bool UserResizable { get; internal set; }

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
        public bool this[Keys key]
        {
            get => _window[key];
        }
        /// <summary>
        /// Determines whether <paramref name="mod"/> is currently active.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="NotSupportedException"/> if <paramref name="mod"/> is <see cref="Mods.CapsLock"/> or <see cref="Mods.NumLock"/>.
        /// </remarks>
        /// <param name="mod">The modifier to query.</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public bool this[Mods mod]
        {
            get => _window[mod];
        }

        public void AddElement(Element e)
        {
            if (e.Parent != null)
            {
                throw new ArgumentException("Element already managed.", nameof(e));
            }

            e.Parent = this;
            if (_window != null)
            {
                e._window = _window;

                // Set child elements
                lock (e._elementRef)
                {
                    Span<Element> span = CollectionsMarshal.AsSpan(e._elements);

                    for (int i = 0; i < span.Length; i++)
                    {
                        span[i]._window = _window;
                    }
                }
            }

            lock (_elementRef)
            {
                _elements.Add(e);
            }

            if (e.UseLayout) { AddLayout(e); }

            if (_window != null && _window.Running) { e.OnStart(); }
        }

        internal void Render(IFramebuffer framebuffer, Matrix4 projection, IDrawable draw)
        {
            if (Shader == null) { return; }

            Framebuffer.Bind();
            Shader.Bind();

            Shader.SetMatrices(
                Matrix4.Identity,
                Matrix4.Identity,
                Projection);

            OnUpdate(new FrameEventArgs(Framebuffer, Shader));

            // Draw child elements
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    textRender.Projection = span[i].Projection;

                    span[i].Render(Framebuffer, Projection, draw);
                }
            }

            framebuffer.Bind();
            Shader.Bind();
            Shader.SetMatrices(
                Matrix4.CreateBox(_bounds),
                Matrix4.Identity,
                projection);
            Shader.ColourSource = ColourSource.Texture;
            Shader.TextureSlot = 0;
            Framebuffer.GetTexture(FrameAttachment.Colour0).Bind();
            draw.Draw();
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
            MouseLeave?.Invoke(this, e);
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

                        _hover.Add(span[i]);
                        newCursor = span[i]._currentCursor;
                        continue;
                    }

                    // Mouse leaves bounds
                    span[i]._mouseOver = false;
                    span[i].OnMouseLeave(new EventArgs());
                    _hover.Remove(span[i]);
                }

                _currentCursor = newCursor;
            }
        }
        private bool _mouseOver = false;
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
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
            MouseUp?.Invoke(this, e);

            // Update child elements
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);

            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnMouseUp(e);
            }
        }

        internal void AddLayout(Element e)
        {
            lock (_elLayoutRef)
            {
                _layouts.Add(e);
            }

            Vector2 multiplier = _bounds.Size * 0.5;

            Vector2 tl = (e.Layout.Left, e.Layout.Top);

            e.Bounds = new RectangleI(
                tl * multiplier,
                e.Layout.Size * multiplier
            );
        }
        internal void RemoveLayout(Element e)
        {
            lock (_elLayoutRef)
            {
                _layouts.Remove(e);
            }
        }

        private readonly object _elLayoutRef = new object();
        private readonly List<Element> _layouts = new List<Element>();

        public Matrix4 Projection { get; private set; }
        protected virtual void OnSizeChange(SizeChangeEventArgs e)
        {
            lock (_boundsRef)
            {
                //if (_bounds.Size == e.Size) { return; }

                _bounds.Size = e.Size;
            }
            Framebuffer.ViewSize = e.Size;
            Framebuffer.Size = e.Size;
            Projection = Matrix4.CreateOrthographic(e.Width, e.Height, 0d, 1d);
            
            SizeChange?.Invoke(this, e);

            // Update child elements
            lock (_elLayoutRef)
            {
                Vector2 multiplier = e.Size * 0.5;
                Span<Element> span = CollectionsMarshal.AsSpan(_layouts);

                for (int i = 0; i < span.Length; i++)
                {
                    Vector2 tl = (span[i].Layout.Left, span[i].Layout.Top);

                    span[i].Bounds = new RectangleI(
                        tl * multiplier,
                        span[i].Layout.Size * multiplier
                    );
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