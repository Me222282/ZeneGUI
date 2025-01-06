using System;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class EventListener
    {
        public EventListener(IElement source)
        {
            this.source = source ?? throw new ArgumentNullException(nameof(source));
        }

        protected readonly IElement source;

        protected internal virtual void OnTextInput(TextInputEventArgs e) { }
        protected internal virtual void OnKeyDown(KeyEventArgs e) { }
        protected internal virtual void OnKeyUp(KeyEventArgs e) { }

        protected internal virtual void OnScroll(ScrollEventArgs e) { }

        protected internal virtual void OnMouseEnter(EventArgs e) => source.Properties.hover = true;
        protected internal virtual void OnMouseLeave(EventArgs e) => source.Properties.hover = false;
        protected internal virtual void OnMouseMove(MouseEventArgs e) => source.Properties.mousePos = e.Location;
        protected internal virtual void OnMouseDown(MouseEventArgs e) => source.Properties.selected = true;
        protected internal virtual void OnMouseUp(MouseEventArgs e)
        {
            if (source.Properties.Window.MouseButton == MouseButton.None)
            {
                source.Properties.selected = false;
            }
        }

        protected internal virtual void OnSizeChange(VectorEventArgs e)
        {
            source.Properties.bounds.Size = e.Value;

            source.Graphics.ChangeSize(e);
        }
        protected internal virtual void OnElementMove(VectorEventArgs e)
        {
            source.Properties.bounds.Centre = e.Value;

            source.Graphics.MoveElement(e);
        }

        protected internal virtual void OnUpdate()
        {
            /*
            ScrollBarHover sbh = source.Properties.scrollBarHover;
            if (sbh != ScrollBarHover.None && source.Properties.selected)
            {
                if (sbh == ScrollBarHover.XAxis)
                {
                    floatv lerpMX = -source.Properties.scrollBounds.Right;
                    floatv lerpIX = -source.Properties.scrollBounds.Left;

                    ScrollBarData sbdX = new ScrollBarData(source,
                        source.Properties.ViewPan.X.InvLerp(lerpMX, lerpIX),
                        1d, false, source.GetRenderSize().X);
                    source.Properties.ScrollBar.ScrollBarPosition(ref sbdX, source.Properties.MouseLocation);
                    source.Properties.ViewPan = (lerpMX.Lerp(lerpIX, Math.Clamp(sbdX.ScrollPercent, 0d, 1d)), source.Properties.ViewPan.Y);
                    return;
                }

                floatv lerpM = -source.Properties.scrollBounds.Top;
                floatv lerpI = -source.Properties.scrollBounds.Bottom;

                ScrollBarData sbd = new ScrollBarData(source,
                    source.Properties.ViewPan.Y.InvLerp(lerpM, lerpI),
                    1d, true, source.GetRenderSize().Y);
                source.Properties.ScrollBar.ScrollBarPosition(ref sbd, source.Properties.MouseLocation);
                source.Properties.ViewPan = (source.Properties.ViewPan.X, lerpM.Lerp(lerpI, Math.Clamp(sbd.ScrollPercent, 0d, 1d)));
                return;
            }*/
        }
        protected internal virtual void OnFocus(FocusedEventArgs e) => source.Properties.focus = e.Focus;
    }
}
