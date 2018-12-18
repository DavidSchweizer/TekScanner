using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Tek1
{
    public partial class ConfigurationForm : Form
    {
        public TekHeuristics Heuristics;

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        public void DoSaveData()
        {
            Heuristics.SetHeuristicDescriptions(clbHeuristics.Items.OfType<string>().ToList()); // set the index 
            for (int i = 0; i < clbHeuristics.Items.Count; i++)
                Heuristics.SetHeuristicEnabled(clbHeuristics.Items[i].ToString(), clbHeuristics.GetItemChecked(i));
            using (StreamWriter cfg = new StreamWriter("heuristics.cfg"))
            {
                Heuristics.SaveConfiguration(cfg);
            }
        }

        public void DoSetData(TekHeuristics heuristics)
        {
            clbHeuristics.Items.Clear();
            Heuristics = heuristics;
            if (Heuristics == null)
                return;
            List<string> descriptions = Heuristics.GetHeuristicDescriptions();
            foreach (string description in descriptions)
                clbHeuristics.Items.Add(description, Heuristics.GetHeuristicEnabled(description));
            EnableUpDownButtons();
        }

        private void EnableUpDownButtons()
        {
            int index = clbHeuristics.SelectedIndex;
            bUp.Enabled = index > 0;
            bDown.Enabled = index >= 0 && index < clbHeuristics.Items.Count;
        }

        private void DoMoveSelected(int direction)
        {
            int index = clbHeuristics.SelectedIndex;
            if (index < 0)
                return;
            bool isChecked = clbHeuristics.GetItemChecked(index);
            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= clbHeuristics.Items.Count)
                return;
            object selected = clbHeuristics.SelectedItem;
            clbHeuristics.Items.Remove(selected);
            clbHeuristics.Items.Insert(newIndex, selected);
            clbHeuristics.SetItemChecked(newIndex, isChecked);
            clbHeuristics.SetSelected(newIndex, true);
            EnableUpDownButtons();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoMoveSelected(-1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DoMoveSelected(1);
        }

        private void clbHeuristics_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableUpDownButtons();
        }
    }
}
