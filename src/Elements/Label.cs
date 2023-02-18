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

        public double BorderWidth { get; set; } = 1;
        public ColourF BorderColour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BackgroundColour { get; set; }
        public double CornerRadius { get; set; } = 0.01;

        public override GraphicsManager Graphics { get; }

        private double BorderWidthDraw()
        {
            if (Focused)
            {
                return BorderWidth + 1d;
            }

            return BorderWidth;
        }

        private class Renderer : GraphicsManager<Label>
        {
            public Renderer(Label source)
                : base(source)
            {

            }

            private readonly BorderShader _shader = BorderShader.GetInstance();

            public override void OnRender(DrawManager context)
            {
                context.Shader = _shader;

                _shader.BorderWidth = Math.Max(Source.BorderWidthDraw(), 0d);
                Size = Source.Size +  _shader.BorderWidth;

                // No point drawing box
                if (Source.BackgroundColour.A <= 0f && (Source.BorderColour.A <= 0f || _shader.BorderWidth <= 0))
                {
                    goto DrawText;
                }

                _shader.BorderColour = Source.BorderColour;
                _shader.Radius = Source.CornerRadius;

                _shader.ColourSource = ColourSource.UniformColour;
                _shader.Colour = Source.BackgroundColour;

                _shader.Size = Source.Size;

                context.Model = Matrix4.CreateScale(Source.Bounds.Size);
                context.Draw(Shapes.Square);

            DrawText:

                if (Source.Font == null || Source.Text == null) { return; }

                TextRenderer.Model = Matrix4.CreateScale(Source.TextSize);
                TextRenderer.Colour = Source.TextColour;
                TextRenderer.DrawCentred(context, Source.Text, Source.Font, Source.CharSpace, Source.LineSpace);
            }
        }
    }
}
