using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class MultiLayout : ILayout
    {
        public MultiLayout(ILayout location, ILayout size)
        {
            Location = location;
            Size = size;
        }

        private ILayout _location;
        /// <summary>
        /// The layout used to calculate location.
        /// </summary>
        public ILayout Location
        {
            get => _location;
            set
            {
                if (_location == value) { return; }

                if (_location != null)
                {
                    _location.Change -= ChangeInvoke;
                }

                _location = value;

                if (_location == null) { return; }
                _location.Change += ChangeInvoke;
            }
        }

        private ILayout _size;
        /// <summary>
        /// The layout used to calculate size.
        /// </summary>
        public ILayout Size
        {
            get => _size;
            set
            {
                if (_size == value) { return; }

                if (_size != null)
                {
                    _size.Change -= ChangeInvoke;
                }

                _size = value;

                if (_size == value) { return; }
                _size.Change += ChangeInvoke;
            }
        }

        public event EventHandler Change;

        private void ChangeInvoke(object sender, EventArgs e) => Change?.Invoke(sender, e);

        public Box GetBounds(LayoutArgs args)
        {
            Box location = default;
            if (_location != null)
            {
                location = _location.GetBounds(args);
            }

            Box size = default;
            if (_size != null)
            {
                size = _size.GetBounds(args);
            }

            return new Box(location.Location, size.Size);
        }
    }
}
