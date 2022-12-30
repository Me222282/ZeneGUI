using System;
using Zene.Structs;

namespace Zene.GUI.LayoutManagers
{
    public class BlockLayout : ILayoutManager
    {
        public BlockLayout(Vector4 margin)
        {
            _margin = margin;
        }
        public BlockLayout(Vector2 margin)
        {
            _margin = (margin, margin);
        }
        public BlockLayout(double margin)
        {
            _margin = (margin, margin, margin, margin);
        }
        public BlockLayout(double marginX, double marginY)
        {
            _margin = (marginX, marginY, marginX, marginY);
        }
        public BlockLayout(double left, double right, double top, double bottom)
        {
            _margin = (left, top, right, bottom);
        }

        private Vector4 _margin;
        /// <summary>
        /// Left - <see cref="Vector4.X"/>, Top - <see cref="Vector4.Y"/>,
        /// Right - <see cref="Vector4.Z"/>, Bottom - <see cref="Vector4.W"/>
        /// </summary>
        public Vector4 Margin
        {
            get => _margin;
            set
            {
                if (_margin == value) { return; }

                _margin = value;
                InvokeChange();
            }
        }

        public event EventHandler Change;

        private void InvokeChange() => Change?.Invoke(this, EventArgs.Empty);

        private double _left;
        private double _right;
        private double _lowestY;
        private Vector2 _current;
        public void SetupManager(LayoutArgs args)
        {
            _right = args.Size.X * 0.5;
            _left = -_right;
            _lowestY = args.Size.Y * 0.5;

            _current = (_left, _lowestY - _margin.Y);
        }

        private void SetLowest(double value)
        {
            if (_lowestY > value)
            {
                _lowestY = value;
            }
        }

        public Box GetBounds(LayoutArgs args, Box layoutResult)
        {
            Vector2 size = layoutResult.Size;

            bool onLeft = _current.X == _left;

            _current.X += _margin.X;
            Vector2 topLeft = _current;
            _current.X += size.X + _margin.Z;

            if (!onLeft && _current.X > _right)
            {
                _current.Y = _lowestY - _margin.Y;

                topLeft = (_left + _margin.X, _current.Y);

                _current.X = topLeft.X + size.X + _margin.Z;
            }

            layoutResult.SetTopLeft(topLeft);
            SetLowest(_current.Y - size.Y - _margin.W);

            return layoutResult;
        }
    }
}
