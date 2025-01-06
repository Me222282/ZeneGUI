using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class MenuLayout : ILayout
    {
        public MenuLayout(floatv height)
        {
            _height = height;
        }

        private floatv _height;
        public floatv Height
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
            floatv top = args.Size.Y * 0.5f;

            return new Box(
                args.Size.X * -0.5f,
                args.Size.X * 0.5f,
                top,
                top - _height);
        }
    }
}
