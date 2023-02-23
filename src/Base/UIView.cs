using System;
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
            _depthState = new DepthStateLock()
            {
                Testing = true,
                Function = DepthFunction.LessEqual,
                Locked = true
            };

            Locked = true;
        }

        private readonly ViewportLock _viewport;
        private readonly ScissorLock _scissor;
        private readonly DepthStateLock _depthState;

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
        public Vector2 ScissorOffset { get; set; }
        public Box View
        {
            get => _viewRef;
            set
            {
                Locked = false;

                _viewRef = value;

                Box b = new Box((value.Centre * Scale) + Offset, value.Size * Scale);

                _viewport.View = new GLBox(b.Left + _hFrameSize.X, b.Bottom + _hFrameSize.Y, b.Width, b.Height);
                GLBox c = _viewport.View;
                c.Right -= (int)ScissorOffset.X;
                c.Bottom += (int)ScissorOffset.Y;
                c = c.Clamp(ScissorBox);
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

        public Vector2 DepthRange { get; set; } = new Vector2(0d, 1d);
        public int DepthDivision { get; set; }
        public int ChildDivision { get; set; }
        public void SetDepth(double depth)
        {
            Vector2 halfRange = GetRange(depth, DepthRange, DepthDivision);
            Vector2 mainRange = GetRange(0, halfRange, ChildDivision);
            DepthDivision = ChildDivision;

            _depthState.Locked = false;
            _depthState.Range = 1d - mainRange;
            _depthState.Locked = true;

            DepthRange = halfRange;
        }
        private static Vector2 GetRange(double depth, Vector2 range, double div)
        {
            depth = Math.Max(depth, 0d);

            double rangeV = range.Y - range.X;
            rangeV /= div;

            Vector2 nDRange = new Vector2(
                range.X + (rangeV * depth),
                range.X + (rangeV * (depth + 1d)));

            return new Vector2(
                Math.Max(nDRange.X, range.X),
                Math.Min(nDRange.Y, range.Y));
        }

        public GLBox PassScissor() => ScissorBox = _scissor.View;

        public static implicit operator Viewport(UIView uIView) => uIView._viewport;
        public static implicit operator Scissor(UIView uIView) => uIView._scissor;
        public static implicit operator DepthState(UIView uIView) => uIView._depthState;
    }
}
