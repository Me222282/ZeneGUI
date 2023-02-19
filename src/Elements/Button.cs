using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class Button : TextElement
    {
        public Button()
        {
            CursorStyle = Cursor.Hand;
            Text = "Button";
            TextColour = new ColourF(0f, 0f, 0f);
            Graphics = new Renderer(this);
        }
        public Button(TextLayout layout)
            : base(layout)
        {
            CursorStyle = Cursor.Hand;
            Text = "Button";
            TextColour = new ColourF(0f, 0f, 0f);
            Graphics = new Renderer(this);
        }

        public ColourF Colour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BorderColour { get; set; } = new ColourF(0.6f, 0.6f, 0.6f);

        public double BorderWidth { get; set; } = 3d;
        public double CornerRadius { get; set; } = 0.1;

        public ITexture Texture { get; set; } = null;

        public override GraphicsManager Graphics { get; }

        private double BorderWidthDraw()
        {
            if (MouseSelect | MouseHover)
            {
                return BorderWidth + 2;
            }
            if (Focused)
            {
                return BorderWidth + 1;
            }

            return BorderWidth;
        }

        private class Renderer : GraphicsManager<Button>
        {
            public Renderer(Button source)
                : base(source)
            {

            }

            private readonly BorderShader _shader = BorderShader.GetInstance();

            public override void OnRender(DrawManager context)
            {
                Vector4 c = (Vector4)Source.Colour;

                if (Source.MouseSelect)
                {
                    c -= new Vector4(0.2, 0.2, 0.2, 0d);
                }
                else if (Source.MouseHover)
                {
                    c -= new Vector4(0.1, 0.1, 0.1, 0d);
                }

                context.Shader = _shader;

                _shader.BorderWidth = Math.Max(Source.BorderWidthDraw(), 0d);
                Size = Source.Size + _shader.BorderWidth;

                // No point drawing box
                if (Source.Colour.A <= 0f && (Source.BorderColour.A <= 0f || _shader.BorderWidth <= 0))
                {
                    goto DrawText;
                }

                _shader.BorderColour = Source.BorderColour;
                _shader.Radius = Source.CornerRadius;

                if (Source.Texture != null)
                {
                    _shader.ColourSource = ColourSource.Texture;
                    _shader.TextureSlot = 0;
                    Source.Texture.Bind(0);
                }
                else
                {
                    _shader.ColourSource = ColourSource.UniformColour;
                    _shader.Colour = (ColourF)c;
                }

                _shader.Size = Source.Size;

                context.Model = Matrix4.CreateScale(Source.Bounds.Size);
                context.Draw(Shapes.Square);

            DrawText:

                if (Source.Font == null || Source.Text == null) { return; }

                context.Model = Matrix4.CreateScale(Source.TextSize);
                TextRenderer.Colour = Source.TextColour;
                TextRenderer.DrawCentred(context, Source.Text, Source.Font, Source.CharSpace, Source.LineSpace);
            }
        }
    }
}
