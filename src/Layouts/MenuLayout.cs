using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class MenuLayout : ILayout
    {
        public MenuLayout(double height)
        {
            _height = height;
        }

        private double _height;
        public double Height
        {
            get => _height;
            set
            {
                if (_height == value) { return; }

                _height = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Change;

        public Box GetBounds(LayoutArgs args)
        {
            double top = args.Size.Y * 0.5;

            return new Box(
                args.Size.X * -0.5,
                args.Size.X * 0.5,
                top,
                top - _height);
        }
    }
}
