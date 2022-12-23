using Zene.Structs;

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

        public Box GetBounds(Element element, Vector2 size)
        {
            Vector2 multiplier = size * 0.5;

            return new Box(
                Relative.Location * multiplier,
                Relative.Size * multiplier
            );
        }
    }
}
