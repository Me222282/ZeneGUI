using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class FixedLayout : ILayout
    {
        public FixedLayout(Box bounds)
        {
            _bounds = bounds;
        }

        public FixedLayout(floatv x, floatv y, floatv w, floatv h)
        {
            _bounds = new Box(x, y, w, h);
        }
        public FixedLayout(Vector2 location, Vector2 size)
        {
            _bounds = new Box(location, size);
        }

        private Box _bounds;
        public Box Bounds
        {
            get => _bounds;
            set
            {
                if (_bounds == value) { return; }

                _bounds = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public floatv Left
        {
            get => _bounds.Left;
            set
            {
                if (_bounds.Left == value) { return; }
                
                _bounds.Left = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        public floatv Right
        {
            get => _bounds.Right;
            set
            {
                if (_bounds.Right == value) { return; }
                
                _bounds.Right = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        public floatv Bottom
        {
            get => _bounds.Bottom;
            set
            {
                if (_bounds.Bottom == value) { return; }
                
                _bounds.Bottom = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        public floatv Top
        {
            get => _bounds.Top;
            set
            {
                if (_bounds.Top == value) { return; }
                
                _bounds.Top = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// The center location of the box.
        /// </summary>
        public Vector2 Location
        {
            get => _bounds.Location;
            set
            {
                if (_bounds.Location == value) { return; }
                
                _bounds.Location = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// The width and height of the box.
        /// </summary>
        public Vector2 Size
        {
            get => _bounds.Size;
            set
            {
                if (_bounds.Size == value) { return; }

                _bounds.Size = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public Vector2 Centre
        {
            get => _bounds.Location;
            set
            {
                if (_bounds.Location == value) { return; }

                _bounds.Location = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public floatv X
        {
            get => _bounds.X;
            set
            {
                if (_bounds.X == value) { return; }

                _bounds.X = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        public floatv Y
        {
            get => _bounds.Y;
            set
            {
                if (_bounds.Y == value) { return; }

                _bounds.Y = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        public floatv Width
        {
            get => _bounds.Width;
            set
            {
                if (_bounds.Width == value) { return; }

                _bounds.Width = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        public floatv Height
        {
            get => _bounds.Height;
            set
            {
                if (_bounds.Height == value) { return; }

                _bounds.Height = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Change;

        public Box GetBounds(LayoutArgs args)
            => _bounds;

        public static readonly FixedLayout Default = new FixedLayout(0, 0, 1, 1);

        public static implicit operator FixedLayout(Box bounds) => new FixedLayout(bounds);
        public static implicit operator FixedLayout(Rectangle bounds) => new FixedLayout(bounds);
    }
}
