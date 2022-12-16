﻿using Zene.Structs;

namespace Zene.GUI
{
    public class Layout : ILayout
    {
        public Layout(IBox box)
        {
            Relative = new Box(box);
        }
        public Layout(double x, double y, double width, double height)
        {
            Relative = new Box((x, y), (width, height));
        }

        public Box Relative { get; set; }

        public RectangleI GetBounds(Element element, Vector2I size)
        {
            Vector2 multiplier = size * 0.5;
            Vector2 tl = (Relative.Left, Relative.Top);

            return new RectangleI(
                tl * multiplier,
                Relative.Size * multiplier
            );
        }
    }
}
