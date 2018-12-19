using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.OCR;
using Emgu.CV.Util;

namespace TekScanner
{
    class TekBorderAnalyzer
    {
        const int EXTERNALBORDER = -114;
        public bool[,] TopAreaBorders;
        public bool[,] LeftAreaBorders;
        private int HorizontalThreshold, VerticalTreshold;
        private int[,] LeftBorderValues;
        private int[,] TopBorderValues;
        public int Rows { get { return TopAreaBorders.GetLength(0); } }
        public int Cols { get { return TopAreaBorders.GetLength(1); } }
        public TekBorderAnalyzer(UMat matGray, OCVGridDefinition gridDef)
        {
            Matrix<Byte> matrix = new Matrix<Byte>(matGray.Rows, matGray.Cols, matGray.NumberOfChannels);
            matGray.CopyTo(matrix);
            LeftBorderValues = FindRowValues(matrix, gridDef, (int)(gridDef.ColSize * 0.1));
            TopBorderValues  = FindColValues(matrix, gridDef, (int)(gridDef.RowSize * 0.1));
            LeftAreaBorders = AnalyzeBorderValues(LeftBorderValues, ref HorizontalThreshold);
            TopAreaBorders = AnalyzeBorderValues(TopBorderValues, ref VerticalTreshold);
        }

        private int[,] FindRowValues(Matrix<byte> matrix, OCVGridDefinition gridDef, int testWidth)
        {
            int[,] result = new int[gridDef.Rows, gridDef.Cols];
            int minVal, maxVal;
            for (int r = 0; r < gridDef.Rows; r++)
            {
                int rowLoc = (int)(gridDef.RowLocation(r) + gridDef.RowSize * 0.5);
                for (int c = 1; c < gridDef.Cols; c++)
                {
                    result[r, 0] = EXTERNALBORDER;
                    int loc = gridDef.ColLocation(c);
                    minVal = Int32.MaxValue;
                    maxVal = Int32.MinValue;
                    for (int col = loc - testWidth; col < loc + testWidth; col++)
                    {
                        if (col < 0 || col >= matrix.Cols)
                            continue;
                        byte value = matrix.Data[rowLoc, col];
                        if (value < minVal)
                            minVal = value;
                        if (value > maxVal)
                            maxVal = value;
                    }
                    result[r, c] = maxVal - minVal;
                }
            }
            return result;
        }
        private int[,] FindColValues(Matrix<byte> matrix, OCVGridDefinition gridDef, int testWidth)
        {
            int[,] result = new int[gridDef.Rows, gridDef.Cols];
            int minVal, maxVal;
            for (int c = 0; c < gridDef.Cols; c++)
            {
                int colLoc = (int)(gridDef.ColLocation(c) + gridDef.ColSize * 0.5);
                result[0, c] = EXTERNALBORDER;
                for (int r = 1; r < gridDef.Rows; r++)
                {
                    int loc = gridDef.RowLocation(r);
                    minVal = Int32.MaxValue;
                    maxVal = Int32.MinValue;
                    for (int row = loc - testWidth; row < loc + testWidth; row++)
                    {
                        if (row < 0 || row >= matrix.Rows)
                            continue;
                        byte value = matrix.Data[row, colLoc];
                        if (value < minVal)
                            minVal = value;
                        if (value > maxVal)
                            maxVal = value;                     
                    }
                    result[r, c] = maxVal - minVal;                        
                }
            }            
            return result;
        }

        private int GetThreshold(int[,] Values)
        {
            int rows = Values.GetLength(0);
            int cols = Values.GetLength(1);
            List<int> AllValues = new List<int>();
            foreach (int value in Values)
                if (value != EXTERNALBORDER && !AllValues.Contains(value))
                    AllValues.Add(value);
            AllValues.Sort();
            int result = AllValues.Min();
            int maxGap = 0;
            for (int i = 1; i < AllValues.Count; i++)
            {
                int gap = AllValues[i] - AllValues[i - 1];
                if (gap > maxGap)
                {
                    maxGap = gap;
                    result = AllValues[i - 1];
                }
            }
            return result;
        }
        private bool[,] AnalyzeBorderValues(int[,] BorderValues, ref int threshold)
        {
            int rows = BorderValues.GetLength(0);
            int cols = BorderValues.GetLength(1);
            bool[,] result = new bool[rows, cols];
            threshold = GetThreshold(BorderValues);
            for (int r = 0; r < rows; r++)
                for (int c = 0; c < cols; c++)
                    result[r, c] = (BorderValues[r, c] > threshold);
            return result;
        }

        private void DumpCell(StreamWriter sw, int row, int col, int lineno)
        {
            switch (lineno)
            {
                case 0: // top
                    if (row == 0 || TopAreaBorders[row - 1, col])
                        sw.Write("===");
                    else
                        sw.Write("...");
                    break;
                case 1: // middle
                    if (col == 0 || LeftAreaBorders[row, col - 1])
                        sw.Write("|");
                    else
                        sw.Write(".");
                    sw.Write(" ");
                    //right
                    if (col == Cols - 1 || LeftAreaBorders[row, col + 1])
                        sw.Write("|");
                    else
                        sw.Write(".");
                    break;
                case 2: //bottom
                    if (row == Rows-1 || TopAreaBorders[row, col])
                        sw.Write("===");
                    else
                        sw.Write("...");
                    break;
            }
        }
        public void Dump(StreamWriter sw)
        {
            sw.WriteLine("Left Area Border values (threshold pct {0}):", HorizontalThreshold);
            for (int r = 0; r < LeftAreaBorders.GetLength(0); r++)
            {
                sw.Write("row {0}:", r);
                for (int c = 0; c < LeftAreaBorders.GetLength(1); c++)
                    sw.Write("{0}{1}  ", LeftBorderValues[r, c], LeftAreaBorders[r, c] ? "L" : " ");
                sw.WriteLine();
            }
            sw.WriteLine("Top Area Border values (threshold pct {0}):", VerticalTreshold);
            for (int r = 0; r < TopAreaBorders.GetLength(0); r++)
            {
                sw.Write("row {0}:", r);
                for (int c = 0; c < TopAreaBorders.GetLength(1); c++)
                    sw.Write("{0}{1}  ", TopBorderValues[r, c], TopAreaBorders[r, c] ? "T" : " ");
                sw.WriteLine();
            }
            sw.WriteLine("Resultant matrix:");
            for (int r = 0; r < Rows; r++)
            {
                for (int l = 0; l < 2; l++)
                {
                    for (int c = 0; c < Cols; c++)
                    {
                        DumpCell(sw, r, c, l);
                    }
                    sw.WriteLine();
                }
            }
        }
    }
}
