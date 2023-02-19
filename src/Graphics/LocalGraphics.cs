using System;
using Zene.Graphics;
using Zene.Windowing;

namespace Zene.GUI.Graphics
{
    public delegate void GraphicsHandler(object sender, DrawManager context);

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

        public override void OnRender(DrawManager context) => Render?.Invoke(this, context);

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
}
