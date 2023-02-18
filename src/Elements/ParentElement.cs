namespace Zene.GUI
{
    public abstract class ParentElement : Element
    {
        protected ParentElement()
        {
            Children = new ElementList(this);
        }

        protected ParentElement(ILayout layout)
            : base(layout)
        {
            Children = new ElementList(this);
        }

        public override ElementList Children { get; }

        public void AddChild(IElement element) => Children.Add(element);
        public void RemoveChild(IElement element) => Children.Remove(element);

        public void ClearChildren() => Children.Clear();

        public T FindChild<T>() where T : class, IElement
            => Children.Find<T>();

        public void SwapChildren(int indexA, int indexB) => Children.Swap(indexA, indexB);
        public void SwapChildren(IElement a, IElement b) => Children.Swap(a, b);
    }

    public abstract class ParentElement<T> : ParentElement
        where T : class, ILayout
    {
        public ParentElement()
        {

        }
        public ParentElement(T layout)
            : base(layout)
        {

        }

        public new T Layout
        {
            get => base.Layout as T;
            set => base.Layout = value;
        }
    }
}
