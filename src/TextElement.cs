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

        private string _text = "TEMP";
        public string Text
        {
            get => _text;
            set
            {
                _text = value;

                TriggerLayout();
            }
        }
        private double _textSize = 10d;
        public double TextSize
        {
            get => _textSize;
            set
            {
                _textSize = value;

                TriggerLayout();
            }
        }
        public ColourF TextColour { get; set; } = new ColourF(1f, 1f, 1f);
        private Font _font;
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;

                TriggerLayout();
            }
        }
    }
}
