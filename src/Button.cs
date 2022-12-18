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
            : base(bounds, false)
        {
            Shader = new BorderShader();
        }

        public Button(ILayout layout)
            : base(layout, false)
        {
            Shader = new BorderShader();
        }

        public override BorderShader Shader { get; }

        public event MouseEventHandler Click;

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            Click?.Invoke(this, e);
        }


    }
}
