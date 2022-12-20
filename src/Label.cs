using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            /*
            e.Framebuffer.Clear(new Colour(100, 150, 200));
            if (MouseHover)
            {
                e.Framebuffer.Clear(new Colour(200, 150, 100));
            }*/

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;
            TextRenderer.DrawCentred(Text, Font, 0, 0);
        }
    }
}
