﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using ControlAstro.Native;

namespace ControlAstro.Controls
{
    [ToolboxBitmap(typeof(TextBox))]
    public partial class TextBoxWatermark : TextBox
    {

        private string hintText = string.Empty;
        [Description("水印文本")]
        public string HintText
        {
            get { return this.hintText; }
            set
            {
                this.hintText = value;
                this.Invalidate();
            }
        }

        private Font hintFont = SystemFonts.DefaultFont;
        [Description("用于显示水印文本的字体")]
        public Font HintFont
        {
            get { return this.hintFont; }
            set
            {
                this.hintFont = value;
                this.Invalidate();
            }
        }

        [Description("是否只能输入数字")]
        public bool NumberOnly { get; set; }

        [Description("是否只能输入字母")]
        public bool LetterOnly { get; set; }


        public TextBoxWatermark()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            ToolStripMenuItem ToolStripMenuItemPaste = new ToolStripMenuItem();
            ToolStripMenuItemPaste.Name = "ToolStripMenuItemPaste";
            ToolStripMenuItemPaste.Size = new Size(152, 22);
            ToolStripMenuItemPaste.Text = "粘贴";
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { ToolStripMenuItemPaste });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(153, 48);
            ToolStripMenuItemPaste.Enabled = Clipboard.ContainsText();
            contextMenuStrip.Renderer = new ToolStripRendererEx();
            contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip_ItemClicked);

            ContextMenuStrip = contextMenuStrip;
        }

        private void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Name)
            {
                case "ToolStripMenuItemPaste":
                    this.Text = Clipboard.GetText(TextDataFormat.Text);
                    break;
                default:
                    break;
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == Keys.Enter)
            {
                return base.ProcessDialogKey(keyData);
            }
            if (NumberOnly)
            {
                if ((keyData >= Keys.NumPad0 && keyData <= Keys.NumPad9) ||
                (keyData >= Keys.D0 && keyData <= Keys.D9))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (LetterOnly)
            {
                if (keyData >= Keys.A && keyData <= Keys.Z)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)WinApi.Messages.WM_PAINT || m.Msg == (int)WinApi.Messages.WM_NCPAINT)
            {
                IntPtr hDC = WinApi.GetWindowDC(m.HWnd);
                if (hDC.ToInt32() == 0)
                {
                    return;
                }
                WmPaint(hDC);

                //返回结果 
                m.Result = IntPtr.Zero;
                //释放 
                WinApi.ReleaseDC(m.HWnd, hDC);
            }
        }

        /// <summary>
        /// 水印
        /// </summary>
        /// <param name="hDC"></param>
        private void WmPaint(IntPtr hDC)
        {
            using (Graphics graphics = Graphics.FromHdc(hDC))
            {
                if (Text.Length == 0 && !string.IsNullOrEmpty(hintText) && !Focused)
                {
                    TextFormatFlags format = TextFormatFlags.EndEllipsis | TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    if (RightToLeft == RightToLeft.Yes)
                    {
                        format |= TextFormatFlags.RightToLeft | TextFormatFlags.Right;
                    }
                    TextRenderer.DrawText(graphics, this.hintText, this.hintFont, ClientRectangle, Color.Gray, format);
                }
            }
        }

    }
}
