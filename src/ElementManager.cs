using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    /// <summary>
    /// The root parent element.
    /// </summary>
    public class ElementManager : Element
    {
        internal override TextRenderer textRender { get; }
        public override IBasicShader Shader { get; }

        public ElementManager(Window w)
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
            _window.SizeChange += (_, e) => OnSizeChange(e);
            _window.Start += (_, _) => OnStart();

            //_shader = new BasicShader();
            Shader = new BasicShader();

            textRender = new TextRenderer();
        }

        private new void MouseMove(object s, MouseEventArgs e)
        {
            Vector2 ml = e.Location - (_window.Size * 0.5);
            ml.Y = -ml.Y;

            OnMouseMove(new MouseEventArgs(ml));

            _window.CursorStyle = _currentCursor;
        }

        public void Render()
        {
            Render(State.GetBoundFramebuffer(FrameTarget.Draw), Projection);
        }

        protected override void OnSizeChange(SizeChangeEventArgs e)
        {
            base.OnSizeChange(e);

            Location = (e.Size.X / -2, e.Size.Y / 2);
        }
    }
}