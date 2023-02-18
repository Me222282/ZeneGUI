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

        private readonly BasicShader _shader = BasicShader.GetInstance();
        public ColourF Colour { get; set; }

        public override void OnRender(DrawManager context)
        {
            // No colour
            if (Colour.A <= 0f) { return; }

            context.Shader = _shader;

            _shader.ColourSource = ColourSource.UniformColour;
            _shader.Colour = Colour;

            context.Model = Matrix4.CreateScale(Bounds.Size);
            context.Draw(Shapes.Square);
        }
    }
}
