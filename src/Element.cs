using System;
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
        
        internal ElementManager _handle;
        protected ElementManager Manager => _handle;
        
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
                
                if (_handle == null) { return; }

                if (_useLayout)
                {
                    _handle.AddLayout(this);
                    return;
                }
                
                _handle.RemoveLayout(this);
            }
        }
        
        public Vector2 MouseLocation
        {
            get => _mousePos;
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
        
        public Cursor CursorStyle { get; set; }

        private Box _hoverBounds => new Box(_bounds.Center, _bounds.Size + 10d);
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
        
        /// <summary>
        /// Determines whether <paramref name="key"/> is currently being pressed.
        /// </summary>
        /// <param name="key">THe key to query</param>
        /// <returns></returns>
        public bool this[Keys key]
        {
            get => _handle[key];
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
            get => _handle[mod];
        }
        
        internal void Render(IFramebuffer framebuffer, IBasicShader shader, IDrawable draw)
        {
            Framebuffer.Bind();
            Shader.Bind();
            
            OnUpdate(new FrameEventArgs(Framebuffer, Shader));

            framebuffer.Bind();
            shader.Bind();
            shader.Matrix1 = Matrix4.CreateBox(_bounds);
            shader.ColourSource = ColourSource.Texture;
            shader.TextureSlot = 0;
            Framebuffer.GetTexture(FrameAttachment.Colour0).Bind();
            draw.Draw();
        }
        
        internal void OnStart()
        {
            OnSizeChange(new SizeChangeEventArgs(_bounds.Size));
            OnElementMove(new PositionEventArgs(_bounds.Location));
        }

        private bool _mouseOver = false;
        internal MouseMange ManageMouse(Vector2 location)
        {
            bool mouseOverNew = _bounds.Contains(location);

            if ((mouseOverNew == _mouseOver) && (_mouseOver == false)) { return MouseMange.NoMouse; }

            // Must also equal true
            if (mouseOverNew == _mouseOver)
            {
                OnMouseMove(new MouseEventArgs(location - _bounds.Center));
                return MouseMange.MouseOver;
            }
            
            if (!mouseOverNew)
            {
                _mouseOver = false;
                OnMouseLeave(new EventArgs());
                return MouseMange.MouseLeave;
            }
            
            _mouseOver = true;
            OnMouseEnter(new EventArgs());
            OnMouseMove(new MouseEventArgs(location - Bounds.Center));
            return MouseMange.MouseEnter;
        }
        
        private void CheckForResize()
        {

        }

        protected internal virtual void OnTextInput(TextInputEventArgs e)
        {
            TextInput?.Invoke(this, e);
        }
        protected internal virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }
        protected internal void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }
        protected internal virtual void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(this, e);
        }
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
            if (UserResizable)
            {
                CheckForResize();
            }

            lock (_mousePosRef)
            {
                _mousePos = e.Location;
            }
            MouseMove?.Invoke(this, e);
        }
        protected internal virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseDown?.Invoke(this, e);
        }
        protected internal virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }
        protected virtual void OnSizeChange(SizeChangeEventArgs e)
        {
            lock (_boundsRef)
            {
                _bounds.Size = e.Size;
            }
            Framebuffer.ViewSize = e.Size;
            Framebuffer.Size = e.Size;
            if (Shader != null)
            {
                Shader.Matrix3 = Matrix4.CreateOrthographic(e.Width, e.Height, 0d, 1d);
            }
            
            SizeChange?.Invoke(this, e);
        }
        protected virtual void OnElementMove(PositionEventArgs e)
        {
            lock (_boundsRef)
            {
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