using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class UIManager : IRenderable
    {
        public UIManager(IElement root, Window window)
            : this(window)
        {
            Root_(root);

        }
        protected UIManager(Window window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));

            Framebuffer = new TextureRenderer(window.Width, window.Height);
            Framebuffer.SetColourAttachment(0, TextureFormat.Rgba8);
            Framebuffer.SetDepthAttachment(TextureFormat.DepthComponent24, false);
            _uiView = new UIView(Framebuffer.Viewport);
            Framebuffer.Viewport = _uiView;
            Framebuffer.Scissor = _uiView;

            TextRenderer = new TextRenderer();
            Animator = new Animator();
        }
        protected void Root_(IElement root)
        {
            if (Root != null)
            {
                throw new Exception();
            }

            if (root.Properties.elementIndex >= 0)
            {
                throw new ArgumentException("The given root is a child of another element.", nameof(root));
            }

            Root = root ?? throw new ArgumentNullException(nameof(root));
            Elements = new ElementList(root);
            Root.Properties.handle = this;
            Root.Properties.parent = null;

            Hover = root;
            _focus = root;
        }

        public ElementList Elements { get; private set; }
        public Window Window { get; }

        public IElement Root { get; private set; }

        protected IElement uninteractHover;
        public IElement Hover { get; private set; }
        private IElement _focus;
        public IElement Focus
        {
            get => _focus;
            set
            {
                if (value == null)
                {
                    value = Root;
                }

                if (_focus == value ||
                    value.Properties.handle != this) { return; }

                _focus.OnFocus(false);
                _focus = value;

                if (!Window.Focused)
                {
                    Windowing.Base.GLFW.FocusWindow(Window.Handle);
                    return;
                }

                _focus.OnFocus(true);
            }
        }
        internal void ResetFocusNoEvent()
        {
            _focus = Root;
            _focus.OnFocus(true);
        }
        internal void ResetHoverNoEvent()
        {
            Hover = Root;
            Hover.OnMouseEnter();
        }

        private readonly UIView _uiView;
        public TextureRenderer Framebuffer { get; }
        public TextRenderer TextRenderer { get; }

        public Animator Animator { get; }

        public void Animate<T>(AnimatorData<T> data) => Animator.Add(data);

        public void ShiftFocusRight()
        {
            if (!_focus.HasParent || _focus.HasChildren)
            {
                Focus = _focus.LowestFirstElement();
                return;
            }

            Focus = _focus.NextElement();
        }
        public void ShiftFocusLeft()
        {
            if (!_focus.HasParent)
            {
                Focus = _focus.LowestLastElement();
            }

            Focus = _focus.PreviousElement();
        }

        public void MouseMove(MouseEventArgs e)
        {
            Root.Properties.mousePos = e.Location;

            if (Window.MouseButton != MouseButton.None)
            {
                ManageMouseSelect(e);
                return;
            }

            IElement hover = Root;
            uninteractHover = null;
            Vector2 localMouse = e.Location;

            // If hover over root scroll bars
            ScrollBarHover sbh;
            if ((sbh = hover.InScrollBar(localMouse)) != ScrollBarHover.None)
            {
                hover.Properties.scrollBarHover = sbh;
                goto SkipChildren;
            }

            // Find subhover element
            while (hover.HasChildren)
            {
                // Adjust for parents view panning and scale
                Vector2 viewMouse = (localMouse - hover.Properties.ViewPan) / hover.Properties.ViewScale;
                IElement nh = ManagerMouseMove(hover.Children, viewMouse);

                // Not hovering over any child elements
                if (nh == null) { break; }

                hover = nh;
                localMouse = viewMouse;
                localMouse -= nh.Properties.bounds.Centre;
                //localMouse = (localMouse - nh.Properties.ViewPan) / nh.Properties.ViewScale;

                // Hover over scroll bar
                if (nh.Properties.scrollBarHover != ScrollBarHover.None) { break; }
            }
            // Element cannot be hovered element
            if (!hover.Properties.Interactable && hover.Properties.scrollBarHover == ScrollBarHover.None)
            {
                uninteractHover = hover;
                // Finds lowest element in hover tree that can be a hover element.
                hover = FindHoverElement(hover, ref localMouse);
            }

        SkipChildren:

            if (Hover != hover)
            {
                Hover.OnMouseLeave();
                Hover.Properties.scrollBarHover = ScrollBarHover.None;
                hover.OnMouseEnter();

                Hover = hover;
                Window.CursorStyle = hover.Properties.CursorStyle;
            }

            if (hover == Root) { return; }

            // Adjust mouse pos sent according to property
            if (hover.Properties.ShiftInternalMouse)
            {
                localMouse = (localMouse - hover.Properties.ViewPan) / hover.Properties.ViewScale;
            }

            hover.OnMouseMove(new MouseEventArgs(localMouse, e.Button, e.Modifier));
        }
        private void ManageMouseSelect(MouseEventArgs e)
        {
            IElement parent = Hover.HasParent ? Hover.Properties.parent : Hover;

            Vector2 localMouse = parent.CalculateLocalMouse(e.Location);
            bool mouseOver = Hover.IsMouseHover(localMouse);

            // Calculate local mouse for hover element from parent
            //localMouse = Hover.CalculateLocalMouse(parent, localMouse);
            localMouse -= Hover.Properties.bounds.Centre;

            if (mouseOver && !Hover.Properties.hover)
            {
                Hover.OnMouseEnter();
            }
            else if (!mouseOver)
            {
                Hover.OnMouseLeave();
            }

            if (Hover != Root)
            {
                Vector2 adjustmouse = localMouse;
                // Adjust mouse pos sent according to property
                if (Hover.Properties.ShiftInternalMouse)
                {
                    adjustmouse = (adjustmouse - Hover.Properties.ViewPan) / Hover.Properties.ViewScale;
                }

                Hover.OnMouseMove(new MouseEventArgs(adjustmouse, e.Button, e.Modifier));
            }

            if (Hover.Properties.scrollBarHover != ScrollBarHover.None)
            {
                ManageMouseScroll(Hover, localMouse);
            }
        }
        private static IElement ManagerMouseMove(ElementList el, Vector2 mousePos)
        {
            if (el == null) { return null; }

            double depth = 0d;
            IElement hover = null;
            ScrollBarHover hoverScroll = ScrollBarHover.None;
            foreach (IElement e in el)
            {
                if (!e.Properties.Visable || e.Properties.Depth < depth) { continue; }

                ScrollBarHover scroll = e.InScrollBar(mousePos);
                if (scroll != ScrollBarHover.None)
                {
                    hoverScroll = scroll;
                    depth = e.Properties.Depth;
                    hover = e;
                    continue;
                }

                if (e.IsMouseHover(mousePos))
                {
                    hoverScroll = ScrollBarHover.None;
                    depth = e.Properties.Depth;
                    hover = e;
                }
            }

            if (hover != null)
            {
                hover.Properties.scrollBarHover = hoverScroll;
            }
            return hover;
        }
        private IElement FindHoverElement(IElement lowest, ref Vector2 mousePos)
        {
            if (!lowest.HasParent || lowest.Properties.Interactable)
            {
                return lowest;
            }

            //mousePos = (mousePos * lowest.Properties.ViewScale) + lowest.Properties.ViewPan + lowest.Properties.bounds.Centre;
            mousePos += lowest.Properties.bounds.Centre;
            IElement parent = lowest.Properties.parent;
            mousePos = (mousePos * parent.Properties.ViewScale) + parent.Properties.ViewPan;
            return FindHoverElement(parent, ref mousePos);
        }
        protected static void ManageMouseScroll(IElement e, Vector2 mousePos)
        {
            Vector2 scrollPercent = e.Properties.GetScrollPercent();
            UIProperties prop = e.Properties;
            double perc;

            if (prop.scrollBarHover == ScrollBarHover.XAxis)
            {
                perc = prop.ScrollBar.GetScrollPercentage(prop.scrollMoveRange.X, mousePos, false);
                prop.SetXScroll(perc);
                prop.initScrollPerc = perc;
                return;
            }

            //Console.WriteLine(prop.scrollMoveRange.Y);
            perc = prop.ScrollBar.GetScrollPercentage(prop.scrollMoveRange.Y, mousePos, true);
            prop.SetYScroll(perc);
            prop.initScrollPerc = perc;
        }

        protected void TriggerChange()
        {
            CalcElementBounds(Root, new LayoutArgs(Root, Root.Properties.bounds.Size, Elements));
            
            // Change in layout causes hover to be recalculated
            MouseMove(new MouseEventArgs(Root.Properties.mousePos));
        }

        public void LayoutElement(IElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (!element.HasParent)
            {
                TriggerChange();
                return;
            }

            Box old = element.Properties.bounds;
            if (element.Properties.parent.LayoutManager != null &&
                element.Properties.parent.LayoutManager != LayoutManager.Empty)
            {
                IElement parent = element.Properties.parent;

                if (parent.LayoutManager.ChildDependent ||
                    parent.LayoutManager.SizeDependent)
                {
                    LayoutElement(element.Properties.parent);
                    return;
                }

                ILayoutManager lm = parent.LayoutManager;
                ILayoutManagerInstance lmi = lm.Init(new LayoutArgs(element, parent.Bounds.Size, parent.Children));

                LayoutArgs args = new LayoutArgs(element, parent.Size, parent.Children);
                Box bounds = CalcElementBounds(element, args);
                bounds = lm.GetBounds(args, bounds, lmi);

                element.OnSizeChange(new VectorEventArgs(bounds.Size));
                element.OnElementMove(new VectorEventArgs(bounds.Location));
            }
            else
            {
                Box bounds = CalcElementBounds(element, new LayoutArgs(element, element.Properties.parent.Size, element.Properties.parent.Children));
                element.OnSizeChange(new VectorEventArgs(bounds.Size));
                element.OnElementMove(new VectorEventArgs(bounds.Location));
            }

            if (element.Graphics == null)
            {
                element.ViewBoxChange(old);
            }

            // Change in layout causes hover to be recalculated
            MouseMove(new MouseEventArgs(Root.Properties.mousePos));
        }
        private Box CalcElementBounds(IElement element, LayoutArgs args)
        {
            element.Properties.scrollViewBox = new Box();

            Box bounds = element.Layout.GetBounds(args);

            if (!element.HasChildren) { return bounds; }

            ILayoutManager lm = element.LayoutManager ?? throw new ArgumentNullException(nameof(element));
            ILayoutManagerInstance lmi = lm.Init(new LayoutArgs(element, bounds.Size, element.Children));
            
            Box[] boxes = new Box[element.Children.Length];
            
            int i = -1;
            foreach (IElement e in element.Children)
            {
                i++;
                if (!e.Properties.Visable) { continue; }

                LayoutArgs ea = new LayoutArgs(e, bounds.Size, element.Children);

                Box nb = CalcElementBounds(e, ea);
                nb = lm.GetBounds(ea, nb, lmi);
                
                boxes[i] = nb;
            }
            // better solution coming later
            i = -1;
            foreach (IElement e in element.Children)
            {
                i++;
                if (!e.Properties.Visable) { continue; }
                
                Box nb = boxes[i].Shifted(lmi.ChildOffset);
                
                // No change in bounds
                if (e.Properties.bounds == nb)
                {
                    element.Properties.PushViewBox(e.GetRenderBounds(), Box.Infinity);
                    continue;
                }

                e.OnSizeChange(new VectorEventArgs(nb.Size));
                e.OnElementMove(new VectorEventArgs(nb.Location));

                if (e.Graphics == null)
                {
                    element.Properties.PushViewBox(e.Properties.bounds, Box.Infinity);
                }
            }

            bounds.Size = lmi.ReturningSize;
            return bounds;
        }

        internal static void RecalculateScrollBounds(UIProperties elementProp)
        {
            elementProp.scrollViewBox = new Box();

            foreach (IElement e in elementProp.Source.Children)
            {
                elementProp.PushViewBox(e.GetRenderBounds(), Box.Infinity);
            }
        }

        protected void SetFrameSize(Vector2 size)
        {
            if (size.X <= 0 || size.Y <= 0) { return; }

            _uiView.FrameSize = size;

            Window.GraphicsContext.Actions.Push(() =>
            {
                Framebuffer.Size = (Vector2I)size;
            });

            Box bounds = CalcElementBounds(Root, new LayoutArgs(Root, size, Elements));
            Root.Properties.bounds = bounds;
        }

        internal IElement renderFocus;
        public void OnRender(IDrawingContext context)
        {
            ScrollInfo scroll = Root.Properties.GetScrollInfo();

            _uiView.Scale = 1d;
            _uiView.Offset = 0d;
            _uiView.ScissorBox = new GLBox(0d, Framebuffer.Size);
            _uiView.ScissorOffset = new Vector2(
                scroll.Y ? Root.Properties.ScrollBar.Width : 0d,
                scroll.X ? Root.Properties.ScrollBar.Width : 0d);

            _uiView.View = new Box(0d, Framebuffer.Size);

            _uiView.DepthRange = new Vector2(0d, 1d);
            _uiView.DepthDivision = 1;
            _uiView.ChildDivision = 1;
            _uiView.SetDepth(0d);

            Animator.Invoke();

            DrawContext dm = new DrawContext(Framebuffer)
            {
                DepthState = _uiView,
                RenderState = new RenderState()
            };
            Framebuffer.Clear(BufferBit.Colour | BufferBit.Depth);
            Render(Root, dm);

            if (scroll.CanScroll)
            {
                _uiView.Scale = 1d;
                _uiView.Offset = 0d;
                _uiView.ScissorBox = new GLBox(0d, Framebuffer.Size);
                _uiView.ScissorOffset = Vector2.Zero;

                _uiView.DepthRange = new Vector2(0d, 1d);
                _uiView.DepthDivision = 1;
                _uiView.ChildDivision = 1;
                _uiView.SetDepth(1d);

                RenderScrollBars(dm, scroll, new Box(0d, Framebuffer.Size), Root);
            }

            _uiView.Scale = 1d;
            _uiView.Offset = 0d;
            _uiView.ScissorBox = new GLBox(0d, Framebuffer.Size);

            context.WriteFramebuffer(Framebuffer, BufferBit.Colour, TextureSampling.Nearest);
        }
        internal void SetRenderSize(Vector2 size) => _uiView.Size = size;
        internal void SetRenderLocation(Vector2 pos) => _uiView.Location = pos;
        private void Render(IElement e, IDrawingContext dm)
        {
            renderFocus = e;

            dm.Projection = e.Graphics?.Projection;
            dm.View = Matrix.Identity;
            dm.Model = Matrix.Identity;
            dm.RenderState.Blending = true;
            dm.RenderState.SourceScaleBlending = BlendFunction.SourceAlpha;
            dm.RenderState.DestinationScaleBlending = BlendFunction.OneMinusSourceAlpha;
            e.Events.OnUpdate();
            dm.Render(e.Graphics);
            //e.Graphics?.OnRender(dm);

            renderFocus = null;

            if (!e.HasChildren) { return; }

            double scaleRef = _uiView.Scale * e.Properties.ViewScale;
            Vector2 offsetRef = _uiView.Offset + (e.Properties.bounds.Centre * e.Properties.ViewScale) + e.Properties.ViewPan;
            GLBox scissor = _uiView.PassScissor();

            Vector2 depthRange = _uiView.DepthRange;
            int depthDiv = _uiView.DepthDivision;
            int offset = e == Root ? 0 : 1;

            foreach (IElement child in e.Children)
            {
                if (!child.Properties.Visable) { continue; }

                _uiView.Scale = scaleRef;
                _uiView.Offset = offsetRef;
                _uiView.ScissorBox = scissor;

                ScrollInfo scroll = child.Properties.GetScrollInfo();

                _uiView.ScissorOffset = new Vector2(
                    scroll.Y ? child.Properties.ScrollBar.Width : 0d,
                    scroll.X ? child.Properties.ScrollBar.Width : 0d
                    );

                Box bounds = child.GetRenderBounds();
                _uiView.View = bounds;

                _uiView.DepthRange = depthRange;
                _uiView.DepthDivision = depthDiv;
                _uiView.ChildDivision = child.HasChildren ? child.Children.Length + 1 : 1;
                _uiView.SetDepth(child.Properties.Depth + offset);

                if (!_uiView.Visable) { continue; }

                Render(child, dm);

                if (!scroll.CanScroll) { continue; }

                _uiView.Scale = scaleRef;
                _uiView.Offset = offsetRef;
                _uiView.ScissorBox = scissor;
                _uiView.ScissorOffset = Vector2.Zero;

                _uiView.DepthRange = depthRange;
                _uiView.DepthDivision = depthDiv;
                _uiView.ChildDivision = child.HasChildren ? child.Children.Length + 1 : 1;
                _uiView.SetDepth(child.Properties.Depth + 1);

                RenderScrollBars(dm, scroll, bounds, child);
            }
        }
        private void RenderScrollBars(IDrawingContext dm, ScrollInfo scroll, Box bounds, IElement e)
        {
            double width = e.Properties.ScrollBar.Width;

            if (scroll.X)
            {
                bounds.Bottom += width;
            }

            Vector2 scrollPercent = e.Properties.GetScrollPercent();

            if (scroll.Y)
            {
                _uiView.View = new Box(bounds.Right - width, bounds.Right, bounds.Top, bounds.Bottom);
                dm.Projection = Matrix4.CreateOrthographic(width, bounds.Height, 0d, 1d);
                dm.View = Matrix.Identity;
                dm.Model = Matrix.Identity;
                dm.Render(e.Properties.ScrollBar,
                    new ScrollBarData(e,
                        scrollPercent.Y,
                        scroll.ViewSize.Y / scroll.ScrollView.Y,
                        true, bounds.Height));

                bounds.Right -= width;
            }
            if (scroll.X)
            {
                bounds.Bottom -= width;

                _uiView.View = new Box(bounds.Left, bounds.Right, bounds.Bottom + width, bounds.Bottom);
                dm.Projection = Matrix4.CreateOrthographic(bounds.Width, width, 0d, 1d);
                dm.View = Matrix.Identity;
                dm.Model = Matrix.Identity;
                dm.Render(e.Properties.ScrollBar,
                    new ScrollBarData(e,
                        scrollPercent.X,
                        scroll.ViewSize.X / scroll.ScrollView.X,
                        false, bounds.Width));
            }
        }
    }
}
