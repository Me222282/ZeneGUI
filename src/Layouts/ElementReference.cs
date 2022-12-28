using System.Collections.Generic;

namespace Zene.GUI
{
    public unsafe struct ElementReference
    {
        public ElementReference(List<Element> list)
        {
            _elements = list;
        }

        public int Length => _elements.Count;
        private readonly List<Element> _elements;

        public Element this[int index] => _elements[index];
    }
}
