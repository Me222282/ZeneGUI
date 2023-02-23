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
            Graphics = new BoxColour(this);
            Properties.Interactable = false;
            Properties.Selectable = false;
            Properties.ScrollBar = new ScrollBar();
        }

        public Container(ILayout layout)
            : base(layout)
        {
            Graphics = new BoxColour(this);
            Properties.Interactable = false;
            Properties.Selectable = false;
            Properties.ScrollBar = new ScrollBar();
        }

        public ColourF Colour
        {
            get => Graphics.Colour;
            set => Graphics.Colour = value;
        }

        public override BoxColour Graphics { get; }
    }
}
