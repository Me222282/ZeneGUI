using System;
using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public class BoxColour : GraphicsManager
    {
        public BoxColour(IElement source)
            : base(source)
        {

        }

        public ColourF Colour { get; set; }

        public override void OnRender(IDrawingContext context)
        {
            // No colour
            if (Colour.A <= 0f) { return; }

            context.DrawBox(new Box(Vector2.Zero, Bounds.Size), Colour);
        }
    }
}
