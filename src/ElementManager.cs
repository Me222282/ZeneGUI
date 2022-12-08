using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    internal enum MouseMange
    {
        MouseEnter,
        MouseLeave,
        MouseOver,
        NoMouse
    }

    public class ElementManager
    {
        private readonly DrawObject<float, byte> _square;
        private readonly BasicShader _shader;
        
        public TextRenderer TextRenderer { get; }
        
        public ElementManager(Window w)
        {
            _handle = w;
            
            _handle.MouseMove += MouseMove;
            _handle.MouseDown += MouseDown;
            _handle.MouseUp += MouseUp;
            _handle.Scroll += MouseScroll;
            _handle.KeyDown += KeyDown;
            _handle.KeyUp += KeyUp;
            _handle.TextInput += TextInput;
            _handle.SizeChange += SizeChange;
            _handle.Start += OnStart;

            _shader = new BasicShader
            {
                ColourSource = ColourSource.Texture
            };
            _square = new DrawObject<float, byte>(stackalloc float[]
            {
                0.5f, 0.5f, 1f, 1f,
                0.5f, -0.5f, 1f, 0f,
                -0.5f, -0.5f, 0f, 0f,
                -0.5f, 0.5f, 0f, 1f
            }, stackalloc byte[] { 0, 1, 2, 2, 3, 0 }, 4, 0, AttributeSize.D2, BufferUsage.DrawFrequent);
            _square.AddAttribute(2, 2, AttributeSize.D2);
            
            TextRenderer = new TextRenderer();
            
            SizeChange(this, new SizeChangeEventArgs(w.Size));
        }

        private readonly Window _handle;
        private readonly object _elementRef = new object();
        private readonly List<Element> _elements = new List<Element>();
        
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
        
        public void AddElement(Element e)
        {
            if (e._handle != null)
            {
                throw new ArgumentException("Element already managed.", nameof(e));
            }
            
            e._handle = this;
            
            lock (_elementRef)
            {
                _elements.Add(e);
            }

            if (e.UseLayout) { AddLayout(e); }

            if (_handle.Running) { e.OnStart(); }
        }
        
        public void Render()
        {
            _handle.Framebuffer.Bind();
            
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);
                
                for (int i = 0; i < span.Length; i++)
                {
                    TextRenderer.Projection = span[i].Shader.Matrix3;
                    
                    span[i].Render(_handle.Framebuffer, _shader, _square);
                }
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);

                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnStart();
                }
            }
        }

        private readonly List<Element> _hover = new List<Element>();
        private void MouseMove(object s, MouseEventArgs e)
        {
            Vector2 ml = e.Location - (_handle.Size * 0.5);
            ml.Y = -ml.Y;
            
            lock (_elementRef)
            {
                Cursor newCursor = null;

                Span<Element> span = CollectionsMarshal.AsSpan(_elements);
                
                for (int i = 0; i < span.Length; i++)
                {
                    MouseMange enter = span[i].ManageMouse(ml);
                    
                    if (enter == MouseMange.NoMouse) { continue; }
                    if (enter == MouseMange.MouseOver)
                    {
                        newCursor = span[i].CursorStyle;
                        continue;
                    }
                    
                    if (enter == MouseMange.MouseEnter)
                    {
                        _hover.Add(span[i]);
                        if (_mouseDown)
                        {
                            span[i].OnMouseDown(e);
                        }
                        newCursor = span[i].CursorStyle;
                        continue;
                    }
                    
                    _hover.Remove(span[i]);
                    if (_mouseDown)
                    {
                        span[i].OnMouseUp(e);
                    }
                }

                _handle.CursorStyle = newCursor;
            }
        }
        private bool _mouseDown = false;
        private void MouseDown(object s, MouseEventArgs e)
        {
            _mouseDown = true;
            
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);
            
            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnMouseDown(e);
            }
        }
        private void MouseUp(object s, MouseEventArgs e)
        {
            _mouseDown = false;
            
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);
            
            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnMouseUp(e);
            }
        }
        private void MouseScroll(object s, ScrollEventArgs e)
        {
            Span<Element> span = CollectionsMarshal.AsSpan(_hover);
            
            for (int i = 0; i < span.Length; i++)
            {
                span[i].OnScroll(e);
            }
        }
        
        private void TextInput(object s, TextInputEventArgs e)
        {
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);
                
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnTextInput(e);
                }
            }
        }
        private void KeyDown(object s, KeyEventArgs e)
        {
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);
                
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnKeyDown(e);
                }
            }
        }
        private void KeyUp(object s, KeyEventArgs e)
        {
            lock (_elementRef)
            {
                Span<Element> span = CollectionsMarshal.AsSpan(_elements);
                
                for (int i = 0; i < span.Length; i++)
                {
                    span[i].OnKeyUp(e);
                }
            }
        }
        
        internal void AddLayout(Element e)
        {
            _layouts.Add(e);
            
            Vector2 multiplier = _handle.Size * 0.5;
            
            Vector2 tl = (e.Layout.Left, e.Layout.Top);
            
            e.Bounds = new RectangleI(
                tl * multiplier,
                e.Layout.Size * multiplier
            );
        }
        internal void RemoveLayout(Element e)
        {
            _layouts.Remove(e);
        }
        
        private readonly List<Element> _layouts = new List<Element>();
        private void SizeChange(object s, SizeChangeEventArgs e)
        {
            _shader.Matrix3 = Matrix4.CreateOrthographic(e.Width, e.Height, 0d, 1d);
            
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
}