using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public delegate void FrameEventHandler(object sender, FrameEventArgs e);
    
    public class FrameEventArgs : EventArgs
    {
        public FrameEventArgs(IFramebuffer framebuffer, IMatrixShader shader)
        {
            Framebuffer = framebuffer;
            Shader = shader;
        }

        public IFramebuffer Framebuffer { get; }
        public IMatrixShader Shader { get; }
    }
}