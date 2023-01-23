using System;
using Zene.Structs;

namespace Zene.GUI
{
    public abstract class LayoutManager<T> : LayoutManager where T : Element
    {
        /// <summary>
        /// Determines whether an exception should be thrown if child element is not of type <see cref="T"/>.
        /// </summary>
        protected bool ThrowException { get; set; } = true;

        /// <summary>
        /// Called once for every element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="args"></param>
        /// <param name="layoutResult"></param>
        /// <returns></returns>
        protected abstract Box GetBounds(T element, LayoutArgs args, Box layoutResult);

        protected override Box GetBounds(LayoutArgs args, Box layoutResult)
        {
            if (args.Element is not T e)
            {
                if (!ThrowException)
                {
                    return layoutResult;
                }

                throw new ArgumentException(nameof(args.Element));
            }

            return GetBounds(e, args,layoutResult);
        }
    }
}
