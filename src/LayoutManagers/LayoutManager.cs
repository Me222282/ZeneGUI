using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class LayoutManager
    {
        public static readonly LayoutManager Empty = new LayoutManager();

        public event EventHandler Change;
        protected void InvokeChange() => Change?.Invoke(this, EventArgs.Empty);

        public Vector2 ManageLayouts(LayoutArgs args, Action<Element, Box> setBounds)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            if (setBounds == null)
            {
                throw new ArgumentNullException(nameof(setBounds));
            }

            // Pre processing for layout manager
            SetupManager(args);

            ReadOnlySpan<Element> span = args.Neighbours.AsSpan();

            for (int i = 0; i < span.Length; i++)
            {
                // Needed for Element.cs
                // Makes sure _elementIndex is never wrong
                span[i]._elementIndex = i;

                ILayout layout = span[i].Layout;

                if (!span[i].Visable ||
                    layout == null)// ||
                    //span[i].Layout is FixedLayout)
                {
                    continue;
                }

                LayoutArgs la = new LayoutArgs(span[i], args.Size, args.Neighbours);
                Box newBounds = layout.GetBounds(la);

                newBounds = GetBounds(la, newBounds);

                setBounds(span[i], newBounds);
            }

            return args.Size;
        }

        /// <summary>
        /// Called before elements get managed.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void SetupManager(LayoutArgs args)
        {

        }

        /// <summary>
        /// Called once for every element.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="layoutResult"></param>
        /// <returns></returns>
        protected virtual Box GetBounds(LayoutArgs args, Box layoutResult) => layoutResult;
    }
}
