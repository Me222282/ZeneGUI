using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class UIManager
    {
        public void LayoutAllElements()
        {
            // Recreate structure with temp structs holding various values
            
            // Fit parse - with min and max
            // from top down
            // Grow and shrink parse
            
            
            FitSizing(Root);
            PositionChildren(Root);
        }
        
        private void FitSizing(Element e)
        {
            floatv along = 0;
            floatv across = 0;
            
            for (int i = 0; i < e.Children.Count; i++)
            {
                // ⚠︎ RECURSION ⚠︎
                Element el = e.Children[i];
                FitSizing(el);
                
                along += el.minSize.X;
                across = Maths.Max(across, el.minSize.Y);
            }
            
            e.maxSize = (e.width.Max(this), e.height.Max(this));
            // child min clamped between min and max size
            e.minSize = (Math.Clamp(along, e.width.Min(this), e.maxSize.X),
                Math.Clamp(across, e.height.Min(this), e.maxSize.Y));
            
            e.bounds.Width = e.minSize.X;
            e.bounds.Height = e.minSize.Y;
        }
        private void PositionChildren(Element e)
        {
            floatv currentX = e.bounds.Width * 0.5f;
            floatv top = e.bounds.Height * 0.5f;
            
            for (int i = 0; i < e.Children.Count; i++)
            {
                // ⚠︎ RECURSION ⚠︎
                Element el = e.Children[i];
                PositionChildren(el);
                
                el.bounds.Location = (currentX, top);
                currentX += el.bounds.Width;
            }
        }
        
        public UIManager(Window window, Element root)
        {
            Window = window;
            Root = root;
            
            Framebuffer = new TextureRenderer(window.Width, window.Height);
            Framebuffer.SetColourAttachment(0, TextureFormat.Rgba8);
            Framebuffer.SetDepthAttachment(TextureFormat.DepthComponent24, false);
            _uiView = new UIView(Framebuffer.Viewport);
            Framebuffer.Viewport = _uiView;
            Framebuffer.Scissor = _uiView;
        }
        
        private readonly UIView _uiView;
        public TextureRenderer Framebuffer { get; }
        public Window Window { get; }
        public Element Root { get; set; }
        public Vector2 FrameSize => _uiView.FrameSize;
        public Vector2 FrameSizeSL { get; private set; }
        
        public void SetFrameSize(Vector2 size)
        {
            if (size.X <= 0 || size.Y <= 0) { return; }

            _uiView.FrameSize = size;
            if (size.X < size.Y)
            {
                FrameSizeSL = size;
            }
            else
            {
                FrameSizeSL = new Vector2(size.Y, size.X);
            }

            Window.GraphicsContext.Actions.Push(() =>
            {
                Framebuffer.Size = (Vector2I)size;
                LayoutAllElements();
                // Box bounds = CalcElementBounds(Root, new LayoutArgs(Root, size, Elements));
                // Root.Properties.bounds = bounds;
            });
        }
        
        public void OnRender(IDrawingContext context)
        {
            _uiView.Scale = 1;
            _uiView.Offset = 0;
            _uiView.ScissorBox = new GLBox(0, Framebuffer.Size);
            _uiView.ScissorOffset = 0;

            _uiView.View = new Box(0d, Framebuffer.Size);

            _uiView.DepthRange = new Vector2(0, 1);
            _uiView.DepthDivision = 1;
            _uiView.ChildDivision = 1;
            _uiView.SetDepth(0);

            DrawContext dm = new DrawContext(Framebuffer)
            {
                DepthState = _uiView,
                RenderState = RenderState.BlendReady
            };
            Framebuffer.Clear(BufferBit.Colour | BufferBit.Depth);
            Render(Root, dm);
            
            _uiView.Scale = 1;
            _uiView.Offset = 0;
            _uiView.ScissorBox = new GLBox(0, Framebuffer.Size);

            context.WriteFramebuffer(Framebuffer, BufferBit.Colour, TextureSampling.Nearest);
        }
        internal void SetRenderSize(Vector2 size) => _uiView.Size = size;
        internal void SetRenderLocation(Vector2 pos) => _uiView.Location = pos;
        private void Render(Element e, IDrawingContext dm)
        {
            // renderFocus = e;
            
            // dm.Projection = Matrix4.CreateOrthographicOffCentre(0, e.bounds.Width, e.bounds.Height, 0, 0, 1);//e.Graphics?.Projection;
            dm.Projection = Matrix4.CreateOrthographic(e.bounds.Width, e.bounds.Height, 0, 1);//e.Graphics?.Projection;
            dm.View = Matrix.Identity;
            dm.Model = Matrix.Identity;
            // e.Events.OnUpdate();
            dm.Render(e);
            //e.Graphics?.OnRender(dm);

            // renderFocus = null;

            if (e.Children.Count == 0) { return; }

            floatv scaleRef = _uiView.Scale * 1;//e.Properties.ViewScale;
            Vector2 offsetRef = _uiView.Offset + e.bounds.Centre;// * e.Properties.ViewScale) + e.Properties.ViewPan;
            GLBox scissor = _uiView.PassScissor();

            Vector2 depthRange = _uiView.DepthRange;
            int depthDiv = _uiView.DepthDivision;
            int offset = e == Root ? 0 : 1;

            foreach (Element child in e.Children)
            {
                // if (!child.Properties.Visable) { continue; }

                _uiView.Scale = scaleRef;
                _uiView.Offset = offsetRef;
                _uiView.ScissorBox = scissor;
                
                _uiView.ScissorOffset = 0;

                Box bounds = child.bounds;
                _uiView.View = bounds;

                if (!_uiView.Visable) { continue; }

                _uiView.DepthRange = depthRange;
                _uiView.DepthDivision = depthDiv;
                _uiView.ChildDivision = child.Children.Count + 1;
                _uiView.SetDepth(/*child.Properties.Depth + */offset);

                Render(child, dm);
            }
        }
    }
}