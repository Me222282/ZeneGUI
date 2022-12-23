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

            Relative = relative;
        }
        public TextLayout(double extraWidth, double extraHeight, double x, double y, bool relative = true)
        {
            Padding = (extraWidth, extraHeight);
            Centre = (x, y);

            Relative = relative;
        }

        public Vector2 Padding { get; set; }
        public Vector2 Centre { get; set; }

        public bool Relative { get; set; }

        public Box GetBounds(TextElement element, Vector2 size)
        {
            if (element == null)
            {
                throw new ArgumentException($"Element must be of type {nameof(TextElement)}", nameof(element));
            }

            Vector2 textSize = element.Font.GetFrameSize(element.Text, 0, 0, 4);

            textSize /= element.Font.LineHeight;
            textSize *= element.TextSize;

            if (!Relative)
            {
                return new Box(Centre, textSize + Padding);
            }

            return new Box(Centre * size * 0.5, textSize + Padding);
        }

        public Box GetBounds(Element element, Vector2 size) => GetBounds(element as TextElement, size);
    }
}
