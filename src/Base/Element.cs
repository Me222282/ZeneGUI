using System;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public abstract class Element : IElement
    {
        public Element()
        {
            Properties = new UIProperties(this);

            Events = new UIEvents(this,
                OnTextInput,
                OnKeyDown,
                OnKeyUp,
                OnScroll,
                OnMouseEnter,
                OnMouseLeave,
                OnMouseMove,
                OnMouseDown,
                OnMouseUp,
                OnSizeChange,
                OnElementMove,
                OnUpdate,
                OnFocus);
        }
        public Element(ILayout layout)
            : this()
        {
            Layout = layout;
        }

        public UIProperties Properties { get; }
        public virtual ElementList Children { get; }
        public IElement Parent => Properties.parent;

        public string Id { get; set; }

        private ILayout _layout = FixedLayout.Default;
        public ILayout Layout
        {
            get => _layout;
            set
            {
                if (value == null)
                {
                    value = FixedLayout.Default;
                }

                if (_layout != null)
                {
                    _layout.Change -= Properties.OnLayoutChange;
                }

                _layout = value;

                _layout.Change += Properties.OnLayoutChange;

                TriggerChange();
            }
        }
        private ILayoutManager _layoutManager = GUI.LayoutManager.Empty;
        public ILayoutManager LayoutManager
        {
            get => _layoutManager;
            set
            {
                if (value == null)
                {
                    value = GUI.LayoutManager.Empty;
                }

                if (_layoutManager != null)
                {
                    _layoutManager.Change -= Properties.OnLayoutChange;
                }

                _layoutManager = value;

                _layoutManager.Change += Properties.OnLayoutChange;

                TriggerChange();
            }
        }

        public UIEvents Events { get; }
        EventListener IElement.Events => Events;
        public abstract GraphicsManager Graphics { get; }

        protected ActionManager Actions => Properties.handle.Window.GraphicsContext.Actions;
        public Window Window => Properties.handle.Window;
        public UIManager Hande => Properties.handle;
        public IElement RootElement => Properties.handle.Root;

        public bool MouseSelect => Properties.selected;
        public bool MouseHover => Properties.hover;
        public bool Focused => Properties.focus;

        public Box Bounds => Properties.bounds;
        public Vector2 Size => Properties.bounds.Size;
        public Vector2 Location => Properties.bounds.Location;

        public bool HasChildren => Children != null && Children.Length > 0;
        public bool IsParent => Children != null;
        public bool HasParent => Properties.parent != null;

        public Vector2 MouseLocation => Properties.mousePos;

        public int CurrentIndex => Properties.elementIndex;

        /// <summary>
        /// The style of the cursor when hovering over this element.
        /// </summary>
        public Cursor CursorStyle
        {
            get => Properties.CursorStyle;
            set => Properties.CursorStyle = value;
        }

        public bool Visable
        {
            get => Properties.Visable;
            set => Properties.Visable = value;
        }

        /// <summary>
        /// Determines whether this element can become the hover element or focus element.
        /// Child elements are not affected.
        /// </summary>
        public bool Ineractable
        {
            get => Properties.Interactable;
            set => Properties.Interactable = value;
        }

        public bool TabShifting
        {
            get => Properties.TabShifting;
            set => Properties.TabShifting = value;
        }

        /// <summary>
        /// Determines whether <paramref name="key"/> is currently being pressed.
        /// </summary>
        /// <param name="key">THe key to query</param>
        /// <returns></returns>
        public bool this[Keys key] => Properties.focus && Properties.Window[key];
        /// <summary>
        /// Determines whether <paramref name="mod"/> is currently active.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="NotSupportedException"/> if <paramref name="mod"/> is <see cref="Mods.CapsLock"/> or <see cref="Mods.NumLock"/>.
        /// </remarks>
        /// <param name="mod">The modifier to query.</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public bool this[Mods mod] => Properties.Window[mod];
        /// <summary>
        /// Determines whether <paramref name="button"/> is currently pressed.
        /// </summary>
        /// <param name="button">The mouse button to query.</param>
        /// <returns></returns>
        public bool this[MouseButton button] => Properties.selected && Properties.Window[button];

        public Vector2 ViewPan
        {
            get => Properties.ViewPan;
            set => Properties.ViewPan = value;
        }
        public double ViewScale
        {
            get => Properties.ViewScale;
            set => Properties.ViewScale = value;
        }

        public event TextInputEventHandler TextInput
        {
            add => Events.TextInput += value;
            remove => Events.TextInput -= value;
        }
        public event KeyEventHandler KeyDown
        {
            add => Events.KeyDown += value;
            remove => Events.KeyDown -= value;
        }
        public event KeyEventHandler KeyUp
        {
            add => Events.KeyUp += value;
            remove => Events.KeyUp -= value;
        }
        public event ScrolEventHandler Scroll
        {
            add => Events.Scroll += value;
            remove => Events.Scroll -= value;
        }
        public event EventHandler MouseEnter
        {
            add => Events.MouseEnter += value;
            remove => Events.MouseEnter -= value;
        }
        public event EventHandler MouseLeave
        {
            add => Events.MouseLeave += value;
            remove => Events.MouseLeave -= value;
        }

        public event MouseEventHandler MouseMove
        {
            add => Events.MouseMove += value;
            remove => Events.MouseMove -= value;
        }
        public event MouseEventHandler MouseDown
        {
            add => Events.MouseDown += value;
            remove => Events.MouseDown -= value;
        }
        public event MouseEventHandler MouseUp
        {
            add => Events.MouseUp += value;
            remove => Events.MouseUp -= value;
        }
        public event MouseEventHandler Click
        {
            add => Events.MouseUp += value;
            remove => Events.MouseUp -= value;
        }

        public event VectorEventHandler SizeChange
        {
            add => Events.SizeChange += value;
            remove => Events.SizeChange -= value;
        }
        public event VectorEventHandler ElementMove
        {
            add => Events.ElementMove += value;
            remove => Events.ElementMove -= value;
        }

        public event EventHandler Update
        {
            add => Events.Update += value;
            remove => Events.Update -= value;
        }
        public event FocusedEventHandler Focus
        {
            add => Events.Focus += value;
            remove => Events.Focus -= value;
        }

        public virtual bool IsMouseHover(Vector2 mousePos) => Properties.bounds.Contains(mousePos);

        protected void TriggerChange()
        {
            if (!Properties.Visable || Properties.handle == null)
            {
                return;
            }

            if (HasParent && Properties.parent.LayoutManager.ChildDependent)
            {
                Properties.handle?.LayoutElement(Properties.parent);
                return;
            }

            Properties.handle?.LayoutElement(this);
        }

        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            
        }

        protected virtual void OnScroll(ScrollEventArgs e)
        {
            
        }

        protected virtual void OnMouseEnter(EventArgs e)
        {
            
        }
        protected virtual void OnMouseLeave(EventArgs e)
        {
            
        }
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            
        }
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            
        }
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            
        }

        protected virtual void OnSizeChange(VectorEventArgs e)
        {
            
        }
        protected virtual void OnElementMove(VectorEventArgs e)
        {
            
        }

        protected virtual void OnUpdate(EventArgs e)
        {
            
        }
        protected virtual void OnFocus(FocusedEventArgs e)
        {
            
        }
    }
}
