using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public abstract class TextElement : Element<TextLayout>
    {
        public TextElement()
            : base(new TextLayout(3d, 0d, true))
        {
            CursorStyle = Cursor.IBeam;
        }
        public TextElement(TextLayout layout)
            : base(layout)
        {
            CursorStyle = Cursor.IBeam;
        }

        protected virtual string TextReference { get; set; } = "TEMP";
        public string Text
        {
            get => TextReference;
            set
            {
                TextReference = value;

                TriggerChange();
            }
        }
        private int _charSpace = 0;
        public int CharSpace
        {
            get => _charSpace;
            set
            {
                _charSpace = value;

                TriggerChange();
            }
        }
        private int _lineSpace = 0;
        public int LineSpace
        {
            get => _lineSpace;
            set
            {
                _lineSpace = value;

                TriggerChange();
            }
        }
        private double _textSize = 10d;
        public double TextSize
        {
            get => _textSize;
            set
            {
                _textSize = value;

                TriggerChange();
            }
        }
        public ColourF TextColour { get; set; } = new ColourF(1f, 1f, 1f);
        private Font _font = Shapes.SampleFont;
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;

                TriggerChange();
            }
        }
    }
}
