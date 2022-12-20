using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public abstract class TextElement : Element
    {
        public TextElement(IBox bounds, bool framebuffer = true)
            : base(bounds, framebuffer)
        {
        }

        public TextElement(ILayout layout, bool framebuffer = true)
            : base(layout, framebuffer)
        {
        }

        public string Text { get; set; } = "TEMP";
        public double TextSize { get; set; } = 10d;
        public ColourF TextColour { get; set; } = new ColourF(1f, 1f, 1f);
        public Font Font { get; set; }
    }
}
