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
            TextColour = ColourF.Zero;
            DrawingBounds();
        }

        public Button(IBox bounds)
            : this()
        {
            Bounds = new Box(bounds);
        }
        public Button(ILayout layout)
            : this()
        {
            Layout = layout;
        }

        private readonly BorderShader _shader = BorderShader.GetInstance();

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
            if (MouseSelect | MouseHover)
            {
                return _bw + 2;
            }
            if (Focused)
            {
                return _bw + 1;
            }

            return _bw;
        }
        private void DrawingBounds()
        {
            DrawingBoundOffset = BorderWidthDraw();
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

            _shader.BorderWidth = BorderWidthDraw();
            DrawingBoundOffset = _shader.BorderWidth;

            _shader.BorderColour = BorderColour;
            _shader.Radius = CornerRadius;

            if (Texture != null)
            {
                _shader.ColourSource = ColourSource.Texture;
                _shader.TextureSlot = 0;
                Texture.Bind(0);
            }
            else
            {
                _shader.ColourSource = ColourSource.UniformColour;
                _shader.Colour = (ColourF)c;
            }

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
