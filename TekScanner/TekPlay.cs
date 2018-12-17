using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tek1
{
    public enum TekMove { tmValue, tmNote, tmClear, tmExclude, tmSnapshot };

    public class TekPlay
    {
        private int _row;
        private int _col;
        private TekMove _move;
        private int _value;
        private List<int> _notes;
        private string _name;
        public int Row { get { return _row; } }
        public int Col { get { return _col; } }
        public int Value { get { return _value; } }
        public List<int> Notes { get { return _notes; } } 
        public TekMove Move { get { return _move; } }
        public string Name { get { return _name; } }
        public TekPlay(int row, int col, TekMove move, int value, List<int> notes = null, string name = "")
        {
            _row = row;
            _col = col;
            _move = move;
            _value = value;
            if (notes == null)
                _notes = null;
            else
            {
                _notes = new List<int>();
                foreach (int i in notes)
                    _notes.Add(i);
            }
            _name = name;
        }
    }

    public class TekMoves
    {
        private TekBoard _board;
        public TekBoard Board { get { return _board; } }

        private Stack<TekPlay> _moves;
        public Stack<TekPlay> Moves { get { return _moves; } }

        private TekSnapShot _snapshots;
        protected TekSnapShot Snapshots { get { return _snapshots; } }

        public TekMoves(TekBoard board)
        {
            _moves = new Stack<TekPlay>();
            _board = board;
            _snapshots = new TekSnapShot(Board);
            _snapshots.AutoRemove = true;
        }

        public void TakeSnapshot(string name)
        {
            Snapshots.TakeSnapshot(name);
            PushMove(0, 0, TekMove.tmSnapshot, 0, name);
        }

        public void RestoreSnapshot(string name)
        {
            Snapshots.RestoreSnapshot(name);
            while (Moves.Count > 0 && Moves.Peek().Move != TekMove.tmSnapshot && Moves.Peek().Name != name)
                Moves.Pop();
            if (Moves.Count > 0) // found snapshot location
                Moves.Pop();
        }
        public int SnapshotCount()
        {
            return Snapshots.Count();
        }

        private void PushMove(int row, int col, TekMove move, int value, string name="")
        {
            switch(move)
            {
                case TekMove.tmClear:
                    Moves.Push(new TekPlay(row, col, TekMove.tmClear, value, Board.Fields[row, col].Notes));
                    break;
                case TekMove.tmSnapshot:
                    Moves.Push(new TekPlay(row, col, TekMove.tmClear, value, null, name));
                    break;
                default:
                    Moves.Push(new TekPlay(row, col, move, value));
                    break;
            }
        }

        public void PlayValue(TekField field, int value)
        {
            PushMove(field.Row, field.Col, TekMove.tmValue, value);
            field.ToggleValue(value);
        }

        public void PlayValue(int row, int col, int value)
        {
            PlayValue(Board.Fields[row, col], value);
        }

        public void ExcludeValue(TekField field, int value)
        {
            PushMove(field.Row, field.Col, TekMove.tmExclude, value);
            field.ExcludeValue(value, true);
            if (field.Notes.Contains(value))
                PlayNote(field, value);
        }

        public void ExcludeValues(TekField field, params int[] values)
        {
            for (int i = 0; i < values.Length; i++)
                ExcludeValue(field, values[i]);

        }
        public void ExcludeValue(int row, int col, int value)
        {
            ExcludeValue(Board.Fields[row, col], value);
        }
        public void ExcludeValues(int row, int col, int value)
        {
            ExcludeValues(Board.Fields[row, col], value);
        }

        public void PlayNote(TekField field, int value)
        {
            PushMove(field.Row, field.Col, TekMove.tmNote, value);
            field.ToggleNote(value);
        }

        public void PlayNotes(TekField field, params int[] values)
        {
            for (int i = 0; i < values.Length; i++)
                PlayNote(field, values[i]);
        }

        public void PlayNote(int row, int col, int value)
        {
            PlayNote(Board.Fields[row, col], value);
        }
        public void PlayNotes(int row, int col, params int[] values)
        {
            PlayNotes(Board.Fields[row, col], values);
        }

        public void PlayClear(TekField field)
        {
            PushMove(field.Row, field.Col, TekMove.tmClear, field.Value);
            field.ClearNotes();
            if (!field.Initial)
                field.Value = 0;
        }

        public void PlayClear(int row, int col)
        {
            PlayClear(Board.Fields[row, col]);
        }

        public void UnPlay()
        {
            if (Moves.Count() > 0)
            {
                TekPlay move = Moves.Pop();
                switch (move.Move)
                {
                    case TekMove.tmValue:
                        Board.Fields[move.Row, move.Col].ToggleValue(move.Value);
                        break;
                    case TekMove.tmNote:
                        Board.Fields[move.Row, move.Col].ToggleNote(move.Value);
                        break;
                    case TekMove.tmExclude:
                        Board.Fields[move.Row, move.Col].ExcludeValue(move.Value, false);
                        break;
                    case TekMove.tmClear:
                        Board.Fields[move.Row, move.Col].ToggleValue(move.Value);
                        foreach (int i in move.Notes)
                            Board.Fields[move.Row, move.Col].ToggleNote(i);
                        break;
                }
            }
        }
        public void Clear()
        {
            Moves.Clear();
        }
    }

    public class TekSnapShot
    {
        public bool AutoRemove;
        private TekBoard _board;
        public TekBoard Board { get { return _board; } }

        protected List<int[,]> _ssValues;
        protected List<int[,]> Values {  get { return _ssValues; } }

        protected List<List<int>[,]> _ssNotes;
        protected List<List<int>[,]> Notes { get { return _ssNotes; } }

        protected List<List<int>[,]> _ssExcludedValues;
        protected List<List<int>[,]> ExcludedValues { get { return _ssExcludedValues; } }

        protected List<string> _snapshots;
        public List<string> Snapshots { get { return _snapshots; } }


        public TekSnapShot(TekBoard board)
        {
            _ssValues = new List<int[,]>();
            _ssNotes = new List<List<int>[,]>();
            _ssExcludedValues = new List<List<int>[,]>();
            _snapshots = new List<string>();
            _board = board;
            AutoRemove = false;
        }

        public void TakeSnapshot(string name)
        {
            Values.Add(Board.CopyValues());
            Notes.Add(Board.CopyNotes());
            ExcludedValues.Add(Board.CopyExcludedValues());
            Snapshots.Add(name);
        }

        public void RestoreSnapshot(string name)
        {
            int index = Snapshots.IndexOf(name);
            if (index == -1)
                return;
            bool prev = Board.EatExceptions;
            Board.EatExceptions = true;
            try
            {
                Board.LoadExcludedValues(ExcludedValues.ElementAt(index));
                Board.LoadValues(Values.ElementAt(index));
                Board.LoadNotes(Notes.ElementAt(index));
            }
            finally
            {
                Board.EatExceptions = prev;
            }
            if (AutoRemove)
            {
                Values.RemoveAt(index);
                Notes.RemoveAt(index);
                ExcludedValues.RemoveAt(index);
                Snapshots.RemoveAt(index);
            }
        }

        public int Count()
        {
            return Snapshots.Count;
        }
    }
}
