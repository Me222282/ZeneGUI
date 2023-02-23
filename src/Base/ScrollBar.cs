using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class ScrollBar : IRenderable<ScrollBarData>
    {
        public double Width { get; set; } = 10d;
        public double ScrollSpeed { get; set; } = 20d;

        protected internal void OnMouseMove(MouseEventArgs e)
        {

        }
        protected internal void OnMouseDown(MouseEventArgs e)
        {

        }
        protected internal void OnMouseUp(MouseEventArgs e)
        {

        }
        protected internal void OnKeyDown(MouseEventArgs e)
        {

        }
        protected internal void OnKeyUp(MouseEventArgs e)
        {

        }

        public virtual void OnRender(IDrawingContext context, ScrollBarData param)
        {
            context.Framebuffer.Clear(Colour.DarkSalmon);

            Box barBounds;
            double range = param.Size - (param.Size * param.BarPercent);

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

            context.DrawRoundedBox(barBounds, ColourF.Azure, 0.5);
        }
    }

    public struct ScrollBarData
    {
        public ScrollBarData(IElement e, double scroll, double bar, bool latitude, double size)
        {
            Parent = e;
            ScrollPercent = scroll;
            BarPercent = bar;
            Latitude = latitude;
            Size = size;
        }

        public IElement Parent { get; }
        public double ScrollPercent { get; }
        public double BarPercent { get; }
        public bool Latitude { get; }
        public double Size { get; }
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
