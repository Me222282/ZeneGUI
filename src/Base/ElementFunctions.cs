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

            return CalculateLocalMouse(e.Properties.Parent, rootLocation) - e.Bounds.Centre;
        }
        public static bool IsVisable(this IElement e)
        {
            if (!e.HasParent)
            {
                return e.Properties.Visable;
            }

            return e.Properties.Visable && IsVisable(e.Properties.Parent);
        }

        public static IElement LowestFirstElement(this IElement e)
        {
            if (!e.HasChildren) { return e; }

            return LowestFirstElement(e.Children[0]);
        }
        public static IElement LowestLastElement(this IElement e)
        {
            if (!e.HasChildren) { return e; }

            return LowestLastElement(e.Children[^1]);
        }
        public static IElement NextElement(this IElement e)
        {
            if (!e.HasParent) { return null; }

            IElement parent = e.Properties.parent;
            IElement next;

            // This is last element
            if (e.Properties.elementIndex == (parent.Children.Length - 1))
            {
                next = NextElement(parent);

                // Loop round to first element
                if (next == null)
                {
                    next = e.Properties.RootElement;
                }
            }
            else
            {
                next = parent.Children[e.Properties.elementIndex + 1];
            }

            return next?.LowestFirstElement();
        }
        public static IElement PreviousElement(this IElement e)
        {
            if (!e.HasParent) { return null; }

            IElement parent = e.Properties.parent;
            IElement next;

            // This is fisrt element
            if (e.Properties.elementIndex == 0)
            {
                next = PreviousElement(parent);

                // Loop round to first element
                if (next == null)
                {
                    next = e.Properties.RootElement;
                }
            }
            else
            {
                next = parent.Children[e.Properties.elementIndex - 1];
            }

            return next?.LowestLastElement();
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
    }
}
