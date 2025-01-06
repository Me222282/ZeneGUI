using System;
using System.Text;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class TextInput : TextElement
    {
        public TextInput()
        {
            Graphics = new Renderer(this);
        }
        public TextInput(TextLayout layout)
            : base(layout)
        {
            layout.TextInput = true;
            Graphics = new Renderer(this);
        }

        public floatv BorderWidth { get; set; } = 1;
        public ColourF BorderColour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BackgroundColour { get; set; }
        public floatv CornerRadius { get; set; } = 0.01f;
        public bool SingleLine { get; set; } = false;

        private int _caret = 0;
        private StringBuilder _text = new StringBuilder();
        protected override string TextReference
        {
            get => _text.ToString();
            set
            {
                _text = new StringBuilder(value);

                _caret = Math.Clamp(_caret, 0, _text.Length);
            }
        }

        private double _timeOffset = 0;
        private bool DrawCaret
        {
            get => (int)((Core.Time - _timeOffset) * 2) % 2 == 0;
        }

        public override GraphicsManager Graphics { get; }

        private void ResetCaret() => _timeOffset = Core.Time;

        protected override void OnTextInput(TextInputEventArgs e)
        {
            _text.Insert(_caret, e.Character);
            TriggerChange();
            ResetCaret();
            _caret++;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e[Keys.V] && e[Mods.Control])
            {
                string paste = Window.ClipBoard;
                _text.Insert(_caret, paste);
                TriggerChange();
                ResetCaret();
                _caret += paste.Length;
                return;
            }
            if (e[Keys.BackSpace])
            {
                if (_caret < 1) { return; }

                int offset = CaretLeft();

                _text.Remove(_caret + offset, -offset);
                TriggerChange();
                ResetCaret();
                _caret += offset;
                return;
            }
            if (e[Keys.Delete])
            {
                if (_caret >= _text.Length) { return; }

                _text.Remove(_caret, 1);
                TriggerChange();
                ResetCaret();
                return;
            }
            if (!SingleLine && (e[Keys.Enter] || e[Keys.NumPadEnter]))
            {
                _text.Insert(_caret, '\n');
                TriggerChange();
                ResetCaret();
                _caret++;
                return;
            }
            if (e[Keys.Left])
            {
                _caret += CaretLeft();
                ResetCaret();
                if (_caret < 0)
                {
                    _caret = 0;
                }
                return;
            }
            if (e[Keys.Right])
            {
                _caret += CaretRight();
                ResetCaret();
                if (_caret > _text.Length)
                {
                    _caret = _text.Length;
                }
                return;
            }
        }

        private int CaretLeft()
        {
            if (!this[Mods.Control])
            {
                return -1;
            }

            int i = _caret - 1;
            if (i <= 0) { return -1; }

            char eg = _text[i];
            i--;
            while (i >= 0)
            {
                char current = _text[i];

                if (!Compare(eg, current)) { break; }

                eg = current;

                i--;
            }

            return i + 1 - _caret;
        }

        private int CaretRight()
        {
            if (!this[Mods.Control])
            {
                return 1;
            }

            int i = _caret + 1;
            if (i >= _text.Length) { return 1; }

            char eg = _text[_caret];
            while (i < _text.Length)
            {
                char current = _text[i];

                if (!Compare(eg, current)) { break; }
                eg = current;

                i++;
            }

            return i - _caret;
        }
        private static bool Compare(char a, char b)
        {
            if (a == '\n' || a == '\r')
            {
                return false;
            }

            if (char.IsLetterOrDigit(a))
            {
                return char.IsLetterOrDigit(b);
            }
            if (char.IsWhiteSpace(a))
            {
                return char.IsLetterOrDigit(b) || char.IsWhiteSpace(b);
            }

            return false;
        }

        private floatv BorderWidthDraw()
        {
            if (Focused)
            {
                return BorderWidth + 1;
            }

            return BorderWidth;
        }

        protected override void OnFocus(FocusedEventArgs e) => ResetCaret();

        private class Renderer : GraphicsManager<TextInput>
        {
            public Renderer(TextInput source)
                : base(source)
            {

            }

            public override void OnRender(IDrawingContext context)
            {
                floatv borderWidth = Math.Max(Source.BorderWidthDraw(), 0);
                Size = Source.Size + borderWidth;

                // No point drawing box
                if (Source.BackgroundColour.A <= 0f && (Source.BorderColour.A <= 0f || borderWidth <= 0))
                {
                    goto DrawText;
                }

                context.DrawBorderBox(new Box(Vector2.Zero, Source.Bounds.Size), Source.BackgroundColour, borderWidth, Source.BorderColour, Source.CornerRadius);

            DrawText:

                if (Source.Font == null || Source.Text == null) { return; }

                context.Model = Matrix4.CreateScale(Source.TextSize);
                TextRenderer.Colour = Source.TextColour;

                if (Source._text.Length < 1 && Source.DrawCaret && Source.Focused)
                {
                    TextRenderer.DrawLeftBound(context, "|", Source.Font, 0, 0, -1, false);
                    return;
                }

                TextRenderer.DrawLeftBound(context, Source.TextReference, Source.Font, Source.CharSpace, Source.LineSpace, Source._caret, Source.DrawCaret && Source.Focused);
            }
        }
    }
}
