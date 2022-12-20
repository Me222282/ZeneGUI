using Zene.Graphics;
using Zene.Structs;

namespace Zene.GUI
{
    public class Label : TextElement
    {
        public Label(IBox bounds)
            : base(bounds, false)
        {
            Text = "Label";
        }

        public Label(ILayout layout)
            : base(layout, false)
        {
            Text = "Label";
        }

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            if (Font == null || Text == null) { return; }

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;
            TextRenderer.DrawCentred(Text, Font, 0, 0);
        }
    }
}
