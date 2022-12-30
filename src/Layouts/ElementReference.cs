using System.Collections;
using System.Collections.Generic;

namespace Zene.GUI
{
    public unsafe struct ElementReference : IEnumerable<Element>
    {
        public ElementReference(List<Element> list)
        {
            _elements = list;
        }

        public int Length => _elements.Count;
        private readonly List<Element> _elements;

        public Element this[int index] => _elements[index];

        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
        public IEnumerator<Element> GetEnumerator() => _elements.GetEnumerator();
    }
}
