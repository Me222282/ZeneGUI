using System;
using Zene.Structs;

namespace Zene.GUI
{
    public sealed class XSpacerLayout : LayoutManagerI<XSpacerLayout.Instance>
    {
        public class Instance : ILayoutManagerInstance
        {
            public floatv _multipler;
            public floatv _offset;

            public Vector2 ReturningSize { get; init; }
            public Vector2 ChildOffset => 0;
        }

        public XSpacerLayout()
            : base(true, false)
        {

        }

        public override bool ChildDependent => true;

        public override ILayoutManagerInstance Init(LayoutArgs args)
        {
            return new Instance()
            {
                ReturningSize = args.Size,
                _multipler = args.Size.X / args.Neighbours.Length,
                _offset = args.Size.X * -0.5f
            };
        }

        protected override Box GetBounds(LayoutArgs args, Box layoutResult, Instance instance)
        {
            layoutResult.Location = (instance._offset + (instance._multipler * (args.Index + 0.5f)), 0);

            return layoutResult;
        }
    }
}
