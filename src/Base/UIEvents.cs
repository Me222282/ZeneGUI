using System;
using Zene.Windowing;

namespace Zene.GUI
{
    public sealed class UIEvents
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
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));

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

        private readonly IElement _source;

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

        internal void OnTextInput(TextInputEventArgs e)
        {
            TextInput?.Invoke(_source, e);
            _textInput(e);
        }
        internal void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(_source, e);
            _keyDown(e);
        }
        internal void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(_source, e);
            _keyUp(e);
        }

        internal void OnScroll(ScrollEventArgs e)
        {
            Scroll?.Invoke(_source, e);
            _scroll(e);
        }

        internal void OnMouseEnter(EventArgs e)
        {
            _source.Properties.hover = true;

            MouseEnter?.Invoke(_source, e);
            _mouseEnter(e);
        }
        internal void OnMouseLeave(EventArgs e)
        {
            _source.Properties.hover = false;

            MouseLeave?.Invoke(_source, e);
            _mouseLeave(e);
        }
        internal void OnMouseMove(MouseEventArgs e)
        {
            if (_source.Properties.mousePos == e.Location) { return; }

            _source.Properties.mousePos = e.Location;

            MouseMove?.Invoke(_source, e);
            _mouseMove(e);
        }
        internal void OnMouseDown(MouseEventArgs e)
        {
            _source.Properties.selected = true;

            MouseDown?.Invoke(_source, e);
            _mouseDown(e);
        }
        internal void OnMouseUp(MouseEventArgs e)
        {
            if (_source.Properties.Window.MouseButton == MouseButton.None)
            {
                _source.Properties.selected = false;
            }

            MouseUp?.Invoke(_source, e);
            _mouseUp(e);
        }

        internal void OnSizeChange(VectorEventArgs e)
        {
            if (_source.Properties.bounds.Size == e.Value) { return; }

            _source.Properties.bounds.Size = e.Value;

            SizeChange?.Invoke(_source, e);
            _sizeChange(e);

            _source.Graphics.ChangeSize(e);
        }
        internal void OnElementMove(VectorEventArgs e)
        {
            if (_source.Properties.bounds.Centre == e.Value) { return; }

            _source.Properties.bounds.Centre = e.Value;

            ElementMove?.Invoke(_source, e);
            _elementMove(e);

            _source.Graphics.MoveElement(e);
        }

        internal void OnUpdate()
        {
            Update?.Invoke(_source, EventArgs.Empty);
            _update(EventArgs.Empty);
        }
        internal void OnFocus(FocusedEventArgs e)
        {
            _source.Properties.focus = e.Focus;

            Focus?.Invoke(_source, e);
            _focus(e);
        }
    }
}
