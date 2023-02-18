using System;
using Zene.Structs;

namespace Zene.GUI
{
    public abstract class LayoutManager<T> : LayoutManager
        where T : Element
    {
        protected LayoutManager(bool cd, bool sd)
            : base(cd, sd)
        {

        }

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
        protected abstract Box GetBounds(T element, LayoutArgs args, Box layoutResult, ILayoutManagerInstance instance);

        public override Box GetBounds(LayoutArgs args, Box layoutResult, ILayoutManagerInstance instance)
        {
            if (args.Element is not T e)
            {
                if (!ThrowException)
                {
                    return layoutResult;
                }

                throw new ArgumentException("Child element not of expected type.", nameof(args));
            }

            return GetBounds(e, args, layoutResult, instance);
        }
    }

    public abstract class LayoutManagerI<T> : LayoutManager
        where T : ILayoutManagerInstance
    {
        protected LayoutManagerI(bool cd, bool sd)
            : base(cd, sd)
        {

        }

        /// <summary>
        /// Determines whether an exception should be thrown if instance is not of type <see cref="T"/>.
        /// </summary>
        protected bool ThrowException { get; set; } = true;

        /// <summary>
        /// Called once for every element.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="layoutResult"></param>
        /// <returns></returns>
        protected abstract Box GetBounds(LayoutArgs args, Box layoutResult, T instance);

        public override Box GetBounds(LayoutArgs args, Box layoutResult, ILayoutManagerInstance instance)
        {
            if (instance is not T inst)
            {
                if (!ThrowException)
                {
                    return layoutResult;
                }

                throw new ArgumentException("Instance not of expected type.", nameof(instance));
            }

            return GetBounds(args, layoutResult, inst);
        }
    }

    public abstract class LayoutManagerTI<T, I> : LayoutManager
        where T : Element
        where I : ILayoutManagerInstance
    {
        protected LayoutManagerTI(bool cd, bool sd)
            : base(cd, sd)
        {

        }

        /// <summary>
        /// Determines whether an exception should be thrown if instance is not of type <see cref="I"/>,
        /// or child element is not of type <see cref="T"/>.
        /// </summary>
        protected bool ThrowException { get; set; } = true;

        /// <summary>
        /// Called once for every element.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="layoutResult"></param>
        /// <returns></returns>
        protected abstract Box GetBounds(T element, LayoutArgs args, Box layoutResult, I instance);

        public override Box GetBounds(LayoutArgs args, Box layoutResult, ILayoutManagerInstance instance)
        {
            if (instance is not I inst)
            {
                if (!ThrowException)
                {
                    return layoutResult;
                }

                throw new ArgumentException("Instance not of expected type.", nameof(instance));
            }

            if (args.Element is not T e)
            {
                if (!ThrowException)
                {
                    return layoutResult;
                }

                throw new ArgumentException("Child element not of expected type.", nameof(args));
            }

            return GetBounds(e, args, layoutResult, inst);
        }
    }
}
