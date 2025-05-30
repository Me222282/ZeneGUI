﻿using System;
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

        public floatv BorderWidth { get; set; } = 3;
        public floatv CornerRadius { get; set; } = 0.1f;

        public ITexture Texture { get; set; } = null;

        public override GraphicsManager Graphics { get; }

        private floatv BorderWidthDraw()
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

            public override void OnRender(IDrawingContext context)
            {
                Vector4 c = (Vector4)Source.Colour;

                if (Source.MouseSelect)
                {
                    c -= new Vector4(0.2f, 0.2f, 0.2f, 0);
                }
                else if (Source.MouseHover)
                {
                    c -= new Vector4(0.1f, 0.1f, 0.1f, 0);
                }

                floatv borderWidth = Math.Max(Source.BorderWidthDraw(), 0);
                Size = Source.Size + borderWidth;

                // No point drawing box
                if (Source.Colour.A <= 0f && (Source.BorderColour.A <= 0f || borderWidth <= 0))
                {
                    goto DrawText;
                }

                if (Source.Texture != null)
                {
                    context.DrawBorderBox(new Box(Vector2.Zero, Source.Bounds.Size), Source.Texture, borderWidth, Source.BorderColour, Source.CornerRadius);
                }
                else
                {
                    context.DrawBorderBox(new Box(Vector2.Zero, Source.Bounds.Size), (ColourF)c, borderWidth, Source.BorderColour, Source.CornerRadius);
                }

            DrawText:

                if (Source.Font == null || Source.Text == null) { return; }

                context.Model = Matrix4.CreateScale(Source.TextSize);
                TextRenderer.Colour = Source.TextColour;
                TextRenderer.DrawCentred(context, Source.Text, Source.Font, Source.CharSpace, Source.LineSpace);
            }
        }
    }
}
