using System;

namespace Zene.GUI
{
    public class Children<T> : ElementList
        where T : IElement
    {
        public Children(IElement source)
            : base(source)
        {

        }

        public void Add(T item) => base.Add(item);
        public override void Add(IElement item)
        {
            if (item is not T)
            {
                throw new Exception($"Child elements can only be of type {typeof(T)}");
            }

            base.Add(item);
        }
    }
}
