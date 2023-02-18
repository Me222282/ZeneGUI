using Zene.Structs;

namespace Zene.GUI
{
    /// <summary>
    /// An empty element for grouping together other UI elements.
    /// </summary>
    public class Container : ParentElement
    {
        public Container()
        {
            _g = new BoxColour(this);
        }

        public Container(ILayout layout)
            : base(layout)
        {
            _g = new BoxColour(this);
        }

        public ColourF Colour
        {
            get => _g.Colour;
            set => _g.Colour = value;
        }

        private readonly BoxColour _g;
        public override GraphicsManager Graphics => _g;
    }
}
