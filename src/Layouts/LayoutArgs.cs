using System;
using System.Collections.Generic;
using Zene.Structs;

namespace Zene.GUI
{
    /// <summary>
    /// Arguments for <see cref="ILayout"/> and <see cref="ILayoutManager"/> calls.
    /// </summary>
    public class LayoutArgs
    {
        public LayoutArgs(Element e, Vector2 s, List<Element> n)
        {
            if (n == null)
            {
                throw new ArgumentNullException($"{nameof(n)}");
            }

            Element = e ?? throw new ArgumentNullException($"{nameof(e)}");

            Size = s;
            Index = e.CurrentIndex;
            Neighbours = new ElementReference(n);
        }
        public LayoutArgs(Element e, Vector2 s, ElementReference n)
        {
            Element = e ?? throw new ArgumentNullException($"{nameof(e)}");

            Size = s;
            Index = e.CurrentIndex;
            Neighbours = n;
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

        /// <summary>
        /// Gets the neighbouring element next in the list.
        /// </summary>
        /// <returns></returns>
        public Element NextElement()
        {
            // Last element
            if ((Index + 1) == Neighbours.Length)
            {
                return null;
            }

            return Neighbours[Index + 1];
        }
        /// <summary>
        /// Gets the neighbouring element proceding in the list.
        /// </summary>
        /// <returns></returns>
        public Element PreviousElement()
        {
            // First element
            if (Index == 0) { return null; }

            return Neighbours[Index - 1];
        }
    }
}
