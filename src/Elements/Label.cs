using System;
using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public class Label : TextElement
    {
        public Label()
        {
            Text = "Label";
            Graphics = new Renderer(this);
        }

        public Label(TextLayout layout)
            : base(layout)
        {
            Text = "Label";
            Graphics = new Renderer(this);
        }

        public floatv BorderWidth { get; set; } = 1;
        public ColourF BorderColour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BackgroundColour { get; set; }
        public floatv CornerRadius { get; set; } = 0.01f;

        public override GraphicsManager Graphics { get; }

        private floatv BorderWidthDraw()
        {
            if (Focused)
            {
                return BorderWidth + 1;
            }

            return BorderWidth;
        }

        private class Renderer : GraphicsManager<Label>
        {
            public Renderer(Label source)
                : base(source)
            {

            }

            public override void OnRender(IDrawingContext context)
            {
                floatv borderWidth = Math.Max(Source.BorderWidthDraw(), 0);
                Size = Source.Size + borderWidth;

                // No point drawing box
                if (Source.BackgroundColour.A <= 0f && (Source.BorderColour.A <= 0f || borderWidth <= 0))
                {
                    goto DrawText;
                }

                context.DrawBorderBox(new Box(Vector2.Zero, Source.Bounds.Size), Source.BackgroundColour, borderWidth, Source.BorderColour, Source.CornerRadius);

            DrawText:

                if (Source.Font == null || Source.Text == null) { return; }

                context.Model = Matrix4.CreateScale(Source.TextSize);
                TextRenderer.Colour = Source.TextColour;
                TextRenderer.DrawCentred(context, Source.Text, Source.Font, Source.CharSpace, Source.LineSpace);
            }
        }
    }
}
