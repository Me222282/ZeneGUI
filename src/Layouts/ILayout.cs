using System;
using Zene.Structs;

namespace Zene.GUI
{
    public interface ILayout
    {
        public event EventHandler Change;

        public Box GetBounds(LayoutArgs args);
    }
}
