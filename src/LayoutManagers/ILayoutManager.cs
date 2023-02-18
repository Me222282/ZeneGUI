using System;
using Zene.Structs;

namespace Zene.GUI
{
    public interface ILayoutManager
    {
        /// <summary>
        /// Determines whether the children's layouts are dependent upon each other.
        /// </summary>
        public bool ChildDependent { get; }
        /// <summary>
        /// Determines whether the parent's layout is dependent on its children.
        /// </summary>
        public bool SizeDependent { get; }

        public ILayoutManagerInstance Init(LayoutArgs args);
        public Box GetBounds(LayoutArgs args, Box bounds, ILayoutManagerInstance instance);
    }

    public interface ILayoutManagerInstance
    {
        public Vector2 ReturningSize { get; }
    }
}
