using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class FlatButton : Button
    {
        private Color _bgColor;
        private Color _hoverColor;

        public FlatButton(string text, Color bg, Color? hover = null)
        {
            _bgColor = bg;
            _hoverColor = hover ?? ControlPaint.Light(bg, 0.2f);

            Text = text;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = _bgColor;
            ForeColor = Theme.TextPri;
            Font = Theme.FontBtn;
            Cursor = Cursors.Hand;
            Height = 36;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            BackColor = _hoverColor;
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            BackColor = _bgColor;
            base.OnMouseLeave(e);
        }
    }

    public class CardPanel : Panel
    {
        private Color _borderColor;

        public CardPanel(Color? border = null)
        {
            _borderColor = border ?? Theme.Border;
            BackColor = Theme.Card;
            Padding = new Padding(12);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(_borderColor, 1))
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }

    public class DarkTextBox : TextBox
    {
        public DarkTextBox()
        {
            BackColor = Theme.InputBg;
            ForeColor = Theme.TextPri;
            BorderStyle = BorderStyle.FixedSingle;
            Font = Theme.FontMono;
        }
    }

    public class DarkComboBox : ComboBox
    {
        public DarkComboBox()
        {
            BackColor = Theme.InputBg;
            ForeColor = Theme.TextPri;
            FlatStyle = FlatStyle.Flat;
            Font = Theme.FontSub;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }

    public class LabelSmall : Label
    {
        public LabelSmall(string text)
        {
            Text = text;
            ForeColor = Theme.TextSec;
            Font = Theme.FontLabel;
            AutoSize = true;
        }
    }
}