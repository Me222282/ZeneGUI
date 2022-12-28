using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class TextLayout : ILayout
    {
        public TextLayout(Vector2 padding, Vector2 centre, bool relative = true)
        {
            _padding = padding;
            _centre = centre;

            _relative = relative;
        }
        public TextLayout(Vector2 padding, Vector2 centre, Vector2 minSize, bool relative = true)
        {
            _padding = padding;
            _minSize = minSize;
            _centre = centre;

            _relative = relative;
        }
        public TextLayout(double extraWidth, double extraHeight, double x, double y, bool relative = true)
        {
            _padding = (extraWidth, extraHeight);
            _centre = (x, y);

            _relative = relative;
        }
        public TextLayout(double extraWidth, double extraHeight, double x, double y, double minX, double minY, bool relative = true)
        {
            _padding = (extraWidth, extraHeight);
            _minSize = (minX, minY);
            _centre = (x, y);

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
        private Vector2 _minSize;
        public Vector2 MinSize
        {
            get => _minSize;
            set
            {
                if (_minSize == value) { return; }

                _minSize = value;
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
            textSize += Padding;

            textSize = new Vector2(
                Math.Max(textSize.X, _minSize.X),
                Math.Max(textSize.Y, _minSize.Y));

            if (!Relative)
            {
                return new Box(Centre, textSize);
            }

            return new Box(Centre * size * 0.5, textSize);
        }
        public Box GetBounds(LayoutArgs args)
            => GetBounds(args.Element as TextElement, args.Size);
    }
}
