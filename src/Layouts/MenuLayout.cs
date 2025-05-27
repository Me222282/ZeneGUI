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
            floatv y = (args.Size.Y - _height) * 0.5f;

            return new Box(
                0,
                y,
                args.Size.X,
                _height);
        }
    }
}
