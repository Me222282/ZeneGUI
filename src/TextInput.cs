using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zene.Graphics;
using Zene.Structs;
using Zene.Windowing;

namespace Zene.GUI
{
    public class TextInput : TextElement
    {
        public TextInput(IBox bounds)
            : base(bounds, false)
        {
        }

        public TextInput(ILayout layout)
            : base(layout, false)
        {
            
        }

        public override BorderShader Shader { get; } = BorderShader.GetInstance();

        public double BorderWidth { get; set; } = 1;
        public ColourF BorderColour { get; set; } = new ColourF(1f, 1f, 1f);
        public ColourF BackgroundColour { get; set; }
        public double CornerRadius { get; set; } = 0d;

        private int _caret = 0;
        private StringBuilder _text = new StringBuilder();
        protected override string TextReference
        {
            get => _text.ToString() + '|';
            set => _text = new StringBuilder(value);
        }

        private double _timeOffset = 0;
        private bool DrawCaret
        {
            get => (int)((_window.Timer - _timeOffset) * 2) % 2 == 0;
        }

        private void ResetCaret() => _timeOffset = _window.Timer;

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);

            _text.Insert(_caret, e.Character);
            TriggerLayout();
            ResetCaret();
            _caret++;
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e[Keys.V] && e[Mods.Control])
            {
                string paste = _window.ClipBoard;
                _text.Insert(_caret, paste);
                TriggerLayout();
                ResetCaret();
                _caret += paste.Length;
                return;
            }
            if (e[Keys.BackSpace])
            {
                if (_caret < 1) { return; }

                int offset = CaretLeft();

                _text.Remove(_caret + offset, -offset);
                TriggerLayout();
                ResetCaret();
                _caret += offset;
                return;
            }
            if (e[Keys.Delete])
            {
                if (_caret >= _text.Length) { return; }

                _text.Remove(_caret, 1);
                TriggerLayout();
                ResetCaret();
                return;
            }
            if (e[Keys.Enter])
            {
                _text.Insert(_caret, '\n');
                TriggerLayout();
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

        protected override void OnUpdate(FrameEventArgs e)
        {
            base.OnUpdate(e);

            Shader.BorderWidth = BorderWidth;
            DrawingBoundOffset = Shader.BorderWidth;

            Shader.BorderColour = BorderColour;
            Shader.Radius = CornerRadius;

            Shader.ColourSource = ColourSource.UniformColour;
            Shader.Colour = BackgroundColour;

            Shader.Matrix3 = Projection;
            Shader.Size = Size;
            Shader.Matrix1 = Matrix4.CreateScale(Bounds.Size);
            Shapes.Square.Draw();

            if (Font == null || Text == null) { return; }

            TextRenderer.Model = Matrix4.CreateScale(TextSize);
            TextRenderer.Colour = TextColour;

            if (_text.Length < 1 && DrawCaret)
            {
                TextRenderer.DrawLeftBound("|", Font, 0, 0, -1, false);
                return;
            }

            TextRenderer.DrawLeftBound(_text.ToString(), Font, CharSpace, LineSpace, _caret, DrawCaret);
        }
    }
}
