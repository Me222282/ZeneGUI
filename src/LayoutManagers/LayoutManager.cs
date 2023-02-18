using System;
using Zene.Structs;

namespace Zene.GUI
{
    public class LayoutManager : ILayoutManager
    {
        private class Instance : ILayoutManagerInstance
        {
            public Vector2 ReturningSize { get; init; }
        }

        public LayoutManager(bool cd, bool sd)
        {
            ChildDependent = cd;
            SizeDependent = sd;
        }

        public virtual bool ChildDependent { get; }
        public virtual bool SizeDependent { get; }

        public event EventHandler Change;
        protected void InvokeChange() => Change?.Invoke(this, EventArgs.Empty);

        public virtual ILayoutManagerInstance Init(LayoutArgs args) => new Instance() { ReturningSize = args.Size };
        public virtual Box GetBounds(LayoutArgs args, Box bounds, ILayoutManagerInstance instance) => bounds;

        public static readonly LayoutManager Empty = new LayoutManager(false, false);
    }
}
