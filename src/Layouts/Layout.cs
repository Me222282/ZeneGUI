using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class Layout : ILayout
    {
        public Layout(IBox box)
        {
            _relative = new Box(box);
        }
        public Layout(double x, double y, double width, double height)
        {
            _relative = new Box((x, y), (width, height));
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
            Vector2 multiplier = args.Size * 0.5;

            return new Box(
                _relative.Location * multiplier,
                _relative.Size * multiplier
            );
        }
    }
}
