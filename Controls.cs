using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGP
{
    public class FlatBtn : Button
    {
        private Color _bg, _hov;
        public FlatBtn(string text, Color bg)
        {
            _bg = bg; _hov = ControlPaint.Light(bg, 0.18f);
            Text = text; FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = _bg; ForeColor = Theme.TextPri;
            Font = Theme.Btn; Cursor = Cursors.Hand; Height = 36;
        }
        protected override void OnMouseEnter(EventArgs e) { BackColor = _hov; base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { BackColor = _bg;  base.OnMouseLeave(e); }
    }

    public class DarkInput : TextBox
    {
        public DarkInput()
        {
            BackColor = Theme.InputBg; ForeColor = Theme.TextPri;
            BorderStyle = BorderStyle.FixedSingle; Font = Theme.Body;
        }
    }

    public class DarkCombo : ComboBox
    {
        public DarkCombo()
        {
            BackColor = Theme.InputBg; ForeColor = Theme.TextPri;
            FlatStyle = FlatStyle.Flat; Font = Theme.Body;
            DropDownStyle = ComboBoxStyle.DropDownList;
        }
    }

    public class DarkNumeric : NumericUpDown
    {
        public DarkNumeric()
        {
            BackColor = Theme.InputBg; ForeColor = Theme.TextPri;
            Font = Theme.Body;
        }
    }

    public class CardBox : Panel
    {
        private Color _border;
        public CardBox(Color? border = null)
        {
            _border = border ?? Theme.Border;
            BackColor = Theme.Card;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var p = new Pen(_border)) e.Graphics.DrawRectangle(p, 0, 0, Width-1, Height-1);
        }
    }

    public class SectionLabel : Label
    {
        public SectionLabel(string t)
        {
            Text = t; ForeColor = Theme.TextDim;
            Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            AutoSize = true;
        }
    }

    public class FieldLabel : Label
    {
        public FieldLabel(string t)
        {
            Text = t; ForeColor = Theme.TextSec;
            Font = Theme.Label; AutoSize = true;
        }
    }
}
