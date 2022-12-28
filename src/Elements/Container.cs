﻿using Zene.Structs;

namespace Zene.GUI
{
    /// <summary>
    /// An empty element for grouping together other UI elements.
    /// </summary>
    public class Container : Element
    {
        public Container()
        {
        }

        public Container(ILayout layout)
            : base(layout)
        {
        }
    }
}