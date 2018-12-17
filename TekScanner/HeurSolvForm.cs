using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tek1
{
    public partial class HeurSolvForm : Form
    {
        TekEdit View;
        TekHeuristics heuristics = new TekHeuristics();
        TekSnapShot Snapshots;

        List<TekField> previousHeuristicFields;
        List<TekField> previousAffectedFields;
        bool _lastShowErrors = false;
        bool Paused;
        const int MARGIN = 20;

        void LogPlayedMove(int row, int col, TekMove move, int value)
        {
            string s = String.Format("Field [{0},{1}]: ", row, col);
            switch(move)
            {
                case TekMove.tmValue:
                    s = s + String.Format("value changed {0}", value);
                    break;

                case TekMove.tmNote:
                    s = s + String.Format("note changed {0}", value);
                    break;
            }
            listBox1.Items.Add(s);
        }

        public HeurSolvForm()
        {
            InitializeComponent();
            View = new TekEdit(split.Panel1, new Point(MARGIN / 2, MARGIN / 2),
                new Point(split.Panel1.ClientRectangle.Width - MARGIN / 2,
                          split.Panel1.ClientRectangle.Height - MARGIN / 2));
            playPanel1.View = View;
            View.PlayActionHandler = LogPlayedMove;
            ofd1.FileName = "test.tx";
            //DoLoad();
        }

        public void DoLoad(string FileName)
        {
            View.LoadFromFile(FileName);
            split.SplitterDistance = View.Width + MARGIN;
            this.Text = FileName;
            initializeHeuristicLog(this.Text);
            DoReset(true);

        }
        void DoLoad()
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                DoLoad(ofd1.FileName);
            }
        }

        private void bLoad_Click(object sender, EventArgs e)
        {
            DoLoad();
        }

        private void Button_ToggleValue_Click(object sender, EventArgs e)
        {
            if (View.Board == null)
                return;
            int value = 0;
            if ((sender is Button) && Int32.TryParse((sender as Button).Text, out value))
            {
                View.ToggleSelectedValue(value);
            }
        }

        private void bSave_Click(object sender, EventArgs e)
        {

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            View.HandleKeyDown(ref msg, keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void bSolveClick(object sender, EventArgs e)
        {

        }

        void DoReset(bool initial = false)
        {
            if (!initial)
                CloseHeuristicLog();
            listBox1.Items.Clear();
            heuristics.Reset();
            if (!initial)
                View.ResetValues();
            bStart.Enabled = true;
            Snapshots = new TekSnapShot(View.Board);
            Snapshots.AutoRemove = false;
        }

        private void bReset_Click(object sender, EventArgs e)
        {
            DoReset();
        }

        private void ToggleNoteButton_Click(object sender, EventArgs e)
        {
            int value = 0;
            if ((sender is Button) && Int32.TryParse((sender as Button).Text, out value))
                View.ToggleSelectedNoteValue(value);
        }

        private void bUnPlay_Click(object sender, EventArgs e)
        {
            View.UnPlay();
        }

        private void bTakeSnap_Click(object sender, EventArgs e)
        {
            View.TakeSnapshot(String.Format("snapshot {0}", 1 + View.SnapshotCount()));
        }

        private void bRestoreSnap_Click(object sender, EventArgs e)
        {
            View.RestoreSnapshot("snapshot 1");
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            if (View != null)
            {
                View.SetSize(split.Panel1.Width, split.Panel1.Height);
            }
        }

        private void cbShowError_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void bCheck_Click(object sender, EventArgs e)
        {
            _lastShowErrors = View.SetShowErrors(!_lastShowErrors);
        }

        private void bDefaultNotes_Click(object sender, EventArgs e)
        {
            View.ShowDefaultNotes();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TekBoard board;
            board = new TekBoard(6, 6);
            View.Board = board;
            Refresh();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            TekArea area;
            TekFieldView field = View.Selector.CurrentFieldView;
            if (field != null)
            {
                area = View.SelectArea(field.Row, field.Col);
                View.DeleteArea(area);
                Refresh();
            }

        }

        private void bCreate_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void bLoad_Click_1(object sender, EventArgs e)
        {
        }

        private void bSave_Click_1(object sender, EventArgs e)
        {
        }

        private void bSolve_Click(object sender, EventArgs e)
        {
        }

        StreamWriter HeuristicLog = null;
        void initializeHeuristicLog(string PuzzleFilename)
        {
            if (HeuristicLog != null)
                CloseHeuristicLog();
            HeuristicLog = new StreamWriter(Path.ChangeExtension(PuzzleFilename, "log"));
            LogHeuristic("start Log {0} at {1}\n", PuzzleFilename, DateTime.Now.ToString("dd MMMM yyyy   H:mm:ss"));            
        }

        void LogHeuristic(string message, params object[] fields)
        {
            if (HeuristicLog == null)
                initializeHeuristicLog(this.Text);
            if (HeuristicLog == null)
                return;
            HeuristicLog.WriteLine(String.Format(message, fields));
            HeuristicLog.Flush();
        }

        void CloseHeuristicLog()
        {
            if (HeuristicLog != null)
            { 
                LogHeuristic("close Log at {0}", DateTime.Now.ToString("dd MMMM yyyy   H:mm:ss"));
                HeuristicLog.Close();
            }
            HeuristicLog = null;
        }

        void ShowHeuristicFields(List<TekField> HeuristicFields, List<TekField> AffectedFields)
        {
            View.SelectFields(previousHeuristicFields = HeuristicFields);
            View.HighlightFields(previousAffectedFields = AffectedFields, true);
        }

        void UnshowHeuristicFields()
        {
            View.SelectFields(previousHeuristicFields, false);
            View.HighlightFields(previousAffectedFields, false);


        }

        static int heurFound;
        string HeuristicDescription;
        private void AfterHeuristicFoundHandler(TekHeuristic heuristic)
        {
            heurFound++;
            HeuristicDescription = String.Format("{0}: {1}", heurFound, heuristic.AsString());
            listBox1.Items.Add(HeuristicDescription);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            listBox1.Refresh();
            LogHeuristic(HeuristicDescription);
            ShowHeuristicFields(heuristic.HeuristicFields, heuristic.AffectedFields);
        }

        private bool BeforeHeuristicExecution(TekHeuristic heuristic)
        {
            Application.DoEvents();
            if (checkBox1.Checked || Paused)
            {
                Paused = true;
                while (Paused)
                {
                    Application.DoEvents(); // Delphi style, is supposed to be unsafe
                    if (isFinished)
                        return false;
                }
            }
            if (!isFinished)
            {
                Snapshots.TakeSnapshot(HeuristicDescription);
            }
            return true;
        }

        private bool AfterHeuristicExecution(TekHeuristic heuristic)
        {
            View.HighlightFields(heuristic.AffectedFields, false);
            View.Selector.ClearMultiSelect();
            View.Refresh();
            return true;
        }

        private void RunHeuristics(bool initial)
        {
            if (isFinished)
                return;
            if (initial)
            {
                listBox1.Items.Clear();
                View.ShowDefaultNotes();
                heurFound = 0;
            }
            Paused = false;

            heuristics.AfterHeuristicFoundHandler = AfterHeuristicFoundHandler;
            heuristics.BeforeExecutionHandler = BeforeHeuristicExecution;
            heuristics.AfterExecutionHandler = AfterHeuristicExecution;

            heuristics.HeuristicSolve(View.Board, View.Moves);
        }

        static bool isFinished = false;
        private void bStartClick(object sender, EventArgs e)
        {
            bStart.Enabled = false;
            bPause.Enabled = true;
            bPause.Visible = true;
            isFinished = false; 
            RunHeuristics(listBox1.Items.Count == 0);
            if (!isFinished)
            {
                if (View.Board.IsSolved())
                {
                    MessageBox.Show("Solved!");
                    LogHeuristic("Solved!\n");
                    isFinished = true;
                    View.Refresh();                        
                }
                else
                {
                    MessageBox.Show("Can not further be solved using these heuristics...");
                    LogHeuristic("\nCan not further be solved using these heuristics...");
                    if (MessageBox.Show("Save state?", "Verify", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        TekBoardParser parser = new TekBoardParser();
                        parser.Export(View.Board, "test.tx");
                    }
                    isFinished = true;
                }
            }
            bStart.Enabled = true;
                
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void bPauseClick(object sender, EventArgs e)
        {
            Paused = true;
            bStart.Enabled = true;
        }

        private void bNext_Click(object sender, EventArgs e)
        {
            Paused = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            bNext.Enabled = checkBox1.Checked;
            bNext.Visible = checkBox1.Checked;
//            bPause.Enabled = checkBox1.Checked;
  //          bPause.Visible = checkBox1.Checked;
        }

        private void bReset_Click_1(object sender, EventArgs e)
        {
            DoReset();
        }

        private void HeurSolvForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseHeuristicLog();
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            using (ConfigurationForm form = new ConfigurationForm())
            {
                form.DoSetData(heuristics);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    form.DoSaveData();
                }
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int selected = listBox1.SelectedIndex;
            if (selected < 0)
                return;
            TekHeuristicResult result = heuristics.ReturnResult(selected);
            UnshowHeuristicFields();
            Snapshots.RestoreSnapshot(listBox1.Items[selected].ToString());
            ShowHeuristicFields(result.HeuristicFields, result.AffectedFields);
            View.Refresh();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            if (View.Board != null && !View.Solve())
                MessageBox.Show("can not be solved");
        }
    }

   
    
}
