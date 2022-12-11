using System;
using Zene.Graphics;

namespace Zene.GUI
{
    public delegate void FrameEventHandler(object sender, FrameEventArgs e);
    
    public class FrameEventArgs : EventArgs
    {
        public FrameEventArgs(IFramebuffer framebuffer)
        {
            Framebuffer = framebuffer;
        }

        public IFramebuffer Framebuffer { get; }
    }
}