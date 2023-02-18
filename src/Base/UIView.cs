using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public class UIView
    {
        public UIView(Viewport source)
        {
            _viewport = new ViewportLock(source.View);
            ScissorBox = source.View;
            _scissor = new ScissorLock(true);
        }

        private readonly ViewportLock _viewport;
        private readonly ScissorLock _scissor;

        private bool Locked
        {
            set
            {
                _viewport.Locked = value;
                _scissor.Locked = value;
            }
        }

        public bool Visable => _scissor.Width > 0 && _scissor.Height > 0;

        private Vector2 _hFrameSize;
        public Vector2 FrameSize
        {
            get => _hFrameSize * 2d;
            set => _hFrameSize = value * 0.5;
        }

        public Vector2 Offset { get; set; }
        public double Scale { get; set; }

        private Box _viewRef;
        public GLBox ScissorBox { get; set; }
        public Box View
        {
            get => _viewRef;
            set
            {
                Locked = false;

                _viewRef = value;

                Box b = new Box((value.Centre * Scale) + Offset, value.Size * Scale);

                _viewport.View = new GLBox(b.Left + _hFrameSize.X, b.Bottom + _hFrameSize.Y, b.Width, b.Height);
                GLBox c = _viewport.View.Clamp(ScissorBox);
                _scissor.View = c;

                Locked = true;
            }
        }

        public Vector2 Location
        {
            get => _viewRef.Location;
            set
            {
                _viewRef.Location = value;
                View = _viewRef;
            }
        }
        public double X
        {
            get => _viewRef.X;
            set
            {
                _viewRef.X = value;
                View = _viewRef;
            }
        }
        public double Y
        {
            get => _viewRef.Y;
            set
            {
                _viewRef.Y = value;
                View = _viewRef;
            }
        }

        public Vector2 Size
        {
            get => _viewRef.Size;
            set
            {
                _viewRef.Size = value;
                View = _viewRef;
            }
        }
        public double Width
        {
            get => _viewRef.Width;
            set
            {
                _viewRef.Width = value;
                View = _viewRef;
            }
        }
        public double Height
        {
            get => _viewRef.Height;
            set
            {
                _viewRef.Height = value;
                View = _viewRef;
            }
        }

        public GLBox PassScissor() => ScissorBox = _scissor.View;

        public static implicit operator Viewport(UIView uIView) => uIView._viewport;
        public static implicit operator Scissor(UIView uIView) => uIView._scissor;
    }
}
