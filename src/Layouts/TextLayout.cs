using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class TextLayout : ILayout
    {
        public TextLayout(Vector2 padding, Vector2 centre, bool relative = true)
        {
            Padding = padding;
            Centre = centre;

            _relative = relative;
        }
        public TextLayout(double extraWidth, double extraHeight, double x, double y, bool relative = true)
        {
            Padding = (extraWidth, extraHeight);
            Centre = (x, y);

            _relative = relative;
        }

        public event EventHandler Change;

        private Vector2 _padding;
        public Vector2 Padding
        {
            get => _padding;
            set
            {
                if (_padding == value) { return; }
                
                _padding = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        private Vector2 _centre;
        public Vector2 Centre
        {
            get => _centre;
            set
            {
                if (_centre == value) { return; }
                
                _centre = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool _relative;

        public bool Relative
        {
            get => _relative;
            set
            {
                if (_relative == value) { return; }
                
                _relative = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }
        private bool _textInput;
        public bool TextInput
        {
            get => _textInput;
            set
            {
                if (_textInput == value) { return; }
                
                _textInput = value;
                Change?.Invoke(this, EventArgs.Empty);
            }
        }

        public Box GetBounds(TextElement element, Vector2 size)
        {
            if (element == null)
            {
                throw new ArgumentException($"Element must be of type {nameof(TextElement)}", nameof(element));
            }

            string reference = element.Text;

            if (reference == null || reference.Length == 0)
            {
                reference = "|";
            }

            Vector2 textSize = element.Font.GetFrameSize(reference, element.CharSpace, element.LineSpace, 4);

            if (TextInput)
            {
                textSize.X += element.Font.GetCharacterData('|').Size.X;
            }

            textSize /= element.Font.LineHeight;
            textSize *= element.TextSize;

            if (!Relative)
            {
                return new Box(Centre, textSize + Padding);
            }

            return new Box(Centre * size * 0.5, textSize + Padding);
        }

        public Box GetBounds(Element element, Vector2 size, int index, ReadOnlySpan<Element> neighbours)
            => GetBounds(element as TextElement, size);
    }
}
