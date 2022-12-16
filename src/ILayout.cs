using Zene.Structs;

namespace Zene.GUI
{
    public interface ILayout
    {
        public RectangleI GetBounds(Element element, Vector2I size);
    }
}
