using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Tek1
{

    static public class Const
    {
        public const int MAXTEK = 5;
    }

    public class ETekFieldInvalid : Exception
    {
        public TekField Field;
        public int Value;
        public string Msg;
        public ETekFieldInvalid(string msg, TekField field, int value) : base("Invalid field value")
        {
            Field = field;
            Value = value;
            Msg = msg;
        }
    }

    public class TekField
    {
        private bool _cascading = false; // used for signalling in UpdatePossibleValues

        private  int _value;
        public int Value { get { return _value; } set { SetValue(value); } }

        public bool Initial; // the value has been set initially and can not be cleared
        public TekArea Area;
        public List<TekField> Neighbours;  // the direct neighbours of the field
        public List<TekField> Influencers; // the fields that influence the field (Area+Neighbours)
        public List<int> PossibleValues;   // the values that the field may still have (is dynamically updated)
        public List<int> ExcludedValues;   // values excluded by heuristic solution process
        public List<int> Notes;            // notes the user has made

        private int _row, _col;
        public int Row { get { return _row; } }
        public int Col { get { return _col; } }

        public bool EatExceptions = true;   // entering an invalid value is ignored by default
                                            // set this to false to cause an ETekFieldException
                                            // the exceptions are used when solving the puzzle
        private bool _AutoNotes;
        public bool AutoNotes { get { return _AutoNotes; } set { _AutoNotes = value; if (value) UpdatePossibleValues(false); } } 
                                    // notes are set automatically to the values in PossibleValues - ExcludedValues

        public TekField(int arow, int acol) 
        {
            _row = arow;
            _col = acol;
            _value = 0;
            Initial = false;
            Neighbours = new List<TekField>();
            Influencers = new List<TekField>();
            PossibleValues = new List<int>();
            for (int i = 1; i <= Const.MAXTEK; i++)
                PossibleValues.Add(i);
            ExcludedValues = new List<int>();
            Notes = new List<int>();
            Area = null;
            AutoNotes = false;
        }

        public void SetValue(int avalue)
        {
            if (!EatExceptions && (avalue < 0 || avalue > Const.MAXTEK))
                throw new ETekFieldInvalid("value", this, avalue);
            _value = avalue;
            UpdatePossibleValues(true);
        }

        public void ToggleValue(int avalue)
        {
            if (Value == avalue)
                SetValue(0);
            else
                SetValue(avalue);
        }

        public void ToggleNote(int anote)
        {
            if (!EatExceptions && (anote < 0 || anote > Const.MAXTEK))
                throw new ETekFieldInvalid("note", this, anote);
            if (Notes.Contains(anote))
                Notes.Remove(anote);
            else
                Notes.Add(anote);
        }

        public void SetDefaultNotes()
        {
            Notes.Clear();
            foreach (int value in PossibleValues)
                if (!ExcludedValues.Contains(value))
                    ToggleNote(value);
        }

        public void ClearNotes()
        {
            Notes.Clear();
        }

		public List<int> CopyNotes()
		{
			return new List<int>(Notes);
        }

        public void LoadNotes(List<int> values)
        {
            Notes.Clear();
            Notes.AddRange(values);
        }

        public void ExcludeValue(int value, bool onoff = true)
        {
            if (!onoff)
            {
                ExcludedValues.Remove(value);
            }
            else if (!ExcludedValues.Contains(value))
                ExcludedValues.Add(value);
            UpdatePossibleValues(false);
        }

        public List<int> CopyExcludedValues()
        {
            return new List<int>(ExcludedValues);
        }

        public void LoadExcludedValues(List<int> values)
        {
            ExcludedValues.Clear();
            ExcludedValues.AddRange(values);
        }

        public void UpdatePossibleValues(bool cascade = false)
        {
            if (_cascading) // protection against endless loops 
                return;
            PossibleValues.Clear();
            if (Value == 0)
            {
                for (int i = 1; i <= ((Area == null) ? Const.MAXTEK : Area.Fields.Count); i++)
                    if (!ExcludedValues.Contains(i))
                        PossibleValues.Add(i);
                foreach (TekField field in Influencers)
                    if (field.Value > 0)
                        PossibleValues.Remove(field.Value);
                if (PossibleValues.Count == 0 && !EatExceptions)
                    throw new ETekFieldInvalid("no possible values", this, 0);
            }
            if (AutoNotes)
                SetDefaultNotes();
            if (!cascade)
                return;
            _cascading = true;
            try
            {
                foreach (TekField field in Influencers)
                    field.UpdatePossibleValues();
            }
            finally
            {
                _cascading = false;
            }

        }
        public bool IsValid()
        {
            if (Value == 0)
                return PossibleValues.Count > 0;
            else
            {
                foreach (TekField field in Influencers)
                    if (field.Value == Value)
                        return false;
                return true;
            }
        }

        public void AddNeighbour(TekField f)
        {
            if (!Neighbours.Contains(f)) // don't add more than once
                Neighbours.Add(f);
        }

        public bool HasNeighbour(TekField f)
        {
            return Neighbours.Contains(f);
        }

        public void AddInfluencer(TekField f)
        {
            if (!Influencers.Contains(f)) // don't add more than once
                Influencers.Add(f);
        }

        public void SetInfluencers()
        {
            Influencers.Clear();
            // add area
            if (Area != null)
                foreach (TekField field in Area.Fields)
                    if (field != this)
                        AddInfluencer(field);
            // add neighbours not in area
            foreach (TekField field in Neighbours)
                AddInfluencer(field);
        }
        public List<TekField> CommonInfluencers(params TekField[] fields)
        {
            List<TekField> result = new List<TekField>();
            foreach(TekField f in Influencers)
            {
                bool isCommon = true;
                for (int i = 0; i < fields.Length && isCommon; i++)
                    if (fields.Contains(f) || !fields[i].Influencers.Contains(f))
                        isCommon = false;
                if (isCommon)
                    result.Add(f);
            }
            return result;
        }

        public List<TekField> CommonInfluencers(List<TekField> fields)
        {
            List<TekField> result = new List<TekField>();
            foreach (TekField f in Influencers)
            {
                bool isCommon = true;
                for (int i = 0; i < fields.Count && isCommon; i++)
                    if (fields.Contains(f) || !fields[i].Influencers.Contains(f))
                        isCommon = false;
                if (isCommon)
                    result.Add(f);
            }
            return result;
        }

        public bool ValuePossible(int value)
        {
            return PossibleValues.Contains(value);
        }

        public bool ValuesPossible(params int[] values)
        {
            foreach (int value in values)
                if (!ValuePossible(value))
                    return false;
            return true;
        }
        public List<int> CommonPossibleValues(params TekField[] fields)
        {
            List<int> result = new List<int>();
            for(int value = 1; value <= Const.MAXTEK; value++)
            {
                bool isCommon = this.ValuePossible(value);
                for (int i = 0; i < fields.Length && isCommon; i++)
                    if (!fields[i].ValuePossible(value))
                        isCommon = false;
                if (isCommon)
                    result.Add(value);
            }
            return result;
        }
        public List<int> TotalPossibleValues(params TekField[] fields)
        {
            List<int> result = new List<int>();
            for (int value = 1; value <= Const.MAXTEK; value++)
            {
                if (this.ValuePossible(value))
                    result.Add(value);
                else
                    foreach (TekField field in fields)
                        if (field.ValuePossible(value) && !result.Contains(value))
                            result.Add(value);
            }
            return result;
        }
        public List<int> TotalPossibleValues(List <TekField> fields)
        {
            List<int> result = new List<int>();
            for (int value = 1; value <= Const.MAXTEK; value++)
            {
                if (this.ValuePossible(value))
                    result.Add(value);
                else
                    foreach(TekField field in fields)
                       if(field.ValuePossible(value) && !result.Contains(value))
                          result.Add(value);
            }
            return result;
        }
        public string AsString(bool includeValue = false, bool includeArea = false)
        {
            string result = String.Format("[{0},{1}", Row, Col);
            result = result + "]";

            if (includeValue)
                result += String.Format(" value:{0}{1}", Value == 0 ? "-" : Value.ToString(), Initial ? "i" : " ");
            if (includeArea)
                result += String.Format(" area: {0}", Area == null ? "-" : Area.AreaNum.ToString());
            return result;
        }
        
		public const uint FLD_DMP_NEIGHBOURS    = 1;
        public const uint FLD_DMP_INFLUENCERS   = 2;
        public const uint FLD_DMP_POSSIBLES     = 4;
        public const uint FLD_DMP_NOTES         = 8;
        public const uint FLD_DMP_EXCLUDES      = 16;
        public const uint FLD_DMP_ALL           = 65535;

        public void Dump(StreamWriter sw, uint flags = FLD_DMP_ALL)
        {
            sw.WriteLine("Field {0}", AsString(true, true));
            if ((flags & FLD_DMP_NEIGHBOURS) != 0)
            {
                sw.Write("Neighbours:");
                foreach (TekField t in Neighbours)
                    sw.Write("{0} ", t.AsString());
                sw.WriteLine();
            }
            if ((flags & FLD_DMP_INFLUENCERS) != 0)
            {
                sw.Write("Influencrs:");
                foreach (TekField t in Influencers)
                    sw.Write("{0} ", t.AsString());
                sw.WriteLine();
            }
            if ((flags & FLD_DMP_POSSIBLES) != 0)
            {
                sw.Write("Poss. Vals:");
                for (int i = 0; i < PossibleValues.Count; i++)
                    sw.Write("{0} ", PossibleValues[i]);
                sw.WriteLine();
            }
            if ((flags & FLD_DMP_NOTES) != 0)
            {
                sw.Write("Notes     :");
                for (int i = 0; i < Notes.Count; i++)
                    sw.Write("{0} ", Notes[i]);
                sw.WriteLine();
            }
            if ((flags & FLD_DMP_EXCLUDES) != 0)
            {
                sw.Write("Excludes  :");
                for (int i = 0; i < ExcludedValues.Count; i++)
                    sw.Write("{0} ", ExcludedValues[i]);
                sw.WriteLine();
            }
        }
    } // TekField

    public class TekFieldComparer : IComparer<TekField>
    // to sort fields to get a more logical order, mainly for debugging purposes
    {
        public int Compare(TekField x, TekField y)
        {
            if (x.Row == y.Row && x.Col == y.Col)
                return 0;
            else if (x.Row < y.Row)
                return -1;
            else if (x.Row == y.Row)
            {
                if (x.Col < y.Col)
                    return -1;
                else
                    return 1;
            }
            else
                return 1;
        }
    }
    public class TekFields
    {
        // base type for TekArea and TekRegion
        public List<TekField> Fields;
        public TekFields()
        {
            Fields = new List<TekField>();
        }

        public virtual bool IsEqual(TekFields fields2)
        {
            if (Fields.Count != fields2.Fields.Count)
                return false;
            foreach (TekField field in Fields)
                if (!fields2.Fields.Contains(field))
                    return false;
            return true;
        }
            
        public virtual void AddField(TekField f)
        {
            if (Fields.Contains(f)) // don't add more than once
                return;
            Fields.Add(f);
            Sort();
        }

        public List<TekField> GetCommonInfluencers()
        {
            if (Fields.Count > 0)
                return Fields[0].CommonInfluencers(Fields.GetRange(1, Fields.Count - 1));
            else
                return new List<TekField>();
        }

        public List<int> GetTotalPossibleValues()
        {
            if (Fields.Count > 0)
                return Fields[0].TotalPossibleValues(Fields.GetRange(1, Fields.Count - 1));
            else
                return new List<int>();
        }

        public bool FieldsAreConnected()
        {
            if (Fields.Count() <= 1)
                return true;
            foreach (TekField f in Fields)
            {
                bool hasOne = false;
                foreach (TekField f2 in Fields)
                    if (f2 != f && f.HasNeighbour(f2) && (f.Col == f2.Col || f.Row == f2.Row))
                    {
                        hasOne = true;
                        break;
                    }
                if (!hasOne)
                    return false;
            }
            return true;
        }

        public virtual string AsString()
        {
            string result = String.Format("Fields: ");
            foreach (TekField f in Fields)
                result = result + f.AsString();
            return result;
        }

        public void Dump(StreamWriter sw)
        {
            sw.WriteLine(AsString());
        }

        public int MaxValue()
        {
            return (Fields.Count < Const.MAXTEK ? Fields.Count : Const.MAXTEK);
        }

        public void Sort()
        {
            Fields.Sort(new TekFieldComparer());
        }

        public Dictionary<int, List<TekField>> GetFieldsForValues()
        {
            Dictionary<int, List<TekField>> result = new Dictionary<int, List<TekField>>();
            for (int i = 1; i <= MaxValue(); i++)
                result.Add(i, new List<TekField>());
            foreach (TekField f in Fields)
                if (f.Value > 0)
                    result[f.Value].Add(f);
                else
                    foreach (int value in f.PossibleValues)
                        result[value].Add(f);
            return result;
        }       
    } // TekFields

    public class TekArea : TekFields 
    {
        public int AreaNum;
        public TekArea(int anum) : base()
        {
            AreaNum = anum;
        }

        public List<TekArea> GetAdjacentAreas()
        {
            List<TekArea> result = new List<TekArea>();
            foreach (TekField field in Fields)
            {
                foreach (TekField neighbour in field.Neighbours)
                    if (neighbour.Area != null && this != neighbour.Area && !result.Contains(neighbour.Area))
                    {
                        result.Add(neighbour.Area);
                    }
            }
            return result;
        }

        public void SetInfluencers()
        {
            foreach (TekField f in Fields)
                f.SetInfluencers();
        }

        public override void AddField(TekField f)
        {
            base.AddField(f);
            if (f.Area != null)
                return; // or exception
            f.Area = this;
            SetInfluencers();            
        }

        public override string AsString()
        {
            string result = String.Format("Area {0}:", AreaNum);
            foreach (TekField f in Fields)
                result = result + f.AsString();
            return result;
        }

    } // TekArea

    public class TekBoard
    {
        public TekField[,] Fields;
        public List<TekArea> Areas;
        private int _rows, _cols;
        public int Rows { get { return _rows; } }
        public int Cols { get { return _cols; } }

        private bool _AutoNotes;
        public bool AutoNotes { get { return _AutoNotes; } set { _AutoNotes = value; foreach (TekField field in Fields) field.AutoNotes = value; } }
        private bool _EatExceptions;
        public bool EatExceptions { get { return _EatExceptions; } set { _EatExceptions = value; foreach (TekField field in Fields) field.EatExceptions = value; } }

        public TekBoard()
        {
            Areas = new List<TekArea>();
            _rows = 0;
            _cols = 0;
            Fields = null;
            _EatExceptions = true;
        }

        public TekBoard(int rows, int cols) : this()
        {
            _rows = rows;
            _cols = cols;
            initFields(rows, cols);
            setNeighbours();
        }

        public TekBoard(TekBoard board) : this(board.Rows, board.Cols)
        {
            CopyFields(board.Fields);
            Areas = CopyAreas(board.Areas);
            EatExceptions = board.EatExceptions;
            AutoNotes = board.AutoNotes;
        }

        private void initFields(int rows, int cols)
        {
            Fields = new TekField[rows, cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    Fields[r, c] = new TekField(r, c);
                }
            EatExceptions = _EatExceptions;
        }

        private void setNeighbours()
        {
            // set neighbours
            for (int r = 0; r < Rows; r++)
                for (int r1 = r - 1; r1 <= r + 1; r1++)
                {
                    if (r1 >= 0 && r1 < Rows)
                    {
                        for (int c = 0; c < Cols; c++)
                            for (int c1 = c - 1; c1 <= c + 1; c1++)
                            {
                                if ((r1 != r || c1 != c) && c1 >= 0 && c1 < Cols)
                                    Fields[r, c].AddNeighbour(Fields[r1, c1]);
                            }
                    }
                }
        }

        public int[,] CopyValues()
        {
            int[,] result = new int[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    result[r, c] = Fields[r, c].Value;
            return result;
        }


        public void LoadValues(int[,] newValues)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Fields[r, c].Value = newValues[r, c];
        }

        public List<int>[,] CopyNotes()
        {
            List<int>[,] result = new List<int>[Rows, Cols];
            foreach (TekField field in Fields)
            {
                result[field.Row, field.Col] = field.CopyNotes();
            }
            return result;
        }

        public void LoadNotes(List<int>[,] notes)
        {
            foreach (TekField field in Fields)
            {
                field.LoadNotes(notes[field.Row, field.Col]);
            }
        }
        public List<int>[,] CopyExcludedValues()
        {
            List<int>[,] result = new List<int>[Rows, Cols];
            foreach (TekField field in Fields)
            {
                result[field.Row, field.Col] = field.CopyExcludedValues();
            }
            return result;
        }

        public void LoadExcludedValues(List<int>[,] excludedvalues)
        {
            foreach (TekField field in Fields)
            {
                field.LoadExcludedValues(excludedvalues[field.Row, field.Col]);
            }
        }

        public bool IsInRange(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Cols;
        }

        public void ResetValues()
        {
            foreach (TekField field in Fields)
                if (!field.Initial)
                {
                    field.Value = 0;
                    field.Notes.Clear();
                    field.ExcludedValues.Clear();
                }
        }

        public TekArea DefineArea(List<TekField> list)
        {
            TekArea result = new TekArea(Areas.Count());
            foreach (TekField f in list)
                result.AddField(f);
            Areas.Add(result);
            return result;
        }

        public void DeleteArea(TekArea area)
        {
            foreach (TekField field in area.Fields)
            {
                field.Area = null;
            }
            foreach (TekField field in area.Fields)
            {
                field.SetInfluencers();
            }
            Areas.Remove(area);
        }

        public bool IsSolved()
        {
            foreach (TekField field in Fields)
                if (field.Value == 0)
                    return false;
                else foreach (TekField field2 in field.Influencers)
                        if (field2.Value == field.Value)
                            return false;
            return true;
        }

        private TekArea CopyArea(TekArea area)
        {
            TekArea result = new TekArea(area.AreaNum);
            foreach (TekField field in area.Fields)
                result.AddField(Fields[field.Row, field.Col]);
            return result;
        }
        private List<TekArea> CopyAreas(List <TekArea> areas)
        {
            List<TekArea> result = new List<TekArea>();
            foreach (TekArea area in areas)
                result.Add(CopyArea(area));
            return result;
        }

        private void CopyFields(TekField[,] fields)
        {
            foreach (TekField field in fields)
            {
                TekField ThisField = Fields[field.Row, field.Col];
                ThisField.Value = field.Value;
                ThisField.Initial = field.Initial;
                ThisField.PossibleValues.Clear();
                ThisField.PossibleValues.AddRange(field.PossibleValues);
                ThisField.ExcludedValues.Clear();
                ThisField.ExcludedValues.AddRange(field.ExcludedValues);
                ThisField.Notes.Clear();
                ThisField.Notes.AddRange(field.Notes);
            }
        }
        private TekField[,] CopyFields()
        {
            TekField[,] result = new TekField[Rows, Cols];
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Cols; col++)
                {
                    result[row, col] = new TekField(row, col);
                    result[row, col].Value = Fields[row, col].Value;
                    result[row, col].Initial = Fields[row, col].Initial;
                }
            return result;
        }

        public void Resize(int rows, int cols)
        {
            TekField [,] oldValues = CopyFields();

            foreach (TekArea area in Areas)
                DeleteArea(area);
            Areas.Clear();

            _rows = rows;
            _cols = cols;
            initFields(rows, cols);
            setNeighbours();
            for (int row = 0; row < Math.Min(Rows-1,oldValues.GetLength(0)); row++)
                for (int col = 0; col < Math.Min(Cols - 1, oldValues.GetLength(1)); col++)
                {
                    Fields[row, col].Value = oldValues[row, col].Value;
                    Fields[row, col].Initial = oldValues[row, col].Initial;
                }
        }

        public void Dump(StreamWriter sw)
        {
            for (int r = 0; r < _rows; r++)
                for (int c = 0; c < _cols; c++)
                {
                    Fields[r, c].Dump(sw);
                }
            foreach (TekArea a in Areas)
            {
                a.Dump(sw);
            }
        }

        public void Dump(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
                Dump(sw);
        }

        public List<string> ValidAreasErrors()
        {
            List<string> result = new List<string>();
            // every field is part of an area
            foreach(TekField field in Fields)
            {
                if (field.Area == null)
                    result.Add(String.Format("Field ({0},{1}) is not part of an area", field.Row, field.Col));
            }
            // every area contains only adjacent fields
            foreach(TekArea area in Areas)
            {
                if (!area.FieldsAreConnected())
                    result.Add(String.Format("Area {0} is not valid", area.AsString()));
            }
            return result;
        }

        public bool IsValidAreas()
        {
            return ValidAreasErrors().Count == 0;
        }

        public void SetDefaultNotes()
        {
            foreach (TekField field in Fields)
                field.SetDefaultNotes();
        }        
    } // TekBoard
 
} // namespace Tek1
