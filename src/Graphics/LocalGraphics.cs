using System;
using Zene.Graphics;
using Zene.Windowing;

namespace Zene.GUI
{
    public delegate void GraphicsHandler(object sender, RenderArgs context);

    public class LocalGraphics : GraphicsManager
    {
        public LocalGraphics(IElement source)
            : base(source)
        {

        }
        public LocalGraphics(IElement source, GraphicsHandler render)
            : base(source)
        {
            Render += render;
        }

        public event GraphicsHandler Render;
        public event VectorEventHandler SizeChange;
        public event VectorEventHandler ElementMove;

        public override void OnRender(DrawManager context) => Render?.Invoke(this, new RenderArgs(context, TextRenderer));

        protected override void OnSizeChange(VectorEventArgs e)
        {
            base.OnSizeChange(e);

            SizeChange?.Invoke(this, e);
        }
        protected override void OnElementMove(VectorEventArgs e)
        {
            base.OnElementMove(e);

            ElementMove?.Invoke(this, e);
        }
    }

    public class RenderArgs : EventArgs
    {
        public RenderArgs(DrawManager dm, TextRenderer tr)
        {
            Context = dm;
            TextRenderer = tr;
        }

        public TextRenderer TextRenderer { get; }
        public DrawManager Context { get; }
    }
}
