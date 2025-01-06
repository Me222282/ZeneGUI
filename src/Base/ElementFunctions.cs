using System;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public static class ElementFunctions
    {
        public static Vector2 CalculateLocalMouse(this IElement e, Vector2 rootLocation)
        {
            if (!e.HasParent)
            {
                return rootLocation;
            }

            return (CalculateLocalMouse(e.Properties.Parent, rootLocation) - (e.Bounds.Centre + e.Properties.ViewPan)) / e.Properties.ViewScale;
        }
        public static Vector2 CalculateLocalMouse(this IElement e, IElement root, Vector2 rootLocation)
        {
            if (e == root)
            {
                return rootLocation;
            }

            return (CalculateLocalMouse(e.Properties.Parent, root, rootLocation) - (e.Bounds.Centre + e.Properties.ViewPan)) / e.Properties.ViewScale;
        }
        public static bool IsVisable(this IElement e)
        {
            if (!e.HasParent)
            {
                return e.Properties.Visable;
            }

            return e.Properties.Visable && IsVisable(e.Properties.Parent);
        }

        public static Vector2 GetRenderSize(this IElement e) => e.Graphics == null ? e.Properties.bounds.Size : e.Graphics.Bounds.Size;
        public static Box GetRenderBounds(this IElement e) => e.Graphics == null ? e.Properties.bounds : e.Graphics.Bounds;

        public static IElement LowestFirstElement(this IElement e)
        {
            if (!e.HasChildren) { return e; }

            IElement lfe = null;

            int length = e.Children.Length;
            for (int i = 0; i < length; i++)
            {
                lfe = LowestFirstElement(e.Children[i]);

                if (lfe.Properties.Selectable) { break; }
            }

            return lfe;
        }
        public static IElement LowestLastElement(this IElement e)
        {
            if (!e.HasChildren) { return e; }

            IElement lfe = null;

            int length = e.Children.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                lfe = LowestFirstElement(e.Children[i]);

                if (lfe.Properties.Selectable) { break; }
            }

            return lfe;
        }
        public static IElement NextElement(this IElement e)
        {
            if (!e.HasParent) { return null; }

            IElement parent = e.Properties.parent;
            IElement next = null;

            // This is last element
            if (e.Properties.elementIndex == (parent.Children.Length - 1))
            {
                next = NextElement(parent);

                // Loop round to first element
                if (next == null)
                {
                    next = e.Properties.RootElement;
                }

                return next?.LowestFirstElement();
            }

            int length = parent.Children.Length;
            for (int i = e.Properties.elementIndex + 1; i < length; i++)
            {
                next = parent.Children[i].LowestFirstElement();

                if (next.Properties.Selectable) { break; }
                
                // last element
                if (i == length - 1)
                {
                    return NextElement(next);
                }
            }

            return next;
        }
        public static IElement PreviousElement(this IElement e)
        {
            if (!e.HasParent) { return null; }

            IElement parent = e.Properties.parent;
            IElement next = null;

            // This is fisrt element
            if (e.Properties.elementIndex == 0)
            {
                next = PreviousElement(parent);

                // Loop round to first element
                if (next == null)
                {
                    next = e.Properties.RootElement;
                }

                return next?.LowestLastElement();
            }

            for (int i = e.Properties.elementIndex - 1; i >= 0; i--)
            {
                next = parent.Children[i].LowestLastElement();

                if (next.Properties.Selectable) { break; }

                // first element
                if (i == 0)
                {
                    return PreviousElement(next);
                }
            }

            return next;
        }

        internal static void OnTextInput(this IElement element, TextInputEventArgs e) => element.Events.OnTextInput(e);
        internal static void OnKeyDown(this IElement element, KeyEventArgs e) => element.Events.OnKeyDown(e);
        internal static void OnKeyUp(this IElement element, KeyEventArgs e) => element.Events.OnKeyUp(e);

        internal static void OnScroll(this IElement element, ScrollEventArgs e) => element.Events.OnScroll(e);

        internal static void OnMouseEnter(this IElement element) => element.Events.OnMouseEnter(EventArgs.Empty);
        internal static void OnMouseLeave(this IElement element) => element.Events.OnMouseLeave(EventArgs.Empty);
        internal static void OnMouseMove(this IElement element, MouseEventArgs e) => element.Events.OnMouseMove(e);
        internal static void OnMouseDown(this IElement element, MouseEventArgs e) => element.Events.OnMouseDown(e);
        internal static void OnMouseUp(this IElement element, MouseEventArgs e) => element.Events.OnMouseUp(e);

        internal static void OnSizeChange(this IElement element, VectorEventArgs e) => element.Events.OnSizeChange(e);
        internal static void OnElementMove(this IElement element, VectorEventArgs e) => element.Events.OnElementMove(e);

        internal static void OnFocus(this IElement element, bool focused) => element.Events.OnFocus(new FocusedEventArgs(focused));

        internal static void ViewBoxChange(this IElement e, Box oldBounds)
            => e.Properties.parent.Properties.PushViewBox(e.GetRenderBounds(), oldBounds);

        internal static ScrollBarHover InScrollBar(this IElement e, Vector2 mousePos)
        {
            if (e.Properties.ScrollBar == null || !(e.Properties.scrollY || e.Properties.scrollX)) { return ScrollBarHover.None; }
            Box bounds = e.GetRenderBounds();
            floatv width = e.Properties.ScrollBar.Width;

            if (e.Properties.scrollY && !e.Properties.scrollX)
            {
                return (mousePos.X <= bounds.Right) &&
                    (mousePos.X >= bounds.Right - width) &&
                    (mousePos.Y <= bounds.Top) &&
                    (mousePos.Y >= bounds.Bottom)
                        ? ScrollBarHover.YAxis : ScrollBarHover.None;
            }

            if (!e.Properties.scrollY && e.Properties.scrollX)
            {
                return (mousePos.X <= bounds.Right) &&
                    (mousePos.X >= bounds.Left) &&
                    (mousePos.Y <= bounds.Bottom + width) &&
                    (mousePos.Y >= bounds.Bottom)
                        ? ScrollBarHover.XAxis : ScrollBarHover.None;
            }

            if ((mousePos.X <= bounds.Right) &&
                (mousePos.X >= bounds.Right - width) &&
                (mousePos.Y <= bounds.Top) &&
                (mousePos.Y >= bounds.Bottom + width))
            {
                return ScrollBarHover.YAxis;
            }

            return (mousePos.X <= bounds.Right - width) &&
                (mousePos.X >= bounds.Left) &&
                (mousePos.Y <= bounds.Bottom + width) &&
                (mousePos.Y >= bounds.Bottom)
                    ? ScrollBarHover.XAxis : ScrollBarHover.None;
        }
        internal static floatv GetXScrollSize(this UIProperties prop)
        {
            floatv width = GetRenderSize(prop.Source).X;

            return prop.scrollY ? width - prop.ScrollBar.Width : width;
        }
        internal static floatv GetYScrollSize(this UIProperties prop)
        {
            floatv height = GetRenderSize(prop.Source).Y;

            return prop.scrollX ? height - prop.ScrollBar.Width : height;
        }
    }
    
    internal enum ScrollBarHover
    {
        None = 0,
        XAxis,
        YAxis
    }
}
