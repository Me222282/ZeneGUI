using Zene.Windowing;

namespace Zene.GUI
{
    public interface IElementManager
    {
        public Window Handle { get; }

        public Element this[int index] { get; }

        public void AddChild(Element e);
        public bool RemoveChild(Element e);
        public void ClearChildren();
    }
}
