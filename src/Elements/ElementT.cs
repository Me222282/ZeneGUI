namespace Zene.GUI
{
    /// <summary>
    /// An element with a specified <see cref="Layout"/> type.
    /// </summary>
    /// <typeparam name="T">The layout type constraint.</typeparam>
    public abstract class Element<T> : Element
        where T : class, ILayout
    {
        /// <summary>
        /// Creates an empty element.
        /// </summary>
        public Element()
        {

        }
        /// <summary>
        /// Creates an empty element with a layout.
        /// </summary>
        /// <param name="layout">THe layout for th element.</param>
        public Element(T layout)
            : base(layout)
        {

        }

        /// <summary>
        /// The layout of the element. Used to calculate the element's bounds.
        /// </summary>
        public new T Layout
        {
            get => base.Layout as T;
            set => base.Layout = value;
        }
    }
}
