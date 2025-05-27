using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class Layout : ILayout
    {
        public Layout(Box box)
        {
            _relative = box;
        }
        public Layout(floatv x, floatv y, floatv width, floatv height)
        {
            _relative = new Box(x, y, width, height);
        }

        private Box _relative;
        public Box Relative
        {
            get => _relative;
            set
            {
                if (_relative == value) { return; }
                
                _relative = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Change;

        public Box GetBounds(LayoutArgs args)
        {
            Vector2 multiplier = args.Size * 0.5f;

            return new Box(
                _relative.Location * multiplier,
                _relative.Size * multiplier
            );
        }
    }
}
