using System;
using Zene.Graphics;
using Zene.Structs;
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

        protected override Vector2 OnSizeChange(VectorEventArgs e)
        {
            SizeChange?.Invoke(this, e);
            return e.Value;
        }
        protected override Vector2 OnElementMove(VectorEventArgs e)
        {
            ElementMove?.Invoke(this, e);
            return e.Value;
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
