using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    /// <summary>
    /// An empty element for grouping together other UI elements.
    /// </summary>
    public class Container : Element
    {
        public Container()
        {
        }

        public Container(ILayout layout)
            : base(layout)
        {
        }

        private readonly BasicShader _shader = BasicShader.GetInstance();
        public ColourF Colour { get; set; }

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            // No colour
            if (Colour.A <= 0f) { return; }

            e.Context.Shader = _shader;

            _shader.ColourSource = ColourSource.UniformColour;
            _shader.Colour = Colour;
            _shader.Matrix1 = Matrix4.CreateScale(2d);
            _shader.Matrix2 = Matrix4.Identity;
            _shader.Matrix3 = Matrix4.Identity;

            e.Context.Draw(Shapes.Square);
        }
    }
}
