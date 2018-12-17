using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tek1
{
    public partial class PlayPanel : UserControl
    {
        TekView _view;

        public TekView View { get { return _view; } set { _view = value; } }

        public PlayPanel()
        {
            InitializeComponent();
        }

        private void CallPlayActionHandler(int row, int col, TekMove move, int value)
        {
            
        }
        private void NumberButtons_Click(object sender, EventArgs e)
        {
            if (View.Board == null)
                return;
            int value = 0;
            if ((sender is Button) && Int32.TryParse((sender as Button).Text, out value))
            {
                View.ToggleSelectedValue(value);
            }
        }

        private void ToggleNoteButton_Click(object sender, EventArgs e)
        {
            int value = 0;
            if ((sender is Button) && Int32.TryParse((sender as Button).Text, out value))
                View.ToggleSelectedNoteValue(value);
        }

        private void bDefaultNotes_Click(object sender, EventArgs e)
        {
            View.ShowDefaultNotes();
        }

        private void cbMultipleSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (cbMultipleSelect.Checked)
                View.Selector.CurrentMode = TekSelect.SelectMode.smMultiple;
            else
                View.Selector.CurrentMode = TekSelect.SelectMode.smSingle;
        }

        private void bTakeSnap_Click(object sender, EventArgs e)
        {
            View.TakeSnapshot(String.Format("snapshot {0}", 1 + View.SnapshotCount()));
            // to be augmented: edit name of snapshot
        }

        private void bRestoreSnap_Click(object sender, EventArgs e)
        {
            View.RestoreSnapshot("snapshot 1"); // to be augmented: select your snapshot
        }

        private void bBackspace_Click(object sender, EventArgs e)
        {
            View.UnPlay();
        }      
    }
}
