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

        public Label(ILayout layout)
            : base(layout)
        {
            Text = "Label";
        }

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            if (Font == null || Text == null) { return; }

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;
            TextRenderer.DrawCentred(Text, Font, CharSpace, LineSpace);
            //TextRenderer.DrawLeftBound(Text, Font, 0, 0);
        }
    }
}
