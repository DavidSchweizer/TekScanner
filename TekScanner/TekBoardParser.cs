using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Tek1
{
    public class TekBoardParser
    {
        const string COMMENTPATTERN = @"#.*";
        private Regex commentPattern;

        const string SIZEPATTERN = @"size=(?<rows>[1-9]\d*),(?<cols>[1-9]\d*)";
        const string SIZEFORMAT = @"size={0},{1}";
        private Regex sizePattern;
        const string AREAPATTERN1 = @"(area=)(\((?<row>\d+),(?<col>\d+)\))(?<rest>\(.*\))?";
        const string AREAPATTERN2 = @"(\((?<row>\d+),(?<col>\d+)\))";
        const string AREAFORMAT1 = @"area=";
        const string AREAFORMAT2 = @"({0},{1})";
        private Regex areaPattern1, areaPattern2;
        const string VALUEPATTERN = @"value=\((?<row>\d+),(?<col>\d+):(?<value>[1-5])(?<initial>i)?\)";
        const string VALUEFORMAT = @"value=({0},{1}:{2}{3})";
        private Regex valuePattern;
        const string NOTESPATTERN1 = @"notes=\((?<row>\d+),(?<col>\d+)\)(?<value>[1-5])+(?<rest>(.*))?";
        const string NOTESPATTERN2 = @"(?<value>[1-5])+";
        const string NOTESFORMAT1 = @"notes=({0},{1})";
        const string NOTESFORMAT2 = @"{0} ";
        const string EXCLUDESPATTERN1 = @"excludes=\((?<row>\d+),(?<col>\d+)\)(?<value>[1-5])+(?<rest>(.*))?";
        const string EXCLUDESPATTERN2 = @"(?<value>[1-5])+";
        const string EXCLUDESFORMAT1 = @"excludes=({0},{1})";
        const string EXCLUDESFORMAT2 = @"{0} ";
        private Regex notesPattern1, notesPattern2;
        private Regex excludesPattern1, excludesPattern2;

        public TekBoardParser()
        {
            commentPattern = new Regex(COMMENTPATTERN);
            sizePattern = new Regex(SIZEPATTERN);
            areaPattern1 = new Regex(AREAPATTERN1);
            areaPattern2 = new Regex(AREAPATTERN2);
            valuePattern = new Regex(VALUEPATTERN);
            notesPattern1 = new Regex(NOTESPATTERN1);
            notesPattern2 = new Regex(NOTESPATTERN2);
            excludesPattern1 = new Regex(EXCLUDESPATTERN1);
            excludesPattern2 = new Regex(EXCLUDESPATTERN2);
        }

        private void ParseError(string format, params object[] list)
        {
            throw new Exception(String.Format(format, list));
        }

        public TekBoard Import(string filename)
        {
            TekBoard board = null;
            using (StreamReader sr = new StreamReader(filename))
            {
                if ((board = ParseStream(sr)) == null)
                {
                    ParseError("invalid file {0}", filename);
                }
            }
            return board;
        }

        public void Export(TekBoard board, string filename)
        {
            using (StreamWriter wr = new StreamWriter(filename))
            {
                _Export(board, wr);
            }
        }

        private bool ParseComment(string input)
        {
            Match match = commentPattern.Match(input);
            return match.Success;
        }

        private TekBoard ParseSize(string input)
        {
            int rows = 0, cols = 0;
            Match match = sizePattern.Match(input);

            if (match.Success &&
                Int32.TryParse(match.Groups["rows"].Value, out rows) &&
                Int32.TryParse(match.Groups["cols"].Value, out cols))
            {
                if (rows <= 0 || cols <= 0)
                    ParseError("Invalid size line {0}: ({1},{2})", input, rows, cols);
                return new TekBoard(rows, cols);
            }
            else
            {
                return null;
            }
        }

        private bool ParseAreaField(string rowS, string colS, TekBoard board, List<TekField> fields)
        {
            int row, col;
            if (Int32.TryParse(rowS, out row) && Int32.TryParse(colS, out col))
            {
                if (board.IsInRange(row, col))
                {
                    fields.Add(board.Fields[row, col]);
                    return true;
                }
            }
            return false;
        }
        private bool ParseArea(string input, TekBoard board)
        {
            List<TekField> fields = new List<TekField>();
            Match match = areaPattern1.Match(input);
            if (match.Success)
            {
                if (!ParseAreaField(match.Groups["row"].Value, match.Groups["col"].Value, board, fields))
                    ParseError("Invalid field in area line {0}: ({1},{2})", input, match.Groups["row"].Value, match.Groups["col"].Value);
                match = areaPattern2.Match(match.Groups["rest"].Value);
                while (match.Success)
                {
                    if (!ParseAreaField(match.Groups["row"].Value, match.Groups["col"].Value, board, fields))
                        ParseError("Invalid field in area line {0}: ({1},{2})", input, match.Groups["row"].Value, match.Groups["col"].Value);
                    match = match.NextMatch();
                }
            }
            if (fields.Count > 0)
            {
                board.DefineArea(fields);
                return true;
            }
            else
                return false;
        }

        private bool ParseValue(string input, TekBoard board)
        {
            int row, col, value;
            Match match = valuePattern.Match(input);
            if (match.Success &&
                Int32.TryParse(match.Groups["row"].Value, out row) &&
                Int32.TryParse(match.Groups["col"].Value, out col) &&
                Int32.TryParse(match.Groups["value"].Value, out value)
                )
            {
                if (!board.IsInRange(row, col) || value <= 0 || value > Const.MAXTEK)
                {
                    ParseError("Invalid value line {0}: ({1},{2}", input, row, col);
                }
                TekField field = board.Fields[row, col];
                field.Value = value;
                field.Initial = match.Groups["initial"].Value == "i";
                return true;
            }
            else
                return false;
        }

        private enum MultiValues { mvNotes, mvExcludes };
        static string[] MultiValueString = { @"notes", @"excludes" };

        private bool ParseMultiValues(string input, TekBoard board, MultiValues mvType)
        {
            int row, col, value;
            TekField field = null;
            Regex pattern1 = null, pattern2 = null;


            switch (mvType)
            {
                case MultiValues.mvNotes:
                    pattern1 = notesPattern1;
                    pattern2 = notesPattern2;
                    break;
                case MultiValues.mvExcludes:
                    pattern1 = excludesPattern1;
                    pattern2 = excludesPattern2;
                    break;
            }

            Match match = pattern1.Match(input);
            if (match.Success)
            {
                if (field == null && Int32.TryParse(match.Groups["row"].Value, out row) &&
                    Int32.TryParse(match.Groups["col"].Value, out col))
                {
                    if (!board.IsInRange(row, col))
                        ParseError("Invalid field in {0} line {1}: ({2},{3})", MultiValueString[(int)mvType], input, row, col);
                    field = board.Fields[row, col];
                }
                if (field != null && Int32.TryParse(match.Groups["value"].Value, out value))
                {
                    switch (mvType)
                    {
                        case MultiValues.mvNotes:
                            field.ToggleNote(value);
                            break;
                        case MultiValues.mvExcludes:
                            field.ExcludeValue(value);
                            break;
                    }
                    match = pattern2.Match(match.Groups["rest"].Value);
                    while (match.Success)
                    {
                        if (Int32.TryParse(match.Groups["value"].Value, out value))
                        {
                            switch (mvType)
                            {
                                case MultiValues.mvNotes:
                                    field.ToggleNote(value);
                                    break;
                                case MultiValues.mvExcludes:
                                    field.ExcludeValue(value);
                                    break;
                            }
                            match = match.NextMatch();
                        }
                        else
                            ParseError("Invalid value in {0} line {1}: ({2})", MultiValueString[(int)mvType], input, match.Groups["value"].Value);
                    }
                }
            }
            return field != null;
        }

        private bool ParseNotes(string input, TekBoard board)
        {
            return ParseMultiValues(input, board, MultiValues.mvNotes);
        }

        private bool ParseExcludes(string input, TekBoard board)
        {
            return ParseMultiValues(input, board, MultiValues.mvExcludes);
        }

        private void UpdatePossibleValues(TekBoard board)
        {
            foreach (TekField field in board.Fields)
                field.UpdatePossibleValues(false);
        }

        private TekBoard ParseStream(StreamReader sr)
        {
            string s;
            TekBoard board = null;
            while (!sr.EndOfStream)
            {
                s = sr.ReadLine();
                if (s.Trim() == "")
                    continue;
                else
                {
                    if (ParseComment(s))
                        continue;
                    if (board == null)
                    {
                        board = ParseSize(s);
                    }
                    else
                    {
                        if (!(ParseNotes(s, board) || ParseExcludes(s, board) || ParseArea(s, board) || ParseValue(s, board)))
                        {
                            ParseError("Error parsing line {0}", s);
                        }
                    }
                }
            }
            if (board != null)
            {
                UpdatePossibleValues(board);
            }
            return board;
        }
        private void ExportValue(TekField field, StreamWriter wr)
        {
            wr.WriteLine(VALUEFORMAT, field.Row, field.Col, field.Value, field.Initial ? "i" : "");
        }

        private void ExportArea(TekArea area, StreamWriter wr)
        {
            wr.Write(AREAFORMAT1);
            foreach (TekField field in area.Fields)
            {
                wr.Write(AREAFORMAT2, field.Row, field.Col);
            }
            wr.WriteLine();
        }

        private void ExportNotes(TekField field, StreamWriter wr)
        {
            wr.Write(NOTESFORMAT1, field.Row, field.Col);
            foreach (int value in field.Notes)
            {
                wr.Write(NOTESFORMAT2, value);
            }
            wr.WriteLine();
        }
        private void ExportExcludes(TekField field, StreamWriter wr)
        {
            wr.Write(EXCLUDESFORMAT1, field.Row, field.Col);
            foreach (int value in field.ExcludedValues)
            {
                wr.Write(EXCLUDESFORMAT2, value);
            }
            wr.WriteLine();
        }

        private void _Export(TekBoard board, StreamWriter wr)
        {
            wr.WriteLine(SIZEFORMAT, board.Rows, board.Cols);
            foreach (TekArea area in board.Areas)
            {
                ExportArea(area, wr);
            }
            foreach (TekField value in board.Fields)
            {
                if (value.Value > 0)
                    ExportValue(value, wr);
                if (value.Notes.Count > 0)
                    ExportNotes(value, wr);
                if (value.ExcludedValues.Count > 0)
                    ExportExcludes(value, wr);
            }
        }
    } // TekBoardParser
}
