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
            if (root.Properties.elementIndex >= 0)
            {
                throw new ArgumentException("The given root is a child of another element.", nameof(root));
            }

            Root = root ?? throw new ArgumentNullException(nameof(root));
            Elements = new ElementList(root);

            Root.Properties.handle = this;
            Root.Properties.parent = null;

            Hover = Root;
            _focus = Root;

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

            Root = root ?? throw new ArgumentNullException(nameof(root));
            Elements = new ElementList(root);
            Root.Properties.handle = this;
            Root.Properties.parent = null;

            Hover = Root;
            _focus = Root;
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

                if (_focus == value) { return; }

                _focus.OnFocus(false);
                _focus = value;
                _focus.OnFocus(true);
            }
        }

        private readonly UIView _uiView;
        public TextureRenderer Framebuffer { get; }
        public TextRenderer TextRenderer { get; }

        public Animator Animator { get; }

        public void Animate<T>(AnimatorData<T> data) => Animator.Add(data);

        public void ShiftFocusRight()
        {
            if (!Focus.HasParent)
            {
                Focus = Focus.LowestFirstElement();
                return;
            }

            Focus = Focus.NextElement();
        }
        public void ShiftFocusLeft()
        {
            if (!Focus.HasParent)
            {
                Focus = Focus.LowestLastElement();
            }

            Focus = Focus.PreviousElement();
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
            Vector2 localMouse = (e.Location - Root.Properties.ViewPan) / Root.Properties.ViewScale;

            // Find subhover element
            while (hover.HasChildren)
            {
                IElement nh = ManagerMouseMove(hover.Children, localMouse);

                // Not hovering over any child elements
                if (nh == null) { break; }

                hover = nh;
                localMouse -= nh.Properties.bounds.Centre;
                localMouse = (localMouse - nh.Properties.ViewPan) / nh.Properties.ViewScale;
            }
            if (!hover.Properties.Interactable)
            {
                uninteractHover = hover;
                // Finds lowest element in hover tree that can be a hover element.
                hover = FindHoverElement(hover, ref localMouse);
            }

            if (Hover != hover)
            {
                Hover.OnMouseLeave();
                hover.OnMouseEnter();

                Hover = hover;
                Window.CursorStyle = hover.Properties.CursorStyle;
            }

            if (hover == Root) { return; }

            hover.OnMouseMove(new MouseEventArgs(localMouse, e.Button, e.Modifier));
        }
        private void ManageMouseSelect(MouseEventArgs e)
        {
            IElement parent = Hover.HasParent ? Hover.Properties.parent : Hover;

            Vector2 localMouse = parent.CalculateLocalMouse(e.Location);
            bool mouseOver = Hover.IsMouseHover(localMouse);

            // Calculate local mouse for hover element from parent
            localMouse = Hover.CalculateLocalMouse(parent, localMouse);

            if (mouseOver && !Hover.Properties.hover)
            {
                Hover.OnMouseEnter();
            }
            else if (!mouseOver)
            {
                Hover.OnMouseLeave();
            }

            Hover.OnMouseMove(new MouseEventArgs(localMouse, e.Button, e.Modifier));
        }
        private static IElement ManagerMouseMove(ElementList el, Vector2 mousePos)
        {
            if (el == null) { return null; }

            foreach (IElement e in el)
            {
                if (!e.Properties.Visable) { continue; }

                if (e.IsMouseHover(mousePos))
                {
                    return e;
                }
            }

            return null;
        }
        private IElement FindHoverElement(IElement lowest, ref Vector2 mousePos)
        {
            if (!lowest.HasParent || lowest.Properties.Interactable)
            {
                return lowest;
            }

            mousePos = (mousePos * lowest.Properties.ViewScale) + lowest.Properties.ViewPan + lowest.Properties.bounds.Centre;
            return FindHoverElement(lowest.Properties.parent, ref mousePos);
        }

        protected void TriggerChange()
        {
            CalcElementBounds(Root, new LayoutArgs(Root, Root.Properties.bounds.Size, Elements));
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

            foreach (IElement e in element.Children)
            {
                if (!e.Properties.Visable) { continue; }

                LayoutArgs ea = new LayoutArgs(e, bounds.Size, element.Children);

                Box nb = CalcElementBounds(e, ea);
                nb = lm.GetBounds(ea, nb, lmi);

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

            Animator.Invoke();

            DrawManager dm = new DrawManager(Framebuffer);
            Framebuffer.Clear(BufferBit.Colour | BufferBit.Depth);
            Render(Root, dm);

            if (scroll.CanScroll)
            {
                _uiView.Scale = 1d;
                _uiView.Offset = 0d;
                _uiView.ScissorBox = new GLBox(0d, Framebuffer.Size);
                _uiView.ScissorOffset = Vector2.Zero;

                RenderScrollBars(dm, scroll, new Box(0d, Framebuffer.Size), Root);
            }

            _uiView.Scale = 1d;
            _uiView.Offset = 0d;
            _uiView.ScissorBox = new GLBox(0d, Framebuffer.Size);

            context.WriteFramebuffer(Framebuffer, BufferBit.Colour, TextureSampling.Nearest);
        }
        internal void SetRenderSize(Vector2 size) => _uiView.Size = size;
        internal void SetRenderLocation(Vector2 pos) => _uiView.Location = pos;
        private void Render(IElement e, DrawManager dm)
        {
            // Set some drawing properties
            State.DepthTesting = false;
            TextRenderer.Projection = e.Graphics?.Projection;
            TextRenderer.View = Matrix.Identity;
            TextRenderer.Model = Matrix.Identity;

            renderFocus = e;

            dm.Projection = e.Graphics?.Projection;
            dm.View = Matrix.Identity;
            dm.Model = Matrix.Identity;
            e.Events.OnUpdate();
            dm.Render(e.Graphics);
            //e.Graphics?.OnRender(dm);

            renderFocus = null;

            if (!e.HasChildren) { return; }

            double scaleRef = _uiView.Scale * e.Properties.ViewScale;
            Vector2 offsetRef = _uiView.Offset + (e.Properties.bounds.Centre * e.Properties.ViewScale) + e.Properties.ViewPan;
            GLBox scissor = _uiView.PassScissor();

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

                if (!_uiView.Visable) { continue; }

                Render(child, dm);

                if (!scroll.CanScroll) { continue; }

                _uiView.Scale = scaleRef;
                _uiView.Offset = offsetRef;
                _uiView.ScissorBox = scissor;
                _uiView.ScissorOffset = Vector2.Zero;

                RenderScrollBars(dm, scroll, bounds, child);
            }
        }
        private void RenderScrollBars(DrawManager dm, ScrollInfo scroll, Box bounds, IElement e)
        {
            double width = e.Properties.ScrollBar.Width;

            if (scroll.X)
            {
                bounds.Bottom += width;
            }

            if (scroll.Y)
            {
                _uiView.View = new Box(bounds.Right - width, bounds.Right, bounds.Top, bounds.Bottom);
                dm.Projection = Matrix4.CreateOrthographic(width, bounds.Height, 0d, 1d);
                dm.View = Matrix.Identity;
                dm.Model = Matrix.Identity;
                dm.Render(e.Properties.ScrollBar,
                    new ScrollBarData(e,
                        e.Properties.ViewPan.Y.InvLerp(-e.Properties.scrollBounds.Top, -e.Properties.scrollBounds.Bottom),
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
                        e.Properties.ViewPan.X.InvLerp(-e.Properties.scrollBounds.Right, -e.Properties.scrollBounds.Left),
                        scroll.ViewSize.X / scroll.ScrollView.X,
                        false, bounds.Width));
            }
        }
    }
}
