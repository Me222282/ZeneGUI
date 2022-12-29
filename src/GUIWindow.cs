using System;
using Zene.Graphics;
using Zene.Windowing;

namespace Zene.GUI
{
    public class GUIWindow : Window
    {
        public GUIWindow(int width, int height, string title, WindowInitProperties properties = null)
            : this(width, height, title, 4.5, true, properties)
        {
        }
        public GUIWindow(int width, int height, string title, double version, WindowInitProperties properties = null)
            : this(width, height, title, version, true, properties)
        {
        }
        protected GUIWindow(int width, int height, string title, bool multithreading, WindowInitProperties properties = null)
            : this(width, height, title, 4.5, multithreading, properties)
        {
        }
        protected GUIWindow(int width, int height, string title, double version, bool multithreading, WindowInitProperties properties = null)
            : base(width, height, title, version, multithreading, properties)
        {
            RootElement = new RootElement(this, false);

            State.Blending = true;
            State.SourceScaleBlending = BlendFunction.SourceAlpha;
            State.DestinationScaleBlending = BlendFunction.OneMinusSourceAlpha;
        }

        /// <summary>
        /// The root element of the GUI.
        /// </summary>
        public RootElement RootElement { get; }

        public Element FocusElement => RootElement.FocusElement;
        public Element HoverElement => RootElement.Hover;

        public Element this[int index] => RootElement[index];

        public void LoadXml(string xml)
        {
            Xml loader = new Xml();
            loader.LoadGUI(RootElement, xml);
        }
        public void LoadXml(string xml, object events)
        {
            Xml loader = new Xml();

            loader.LoadGUI(RootElement, xml, events);
        }
        public void LoadXml(string xml, Type events)
        {
            Xml loader = new Xml();

            loader.LoadGUI(RootElement, xml, events);
        }

        public void AddChild(Element e) => RootElement.AddChild(e);
        public bool RemoveChild(Element e) => RootElement.RemoveChild(e);
        public void ClearChildren() => RootElement.ClearChildren();

        public T Find<T>() where T : Element
            => RootElement.Find<T>();

        protected override void OnUpdate(EventArgs e)
        {
            base.OnUpdate(e);

            BaseFramebuffer.Clear(BufferBit.Colour | BufferBit.Depth);

            RootElement.Render();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            RootElement.MouseMove(this, e);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            RootElement.OnMouseDown(new MouseEventArgs(RootElement.MouseLocation, e.Button, e.Modifier));
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            RootElement.OnMouseUp(new MouseEventArgs(RootElement.MouseLocation, e.Button, e.Modifier));
        }
        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);

            RootElement.OnScroll(e);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            RootElement.OnKeyDown(e);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            RootElement.FocusElement?.OnKeyUp(e);
        }
        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            RootElement.FocusElement?.OnTextInput(e);
        }
        protected override void OnSizePixelChange(VectorIEventArgs e)
        {
            base.OnSizePixelChange(e);

            RootElement.SizeChangeListener((VectorEventArgs)e);
        }

        protected override void OnStart(EventArgs e)
        {
            base.OnStart(e);

            RootElement.SizeChangeListener(new VectorEventArgs(Size));
        }
    }
}
