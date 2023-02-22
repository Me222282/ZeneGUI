using System;
using Zene.Windowing;

namespace Zene.GUI
{
    public sealed class UIEvents : EventListener
    {
        public UIEvents(
            IElement source,
            Action<TextInputEventArgs> textInput,
            Action<KeyEventArgs> keyDown,
            Action<KeyEventArgs> keyUp,
            Action<ScrollEventArgs> scroll,
            Action<EventArgs> mouseEnter,
            Action<EventArgs> mouseLeave,
            Action<MouseEventArgs> mouseMove,
            Action<MouseEventArgs> mouseDown,
            Action<MouseEventArgs> mouseUp,
            Action<VectorEventArgs> sizeChange,
            Action<VectorEventArgs> elementMove,
            Action<EventArgs> update,
            Action<FocusedEventArgs> focus)
            : base(source)
        {
            _textInput = textInput ?? throw new ArgumentNullException(nameof(textInput));
            _keyDown = keyDown ?? throw new ArgumentNullException(nameof(keyDown));
            _keyUp = keyUp ?? throw new ArgumentNullException(nameof(keyUp));
            _scroll = scroll ?? throw new ArgumentNullException(nameof(scroll));
            _mouseEnter = mouseEnter ?? throw new ArgumentNullException(nameof(mouseEnter));
            _mouseLeave = mouseLeave ?? throw new ArgumentNullException(nameof(mouseLeave));
            _mouseMove = mouseMove ?? throw new ArgumentNullException(nameof(mouseMove));
            _mouseDown = mouseDown ?? throw new ArgumentNullException(nameof(mouseDown));
            _mouseUp = mouseUp ?? throw new ArgumentNullException(nameof(mouseUp));
            _sizeChange = sizeChange ?? throw new ArgumentNullException(nameof(sizeChange));
            _elementMove = elementMove ?? throw new ArgumentNullException(nameof(elementMove));
            _update = update ?? throw new ArgumentNullException(nameof(update));
            _focus = focus ?? throw new ArgumentNullException(nameof(focus));
        }

        public event TextInputEventHandler TextInput;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        public event ScrolEventHandler Scroll;

        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;

        public event VectorEventHandler SizeChange;
        public event VectorEventHandler ElementMove;

        public event EventHandler Update;
        public event FocusedEventHandler Focus;

        private readonly Action<TextInputEventArgs> _textInput;
        private readonly Action<KeyEventArgs> _keyDown;
        private readonly Action<KeyEventArgs> _keyUp;
        private readonly Action<ScrollEventArgs> _scroll;
        private readonly Action<EventArgs> _mouseEnter;
        private readonly Action<EventArgs> _mouseLeave;
        private readonly Action<MouseEventArgs> _mouseMove;
        private readonly Action<MouseEventArgs> _mouseDown;
        private readonly Action<MouseEventArgs> _mouseUp;
        private readonly Action<VectorEventArgs> _sizeChange;
        private readonly Action<VectorEventArgs> _elementMove;
        private readonly Action<EventArgs> _update;
        private readonly Action<FocusedEventArgs> _focus;

        protected internal override void OnTextInput(TextInputEventArgs e)
        {
            TextInput?.Invoke(source, e);
            _textInput(e);
        }
        protected internal override void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(source, e);
            _keyDown(e);
        }
        protected internal override void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(source, e);
            _keyUp(e);
        }

        protected internal override void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(source, e);
            _scroll(e);
        }

        protected internal override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            MouseEnter?.Invoke(source, e);
            _mouseEnter(e);
        }
        protected internal override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            MouseLeave?.Invoke(source, e);
            _mouseLeave(e);
        }
        protected internal override void OnMouseMove(MouseEventArgs e)
        {
            if (source.Properties.mousePos == e.Location) { return; }

            base.OnMouseMove(e);

            MouseMove?.Invoke(source, e);
            _mouseMove(e);
        }
        protected internal override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            MouseDown?.Invoke(source, e);
            _mouseDown(e);
        }
        protected internal override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            MouseUp?.Invoke(source, e);
            _mouseUp(e);
        }

        protected internal override void OnSizeChange(VectorEventArgs e)
        {
            if (source.Properties.bounds.Size == e.Value) { return; }

            base.OnSizeChange(e);

            SizeChange?.Invoke(source, e);
            _sizeChange(e);
        }
        protected internal override void OnElementMove(VectorEventArgs e)
        {
            if (source.Properties.bounds.Centre == e.Value) { return; }

            base.OnElementMove(e);

            ElementMove?.Invoke(source, e);
            _elementMove(e);
        }

        protected internal override void OnUpdate()
        {
            Update?.Invoke(source, EventArgs.Empty);
            _update(EventArgs.Empty);
        }
        protected internal override void OnFocus(FocusedEventArgs e)
        {
            base.OnFocus(e);

            Focus?.Invoke(source, e);
            _focus(e);
        }
    }
}
