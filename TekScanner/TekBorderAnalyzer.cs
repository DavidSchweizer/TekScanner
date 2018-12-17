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
        const int EXTERNALBORDER = 101;
        public bool[,] TopAreaBorders;
        public bool[,] LeftAreaBorders;
        public int Rows { get { return TopAreaBorders.GetLength(0); } }
        public int Cols { get { return TopAreaBorders.GetLength(1); } }
        public TekBorderAnalyzer(UMat matGray, OCVGridDefinition gridDef)
        {
            int testWidth = 10;
            int threshold;
            int[,] LeftBorderValues;
            int[,] TopBorderValues;

            Matrix<Byte> matrix = new Matrix<Byte>(matGray.Rows, matGray.Cols, matGray.NumberOfChannels);
            matGray.CopyTo(matrix);
            threshold = FindThresholdAndWidth(matrix, gridDef, out testWidth);
            LeftBorderValues = FindRowValues(matrix, gridDef, testWidth, threshold);
            TopBorderValues = FindColValues(matrix, gridDef, testWidth, threshold);
            using (StreamWriter sw = new StreamWriter("bordjes.log"))
            {
                sw.WriteLine("analyzing (threshold value {0})", threshold);
                LeftAreaBorders = AnalyzeBorderValues(LeftBorderValues, ref threshold);
                sw.WriteLine("Left Area Border values (threshold pct {0}):", threshold);
                for (int r = 0; r < LeftAreaBorders.GetLength(0); r++)
                {
                    sw.Write("row {0}:", r);
                    for (int c = 0; c < LeftAreaBorders.GetLength(1); c++)
                        sw.Write("{0}{1}  ", LeftBorderValues[r, c], LeftAreaBorders[r, c] ? "L" : " ");
                    sw.WriteLine();
                }
                TopAreaBorders = AnalyzeBorderValues(TopBorderValues, ref threshold);
                sw.WriteLine("Top Area Border values (threshold pct {0}):", threshold);
                for (int r = 0; r < TopAreaBorders.GetLength(0); r++)
                {
                    sw.Write("row {0}:", r);
                    for (int c = 0; c < TopAreaBorders.GetLength(1); c++)
                        sw.Write("{0}{1}  ", TopBorderValues[r, c], TopAreaBorders[r, c] ? "T" : " ");
                    sw.WriteLine();
                }
            }
        }

        private int FindThresholdAndWidth(Matrix<byte> matrix, OCVGridDefinition gridDef, out int testWidth)
        {
            const int MAXBIN = 10;
            double[] minValues = new double[1];
            double[] maxValues = new double[1];
            Point[] minLocations = new Point[1];
            Point[] maxLocations = new Point[1];
            testWidth = (int)(gridDef.ColSize * 0.1);
            int[] binLimits;
            int[] frequencyDistribution = FrequencyDistribution(matrix, MAXBIN, out binLimits);
            int i = MAXBIN;
            int maxi = i;
            while (--i >= 0)
            {
                if (frequencyDistribution[i] > frequencyDistribution[maxi])
                    maxi = i;
            }
            if (maxi > 0)
                return binLimits[maxi - 1];
            else
                return binLimits[0];
        }

        private int[] FrequencyDistribution(Matrix<byte> matrix, int nBins, out int[] binLimits)
        {
            int[] result = new int[nBins + 1];
            binLimits = new int[nBins + 1];
            for (int i = 0; i < nBins; i++)
                binLimits[i] = (i + 1) * (255 / nBins);
            binLimits[nBins] = 255;
            for (int r = 0; r < matrix.Rows; r++)
            { 
                for (int c = 0; c < matrix.Cols; c++)
                {
                    int i = 0;
                    while (i <= nBins)
                        if (matrix.Data[r, c] <= binLimits[i])
                        {
                            result[i]++;
                            break;
                        }
                        else i++;
                }
            }
            return result;
        }

        private int[,] FindRowValues(Matrix<byte> matrix, OCVGridDefinition gridDef, int testWidth, int threshold)
        {
            int[,] result = new int[gridDef.Rows, gridDef.Cols];
            for (int r = 0; r < gridDef.Rows; r++)
            {
                int rowLoc = (int)(gridDef.RowLocation(r) + gridDef.RowSize * 0.5);
                for (int c = 1; c < gridDef.Cols; c++)
                {
                    result[r, 0] = EXTERNALBORDER;
                    int loc = gridDef.ColLocation(c);
                    int nBelow = 0;
                    for (int col = loc - testWidth; col < loc + testWidth; col++)
                    {
                        if (col < 0 || col >= matrix.Cols)
                            continue;
                        byte value = matrix.Data[rowLoc, col];
                        if (value < threshold)
                            nBelow++;
                    }
                    result[r, c] = (int)(100 * (nBelow / (2.0 * testWidth)));
                }
            }
            return result;
        }
        private int[,] FindColValues(Matrix<byte> matrix, OCVGridDefinition gridDef, int testWidth, int threshold)
        {
            int[,] result = new int[gridDef.Rows, gridDef.Cols];

            for (int c = 0; c < gridDef.Cols; c++)
            {
                int colLoc = (int)(gridDef.ColLocation(c) + gridDef.ColSize * 0.5);
                result[0, c] = EXTERNALBORDER;
                for (int r = 1; r < gridDef.Rows; r++)
                {
                    int loc = gridDef.RowLocation(r);
                    int nBelow = 0;
                    for (int row = loc - testWidth; row < loc + testWidth; row++)
                    {
                        if (row < 0 || row >= matrix.Rows)
                            continue;
                        byte value = matrix.Data[row, colLoc];
                        if (value < threshold)
                            nBelow++;
                    }
                    result[r, c] = (int)(100 * (nBelow / (2.0 * testWidth)));
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
