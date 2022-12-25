using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    /// <summary>
    /// The root parent element.
    /// </summary>
    public class RootElement : Element
    {
        internal override TextRenderer textRender { get; }
        private readonly IBasicShader _shader;

        internal override TextureRenderer framebuffer { get; }

        public RootElement(Window w)
            : base(new Box(Vector2.Zero, w.Size))
        {
            _window = w;

            // Add events listeners
            _window.MouseMove += MouseMove;
            _window.MouseDown += (_, e) => OnMouseDown(new MouseEventArgs(MouseLocation, e.Button, e.Modifier));
            _window.MouseUp += (_, e) => OnMouseUp(new MouseEventArgs(MouseLocation, e.Button, e.Modifier));
            _window.Scroll += (_, e) => OnScroll(e);
            _window.KeyDown += (_, e) => OnKeyDown(e);
            _window.KeyUp += (_, e) => OnKeyUp(e);
            _window.TextInput += (_, e) => OnTextInput(e);
            _window.SizeChange += (_, e) => OnSizeChange((VectorEventArgs)e);
            _window.Start += (_, _) => OnStart();

            //_shader = new BasicShader();
            _shader = BasicShader.GetInstance();

            textRender = new TextRenderer();
            framebuffer = new TextureRenderer(w.Width, w.Height);
            framebuffer.SetColourAttachment(0, TextureFormat.Rgba8);
            framebuffer.SetDepthAttachment(TextureFormat.Depth24Stencil8, false);
        }

        private new void MouseMove(object s, MouseEventArgs e)
        {
            Vector2 ml = e.Location - (_window.Size * 0.5);
            ml.Y = -ml.Y;

            OnMouseMove(new MouseEventArgs(ml));

            _window.CursorStyle = _currentCursor;
        }

        /// <summary>
        /// Renders all visable elements to the current context.
        /// </summary>
        public void Render()
        {
            IFramebuffer current = State.GetBoundFramebuffer(FrameTarget.Draw);

            framebuffer.Clear(BufferBit.Colour | BufferBit.Depth);
            framebuffer.Scissor.Enabled = true;

            Render(Matrix4.Identity, Projection);

            // Copy data to main framebuffer
            framebuffer.CopyFrameBuffer(current, BufferBit.Colour, TextureSampling.Nearest);
        }

        protected override void OnSizeChange(VectorEventArgs e)
        {
            base.OnSizeChange(e);

            if (e.X <= 0 || e.Y <= 0) { return; }

            Actions.Push(() =>
            {
                framebuffer.Size = (Vector2I)e.Value;
            });
        }
    }
}