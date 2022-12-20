using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class TextLayout : ILayout
    {
        public TextLayout(Vector2I padding, Vector2I centre)
        {
            Padding = padding;
            Centre = centre;

            Relative = false;
        }
        public TextLayout(int extraWidth, int extraHeight, int x, int y)
        {
            Padding = (extraWidth, extraHeight);
            Centre = (x, y);

            Relative = false;
        }
        public TextLayout(Vector2I padding, Vector2 centre, bool relative = true)
        {
            Padding = padding;
            Centre = centre;

            Relative = relative;
        }
        public TextLayout(int extraWidth, int extraHeight, double x, double y, bool relative = true)
        {
            Padding = (extraWidth, extraHeight);
            Centre = (x, y);

            Relative = relative;
        }

        public Vector2I Padding { get; set; }
        public Vector2 Centre { get; set; }

        public bool Relative { get; set; }

        public RectangleI GetBounds(TextElement element, Vector2I size)
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
                return new RectangleI(new Box(Centre, textSize + Padding));
            }

            return new RectangleI(new Box(Centre * size * 0.5, textSize + Padding));
        }

        public RectangleI GetBounds(Element element, Vector2I size) => GetBounds(element as TextElement, size);
    }
}
