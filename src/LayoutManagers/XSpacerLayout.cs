using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class XSpacerLayout : LayoutManager
    {
        private double _multipler;
        private double _offset;
        protected override void SetupManager(LayoutArgs args)
        {
            _multipler = args.Size.X / args.Neighbours.Length;
            _offset = args.Size.X * -0.5;
        }

        protected override Box GetBounds(LayoutArgs args, Box layoutResult)
        {
            layoutResult.Location = (_offset + (_multipler * (args.Index + 0.5)), 0d);

            return layoutResult;
        }
    }
}
