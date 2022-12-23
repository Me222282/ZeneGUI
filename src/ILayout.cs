using Zene.Structs;

namespace Zene.GUI
{
    public interface ILayout
    {
        public Box GetBounds(Element element, Vector2 size);
    }
}
