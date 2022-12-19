using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class Button : Element
    {
        public Button(IBox bounds)
            : base(bounds, true)
        {
            Shader = new BorderShader()
            {
                BorderColour = new ColourF(0.6f, 0.6f, 0.6f),
                Radius = 0.1
            };
        }

        public Button(ILayout layout)
            : base(layout, true)
        {
            Shader = new BorderShader()
            {
                BorderColour = new ColourF(0.6f, 0.6f, 0.6f),
                Radius = 0.1
            };
        }

        public override BorderShader Shader { get; }

        public event MouseEventHandler Click;

        public string Text { get; set; }
        public double TextSize { get; set; } = 10d;

        public Font Font { get; set; }

        public ColourF Colour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF TextColour { get; set; } = ColourF.Zero;
        public ColourF BorderColour
        {
            get => Shader.BorderColour;
            set => Shader.BorderColour = value;
        }

        private double _bw = 5d;
        public double BorderWidth
        {
            get => _bw;
            set => _bw = value;
        }
        public double CornerRadius
        {
            get => Shader.Radius;
            set => Shader.Radius = value;
        }

        public ITexture Texture { get; set; }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Click?.Invoke(this, e);
        }

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            Vector4 c = (Vector4)Colour;

            if (MouseSelect)
            {
                c -= new Vector4(0.2, 0.2, 0.2, 0d);
                Shader.BorderWidth = _bw + 2;
            }
            else if (MouseHover)
            {
                c -= new Vector4(0.1, 0.1, 0.1, 0d);
                Shader.BorderWidth = _bw + 2;
            }
            else
            {
                Shader.BorderWidth = _bw;
            }

            e.Framebuffer.Clear((ColourF)c);

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;
            TextRenderer.DrawCentred(Text, Font, 0, 0);
        }

        protected override void OnSizeChange(SizeChangeEventArgs e)
        {
            base.OnSizeChange(e);

            Actions.Push(() =>
            {
                Shader.Matrix3 = Projection;
                Shader.Size = e.Size;
            });
        }
    }
}
