using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tek1;

namespace TekScanner
{
    class TekBoardAnalyzer
    {
        public TekBoard Board;
        private TekBorderAnalyzer Borders;
        private TekCharacterRecognition Characters;
        public TekBoardAnalyzer(TekGridAnalyzer GridAnalyzer)
        {
            Borders = new TekBorderAnalyzer(GridAnalyzer.matGrayScaleImage, GridAnalyzer.gridDef);
            Characters = new TekCharacterRecognition(GridAnalyzer.matGrayScaleImage, GridAnalyzer.gridDef);
            Board = new TekBoard(GridAnalyzer.gridDef.Rows, GridAnalyzer.gridDef.Cols);
            InsertValues();
            InsertAreas();
        }
        private void InsertValues()
        {
            for (int r = 0; r < Board.Rows; r++)
                for (int c = 0; c < Board.Cols; c++)
                    if (Characters.Values[r, c] > 0)
                    {
                        Board.Fields[r, c].Value = Characters.Values[r, c];
                        Board.Fields[r, c].Initial = true;
                    }
        }
        private void HandleField(int row, int col, TekFields CurrentAreaFields)
        {
            if (row >= Board.Rows || col >= Board.Cols || Board.Fields[row, col].Area != null ||
                CurrentAreaFields.Fields.Contains(Board.Fields[row, col]))
                return;
            else
            {
                CurrentAreaFields.AddField(Board.Fields[row, col]);

                if (col < Board.Cols-1 && !Borders.LeftAreaBorders[row, col+1])
                    HandleField(row, col + 1, CurrentAreaFields);
                if (col > 0 && !Borders.LeftAreaBorders[row, col])
                    HandleField(row, col - 1, CurrentAreaFields);
                if (row < Board.Rows-1 && !Borders.TopAreaBorders[row+1,col])
                    HandleField(row+1, col, CurrentAreaFields);
                if (row > 0 && !Borders.TopAreaBorders[row, col])
                    HandleField(row - 1, col, CurrentAreaFields);
            }
        }
        private bool NextRowCol(ref int row, ref int col)
        {
            for (int r = 0; r < Board.Rows; r ++)
                for (int c = 0; c < Board.Cols; c++)
                    if (Board.Fields[r, c].Area == null)
                    {
                        row = r;
                        col = c;
                        return true;
                    }
            return false;
        }
        private bool InsertAreas()
        {
            int r = 0;
            int c = 0;
            TekFields CurrentFields = new TekFields();
            while (!Board.IsValidAreas())
            {
                HandleField(r, c, CurrentFields);
                if (CurrentFields.Fields.Count > 0)
                    Board.DefineArea(CurrentFields.Fields);
                CurrentFields.Fields.Clear();
                if (!NextRowCol(ref r, ref c))
                    return false;
            }
            return true;
        }
    }
}
