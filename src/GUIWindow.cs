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
            RootElement = new RootElement(this);

            State.Blending = true;
            State.SourceScaleBlending = BlendFunction.SourceAlpha;
            State.DestinationScaleBlending = BlendFunction.OneMinusSourceAlpha;
        }

        /// <summary>
        /// The root element of the GUI.
        /// </summary>
        public RootElement RootElement { get; }

        public IElement FocusElement => RootElement.Focus;
        public IElement HoverElement => RootElement.Hover;

        public IElement this[int index] => RootElement.Elements[index];

        public void LoadXml(string xml)
        {
            Xml loader = new Xml();
            loader.LoadGUI(RootElement.Elements, xml);
        }
        public void LoadXml(string xml, object events)
        {
            Xml loader = new Xml();

            loader.LoadGUI(RootElement.Elements, xml, events);
        }
        public void LoadXml(string xml, Type events)
        {
            Xml loader = new Xml();

            loader.LoadGUI(RootElement.Elements, xml, events);
        }

        public void AddChild(IElement e) => RootElement.Elements.Add(e);
        public bool RemoveChild(IElement e) => RootElement.Elements.Remove(e);
        public void ClearChildren() => RootElement.Elements.Clear();

        public T Find<T>() where T : class, IElement
            => RootElement.Elements.Find<T>();

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            BaseFramebuffer.Clear(BufferBit.Colour | BufferBit.Depth);

            DrawContext.Render(RootElement);
        }
    }
}
