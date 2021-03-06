// 
// Main website for TVRename is http://tvrename.com
// 
// Source code available at http://code.google.com/p/tvrename/
// 
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// 

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// Starting from:
// http://social.msdn.microsoft.com/Forums/ja-JP/csharpexpressja/thread/67475927-015c-4206-b5e7-d67504edb3a1
//
// Standard windows listview has a very weird behaviour when shift-clicking, if it is a multicolumn, 
// full-row-select, and checkboxes on.. and you click in anything other than the first column.

// This is the list view used on the "Scan" tab

namespace TVRename
{
    /// <summary>
    /// Summary for MyListView
    /// </summary>
    public class MyListView : ListViewFlickerFree
    {
        private bool _checkEnable;
        private bool _keyCheck;
        private bool _menuCheck;
        private bool _onMouseDown;

        public MyListView()
        {
            this._keyCheck = false;
            this._checkEnable = true;
            this._onMouseDown = false;
            this._menuCheck = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this._onMouseDown = true;
            base.OnMouseDown(e);
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            if (this._onMouseDown)
                this._checkEnable = false;
            base.OnItemSelectionChanged(e);
        }

        protected override void OnItemCheck(ItemCheckEventArgs ice)
        {
            if (!this._menuCheck && !this._keyCheck && (false == this._checkEnable)) //  || (!_keyCheck && _checkEnable && SelectedItems->Count > 1)
            {
                ice.NewValue = ice.CurrentValue;
                return;
            }
            base.OnItemCheck(ice);
            if (this.SelectedItems.Count == 1)
            {
                this.Items[this.SelectedIndices[0]].Selected = false;
                this.Items[ice.Index].Selected = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this._checkEnable = true;
            this._onMouseDown = false;
            base.OnMouseUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                this._keyCheck = true;
            else
                base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                this._keyCheck = false;
        }

        // The 'TopItem' function doesn't work in a ListView if groups are enabled. This is meant to be a workaround.
        // Problem is, it just doesn't work and I don't know why!
        const Int32 LVM_FIRST = 0x1000;
        const Int32 LVM_SCROLL = LVM_FIRST + 20;
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;

        [DllImport("user32.dll")]
        static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public int GetScrollVerticalPos()
        {
            return GetScrollPos(this.Handle, SB_VERT);
        }

        public void SetScrollVerticalPos(int position)
        {
            var currentPos = GetScrollPos(this.Handle, SB_VERT);
            var delta = -(currentPos - position);
            SendMessage(this.Handle, LVM_SCROLL, IntPtr.Zero, (IntPtr)delta); // First param is horizontal scroll amount, second is vertical scroll amount
        }
    }
}