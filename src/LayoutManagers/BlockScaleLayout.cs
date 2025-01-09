using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class BlockScaleLayout : LayoutManagerI<BlockScaleLayout.Instance>
    {
        public class Instance : ILayoutManagerInstance
        {
            public floatv _current;
            private floatv _height;
            
            public void Height(floatv h)
            {
                if (_height >= h) { return; }
                _height = h;
            }
            
            public Vector2 ReturningSize => (_current, _height);
            public Vector2 ChildOffset => (ReturningSize.X * -0.5f, 0);
        }

        public BlockScaleLayout()
            : base(true, true)
        {

        }

        public override bool ChildDependent => true;

        public BlockScaleLayout(Vector4 margin)
            : base(true, true)
        {
            _margin = margin;
        }
        public BlockScaleLayout(Vector2 margin)
            : base(true, true)
        {
            _margin = (margin, margin);
        }
        public BlockScaleLayout(floatv margin)
            : base(true, true)
        {
            _margin = (margin, margin, margin, margin);
        }
        public BlockScaleLayout(floatv marginX, floatv marginY)
            : base(true, true)
        {
            _margin = (marginX, marginY, marginX, marginY);
        }
        public BlockScaleLayout(floatv left, floatv right, floatv top, floatv bottom)
            : base(true, true)
        {
            _margin = (left, top, right, bottom);
        }

        private Vector4 _margin;
        /// <summary>
        /// Left - <see cref="Vector4.X"/>, Top - <see cref="Vector4.Y"/>,
        /// Right - <see cref="Vector4.Z"/>, Bottom - <see cref="Vector4.W"/>
        /// </summary>
        public Vector4 Margin
        {
            get => _margin;
            set
            {
                if (_margin == value) { return; }

                _margin = value;
                InvokeChange();
            }
        }

        /// <summary>
        /// The margin on the left side.
        /// </summary>
        public floatv Left
        {
            get => _margin.X;
            set
            {
                if (_margin.X == value) { return; }

                _margin.X = value;
                InvokeChange();
            }
        }
        /// <summary>
        /// The margin on the right side.
        /// </summary>
        public floatv Right
        {
            get => _margin.Z;
            set
            {
                if (_margin.Z == value) { return; }

                _margin.Z = value;
                InvokeChange();
            }
        }
        /// <summary>
        /// The margin on the top side.
        /// </summary>
        public floatv Top
        {
            get => _margin.Y;
            set
            {
                if (_margin.Y == value) { return; }

                _margin.Y = value;
                InvokeChange();
            }
        }
        /// <summary>
        /// The margin on the bottom side.
        /// </summary>
        public floatv Bottom
        {
            get => _margin.W;
            set
            {
                if (_margin.W == value) { return; }

                _margin.W = value;
                InvokeChange();
            }
        }

        public override ILayoutManagerInstance Init(LayoutArgs args)
            => new Instance();

        protected override Box GetBounds(LayoutArgs args, Box layoutResult, Instance instance)
        {
            Vector2 size = layoutResult.Size;
            
            instance._current += _margin.X;
            floatv x = instance._current;
            instance._current += size.X + _margin.Z;

            layoutResult.Location = 0;
            layoutResult += (x + (size.X * 0.5f), 0);
            instance.Height(size.Y + _margin.Y + _margin.W);

            return layoutResult;
        }
    }
}
