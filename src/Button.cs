using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class Button : TextElement
    {
        public Button(IBox bounds)
            : base(bounds, false)
        {
            CursorStyle = Cursor.Hand;
            Text = "Button";
            TextColour = ColourF.Zero;
            DrawingBounds();
        }
        public Button(ILayout layout)
            : base(layout, false)
        {
            CursorStyle = Cursor.Hand;
            Text = "Button";
            TextColour = ColourF.Zero;
            DrawingBounds();
        }

        public override BorderShader Shader { get; } = BorderShader.GetInstance();

        public ColourF Colour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BorderColour { get; set; } = new ColourF(0.6f, 0.6f, 0.6f);

        private double _bw = 5d;
        public double BorderWidth
        {
            get => _bw;
            set
            {
                _bw = value;

                DrawingBounds();
            }
        }
        public double CornerRadius { get; set; } = 0.1;

        public ITexture Texture { get; set; } = null;

        private double BorderWidthDraw()
        {
            double v = _bw;

            if (MouseSelect | MouseHover)
            {
                v += 2;
            }

            return v;
        }
        private void DrawingBounds()
        {
            DrawingBoundOffset = new Vector2I(BorderWidthDraw());
        }

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            Vector4 c = (Vector4)Colour;

            if (MouseSelect)
            {
                c -= new Vector4(0.2, 0.2, 0.2, 0d);
            }
            else if (MouseHover)
            {
                c -= new Vector4(0.1, 0.1, 0.1, 0d);
            }

            Shader.BorderWidth = BorderWidthDraw();
            DrawingBoundOffset = Shader.BorderWidth;

            Shader.BorderColour = BorderColour;
            Shader.Radius = CornerRadius;

            if (Texture != null)
            {
                Shader.ColourSource = ColourSource.Texture;
                Shader.TextureSlot = 0;
                Texture.Bind(0);
            }
            else
            {
                Shader.ColourSource = ColourSource.UniformColour;
                Shader.Colour = (ColourF)c;
            }

            Shader.Matrix3 = Projection;
            Shader.Size = Size;
            Shader.Matrix1 = Matrix4.CreateScale(Bounds.Size);
            Shapes.Square.Draw();

            if (Font == null || Text == null) { return; }

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;
            TextRenderer.DrawCentred(Text, Font, CharSpace, LineSpace);
        }
    }
}
