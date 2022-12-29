using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public abstract class TextElement : Element
    {
        public TextElement()
            : base()
        {
        }
        public TextElement(ILayout layout)
            : base(layout)
        {
        }

        protected virtual string TextReference { get; set; } = "TEMP";
        public string Text
        {
            get => TextReference;
            set
            {
                TextReference = value;

                TriggerLayout();
            }
        }
        private int _charSpace = 0;
        public int CharSpace
        {
            get => _charSpace;
            set
            {
                _charSpace = value;

                TriggerLayout();
            }
        }
        private int _lineSpace = 0;
        public int LineSpace
        {
            get => _lineSpace;
            set
            {
                _lineSpace = value;

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
        private Font _font = SampleFont.GetInstance();
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
