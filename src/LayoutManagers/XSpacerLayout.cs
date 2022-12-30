using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class XSpacerLayout : ILayoutManager
    {
        public event EventHandler Change;

        private double _multipler;
        private double _offset;
        public void SetupManager(LayoutArgs args)
        {
            _multipler = args.Size.X / args.Neighbours.Length;
            _offset = args.Size.X * -0.5;
        }

        public Box GetBounds(LayoutArgs args, Box layoutResult)
        {
            layoutResult.Location = (_offset + (_multipler * (args.Index + 1)), 0d);

            return layoutResult;
        }
    }
}
