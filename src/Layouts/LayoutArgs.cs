using System.Collections.Generic;
using Zene.Structs;

namespace Zene.GUI
{
    public class LayoutArgs
    {
        public LayoutArgs(Element e, Vector2 s, int i, List<Element> n)
        {
            Element = e;
            Size = s;
            Index = i;
            Neighbours = new ElementReference(n);
        }

        /// <summary>
        /// The element being processed.
        /// </summary>
        public Element Element { get; }
        /// <summary>
        /// The bounding size of the parent element.
        /// </summary>
        public Vector2 Size { get; }
        /// <summary>
        /// The index in <see cref="Neighbours"/> of the xurrent element.
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// A reference to the neighbouring elements.
        /// </summary>
        public ElementReference Neighbours { get; }
    }
}
