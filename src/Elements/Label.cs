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
        }

        public Label(TextLayout layout)
            : base(layout)
        {
            Text = "Label";
        }

        private readonly BorderShader _shader = BorderShader.GetInstance();

        public double BorderWidth { get; set; } = 1;
        public ColourF BorderColour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BackgroundColour { get; set; }
        public double CornerRadius { get; set; } = 0.01;

        private double BorderWidthDraw()
        {
            if (Focused)
            {
                return BorderWidth + 1d;
            }

            return BorderWidth;
        }

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);
            
            _shader.BorderWidth = Math.Max(BorderWidthDraw(), 0d);
            DrawingBoundOffset = _shader.BorderWidth;

            _shader.BorderColour = BorderColour;
            _shader.Radius = CornerRadius;

            _shader.ColourSource = ColourSource.UniformColour;
            _shader.Colour = BackgroundColour;

            _shader.Matrix3 = Projection;
            _shader.Size = Size;
            _shader.Matrix1 = Matrix4.CreateScale(Bounds.Size);
            Shapes.Square.Draw();

            if (Font == null || Text == null) { return; }

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;
            TextRenderer.DrawCentred(Text, Font, CharSpace, LineSpace);
        }
    }
}
