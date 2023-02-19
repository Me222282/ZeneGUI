namespace Zene.GUI
{
    public abstract class GraphicsManager<T> : GraphicsManager
        where T : IElement
    {
        protected GraphicsManager(T source)
            : base(source)
        {

        }

        public new T Source => (T)base.Source;
    }
}
