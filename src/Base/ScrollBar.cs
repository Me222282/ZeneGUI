using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class ScrollBar : IRenderable<ScrollBarData>
    {
        public floatv Width { get; set; } = 10;
        public floatv ScrollSpeed { get; set; } = 20;

        public virtual floatv GetScrollPercentage(floatv size, Vector2 mousePos, bool latitude)
        {
            floatv mouse = latitude ? mousePos.Y : mousePos.X;

            return 0.5f - (mouse / (size - 4));
        }
        public virtual void OnClick(IElement parent, Vector2 size, Vector2 mousePos)
        {
            
        }

        public virtual void OnRender(IDrawingContext context, ScrollBarData param)
        {
            context.Framebuffer.Clear(Colour.DarkSalmon);

            Box barBounds;
            floatv range = param.Size - (param.Size * param.BarPercent);

            if (param.Latitude)
            {
                barBounds = new Box(
                    (0d, range * -(param.ScrollPercent - 0.5)),
                    (Width - 4d, param.Size * param.BarPercent - 4d));
            }
            else
            {
                barBounds = new Box(
                    (range * -(param.ScrollPercent - 0.5), 0d),
                    (param.Size * param.BarPercent - 4d, Width - 4d));
            }

            context.DrawRoundedBox(barBounds, ColourF.Azure, 0.5f);
        }
    }

    public struct ScrollBarData
    {
        public ScrollBarData(IElement e, floatv scroll, floatv bar, bool latitude, floatv size)
        {
            Parent = e;
            ScrollPercent = scroll;
            BarPercent = bar;
            Latitude = latitude;
            Size = size;
        }

        public IElement Parent { get; }
        public floatv ScrollPercent { get; set; }
        public floatv BarPercent { get; set; }
        public bool Latitude { get; }
        public floatv Size { get; }
    }

    public struct ScrollInfo
    {
        public ScrollInfo(bool x, bool y, Vector2 viewSize, Vector2 scrollView)
        {
            X = x;
            Y = y;
            ViewSize = viewSize;
            ScrollView = scrollView;
        }

        public bool X { get; }
        public bool Y { get; }
        public bool CanScroll => X || Y;
        public Vector2 ViewSize { get; }
        public Vector2 ScrollView { get; }
    }
}
