using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class BlockLayout : LayoutManager
    {
        public BlockLayout()
        {

        }

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

        /// <summary>
        /// The margin on the left side.
        /// </summary>
        public double Left
        {
            get => _margin.X;
            set
            {
                if (_margin.X == value) { return; }

                _margin.X = value;
                InvokeChange();
            }
        }
        /// <summary>
        /// The margin on the right side.
        /// </summary>
        public double Right
        {
            get => _margin.Z;
            set
            {
                if (_margin.Z == value) { return; }

                _margin.Z = value;
                InvokeChange();
            }
        }
        /// <summary>
        /// The margin on the top side.
        /// </summary>
        public double Top
        {
            get => _margin.Y;
            set
            {
                if (_margin.Y == value) { return; }

                _margin.Y = value;
                InvokeChange();
            }
        }
        /// <summary>
        /// The margin on the bottom side.
        /// </summary>
        public double Bottom
        {
            get => _margin.W;
            set
            {
                if (_margin.W == value) { return; }

                _margin.W = value;
                InvokeChange();
            }
        }

        private double _left;
        private double _right;
        private double _lowestY;
        private Vector2 _current;
        protected override void SetupManager(LayoutArgs args)
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

        protected override Box GetBounds(LayoutArgs args, Box layoutResult)
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
