using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace Tek1
{
    public class TekAreaDef
    {
        private List<Point> Points;
        private List<Point> Deltas;
        private int _xSize;
        private int _ySize;
        private int _xMin;
        private int _xMax;
        private int _yMin;
        private int _yMax;
        public int xSize { get { return _xSize; } } 
		public int ySize { get { return _ySize; } }
        public int xMin { get { return _xMin; } }
        public int xMax { get { return _xMax; } }
        public int yMin { get { return _yMin; } }
        public int yMax { get { return _yMax; } }

        public bool TemplateArea { get; set; }
        // set to true when used to generate symmetric versions, to be used when a simple list is sufficient
        private string _Description;
        public string Description { get { return _Description; } }
        // to help identify templates while debugging
        private void initLists()
        {
            Deltas = new List<Point>();
            Points = new List<Point>();
        }

        public Point GetPoint(int i)
		{
			return Points[i];
		}
		
		public TekAreaDef(TekAreaDef value)
        {
            initLists();
            foreach (Point point in value.Points)
                Points.Add(new Point(point.X, point.Y));
            Update();
            _Description = value.Description;
        }

        public int PointCount { get { return Points.Count;  } }

        public TekAreaDef(string description =  "", params Point[] values)
        {
            initLists();
            foreach (Point value in values)
            {
                if (Points.Count == Const.MAXTEK)
                    throw new Exception(String.Format("Too many values ({0})for area: already {1} fields present", values.Length, Points.Count));
                Points.Add(value);
            }
            Update();
            _Description = description;
        }

        private void ComputeDeltas()
        {
            Deltas.Clear();
            if (Points.Count == 0)
                return;
            Point P = Points[0];
            for (int i = 1; i < Points.Count; i++)
                Deltas.Add(new Point(Points[i].X - P.X, Points[i].Y - P.Y));
        }

        private void ComputeSize()
        {
            _xMin = xMinimum();
            _xMax = xMaximum();
            _yMin = yMinimum();
            _yMax = yMaximum();
            if (Points.Count() == 0)
            {
                _xSize = 0;
                _ySize = 0;
            }
            else 
            {
                _xSize = 1 + xMax - xMin;
                _ySize = 1 + yMax - yMin;
            }
        }

        private void Update()
        {
            ComputeSize();
            ComputeDeltas();
        }

        private int xMinimum()
        {
            if (Points.Count == 0)
                return 0;
            int result = Int32.MaxValue;
            foreach (Point value in Points)
            {
                if (value.X < result)
                    result = value.X;
            }
            return result;
        }

        private int yMinimum()
        {
            if (Points.Count == 0)
                return 0;
            int result = Int32.MaxValue;
            foreach (Point value in Points)
            {
                if (value.Y < result)
                    result = value.Y;
            }
            return result;
        }

        private int xMaximum()
        {
            if (Points.Count == 0)
                return 0;
            int result = -1;
            foreach (Point value in Points)
            {
                if (value.X > result)
                    result = value.X;
            }
            return result;
        }

        private int yMaximum()
        {
            if (Points.Count == 0)
                return 0;
            int result = -1;
            foreach (Point value in Points)
            {
                if (value.Y > result)
                    result = value.Y;
            }
            return result;
        }

        public void Shift(int DeltaX, int DeltaY)
        {
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i] = new Point(Points[i].X + DeltaX, Points[i].Y + DeltaY);
            }
        }

        public TekAreaDef Normalized()
        {
            TekAreaDef result = new TekAreaDef(this);
            result.ComputeDeltas();
            result.Shift(-result.xMin, -result.yMin);
            result.ComputeSize();
            return result;
        }
        
        public bool Equals(TekAreaDef other)
        {
            TekAreaDef N1 = this.Normalized();
            TekAreaDef N2 = other.Normalized();
            if (N1.Points.Count != N2.Points.Count)
                return false;
            foreach (Point P1 in N1.Points)
                if (!N2.Points.Contains(P1))
                    return false;
            return true;
        }


        private void AddPoint(Point P)
        {
            Points.Add(P);
            Update();
        }

        public TekAreaDef FlipVertical()
        {
            TekAreaDef result = new TekAreaDef(String.Format("{0} vert flip", Description));
            for (int i = 0; i < Points.Count; i++)
            {
                Point P = Points[i];
                result.AddPoint(new Point(P.X, yMax - P.Y));
            }
            return result.Normalized();
        }

        public TekAreaDef FlipHorizontal()
        {
            TekAreaDef result = new TekAreaDef(String.Format("{0} hori flip", Description));
            for (int i = 0; i < Points.Count; i++)
            {
                Point P = Points[i];
                result.AddPoint(new Point(xMax - P.X, P.Y));
            }
            return result.Normalized();
        }

        public TekAreaDef Rotate90()
        {
            TekAreaDef result = new TekAreaDef(String.Format("{0} rot 90", Description), this.Points[0]);
            foreach (Point value in this.Deltas)
            {
                result.Deltas.Add(new Point(-value.Y, value.X));
                Point p = result.Deltas.ElementAt(result.Deltas.Count - 1);
                result.AddPoint(new Point(this.Points[0].X + p.X, this.Points[0].Y + p.Y));
            }
            return result.Normalized();
        }

        public TekAreaDef Rotate180()
        {
            return Rotate90().Rotate90();
        }

        public void Dump(StreamWriter sw)
        {
            sw.Write(String.Format("{0}  [(size:({1},{2})]\n\tpoints: ", Description, xSize, ySize));
            foreach (Point value in Points)
            {
                sw.Write(" ({0},{1})", value.X, value.Y);
            }
            sw.Write("\n\tdeltas: ");
            foreach (Point value in Deltas)
            {
                sw.Write(" ({0},{1})", value.X, value.Y);
            }
            sw.WriteLine();
        }

        public bool IsInList(List<TekAreaDef> list)
        {
            foreach (TekAreaDef value in list)
                if (this.Equals(value))
                    return true;
            return false;
        }

        public bool canFit(List<Point> list)
        {
            int x0 = Int32.MaxValue, y0 = Int32.MaxValue;
            int x1 = -1, y1 = -1;
            foreach (Point point in list)
            {
                if (point.X < x0)
                    x0 = point.X;
                if (point.Y < y0)
                    y0 = point.Y;
                if (point.X > x1)
                    x1 = point.X;
                if (point.Y > y1)
                    y1 = point.Y;
            }
            foreach (Point point in Points)
                if (!list.Contains(new Point(x0 + point.X, y0 + point.Y)))
                    return false;
            return true;
        }

        private void AddAlternative(TekAreaDef area, List<TekAreaDef> list)
		// note: includes mirror images as well
        {
            if (!area.IsInList(list))
                list.Add(area);
            TekAreaDef area2 = area.FlipHorizontal();
            if (!area2.IsInList(list))
                list.Add(area2);
            area2 = area.FlipVertical();
            if (!area2.IsInList(list))
                list.Add(area2);
        }

        public List<TekAreaDef> GetAlternatives()
        {
            List<TekAreaDef> result = new List<TekAreaDef>();
            TekAreaDef thisArea = this.Normalized();
            thisArea.TemplateArea = this.TemplateArea;
            AddAlternative(thisArea, result);
            // rotations
            AddAlternative(this.Rotate90(), result);
            AddAlternative(this.Rotate180(), result);
            AddAlternative(this.Rotate180().Rotate90(), result);
            return result;
        }

        public string[] AsAsciiArt(char NoChar = '.', char ShowChar = 'X')
        {
            string[] result = new string[ySize];
            for (int i = 0; i < ySize; i++)
                result[i] = new String(NoChar, xSize);
            for (int i = 0; i < Points.Count; i++)
            {
                int xPos = Points[i].X - xMin;
                int yPos = Points[i].Y - yMin;
                result[yPos] = result[yPos].Substring(0, xPos) + ShowChar + 
                    result[yPos].Substring(xPos + 1, xSize - xPos-1);
            }
            return result;
        }

        public void DumpAsAsciiArt(StreamWriter sw, char NoChar = '.', char ShowChar = 'X')
        {
            string[] art = AsAsciiArt(NoChar, ShowChar);
            foreach (string s in art)
                sw.WriteLine(s);
            sw.Flush();
        }
    }

    public class TekStandardAreas
    {
        private List<TekAreaDef> values;

        public TekAreaDef GetValue(int i) 
        {
            return values[i];
        }
        public int Count { get { return values.Count; } }
        public int nCount(int nFields)
        {
            int result = 0;
            foreach (TekAreaDef value in values)
                if (value.PointCount == nFields)
                    result++;
            return result;
        }

        public TekAreaDef GetValue(int i, int nFields)
        {
            int j = i;
            TekAreaDef result = null;
            foreach(TekAreaDef value in values)
            {
                if (value.PointCount != nFields)
                    continue;
                if (j == 0)
                {
                    result = value;
                    break;
                }
                j--;
            }
            return result;
        }

        private void AddAlternatives(TekAreaDef value)
        {
            value.TemplateArea = true;
            foreach (TekAreaDef val in value.GetAlternatives())
                if (!val.IsInList(values))
                    values.Add(val);
        }
        private void Add1FieldAreas()
        {
            AddAlternatives(new TekAreaDef("Size: 1", new Point(0, 0)));           
        }
        private void Add2FieldAreas()
        {
            AddAlternatives(new TekAreaDef("Size: 2", new Point(0, 0), new Point(0, 1)));
        }
        private void Add3FieldAreas()
        {
            // straight
            AddAlternatives(new TekAreaDef("Size: 3 (straight)", new Point(0, 0), new Point(0, 1), new Point(0, 2)));
            // cornered
            AddAlternatives(new TekAreaDef("Size: 3 (cornered)", new Point(0, 0), new Point(0, 1), new Point(1, 0)));
        }
        private void Add4FieldAreas()
        {
            // straight
            AddAlternatives(new TekAreaDef("Size: 4 (straight)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3)));
            // cornered 1
            AddAlternatives(new TekAreaDef("Size: 4 (cornered v1)",  new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 2)));
            // cornered 2
            AddAlternatives(new TekAreaDef("Size: 4 (cornered v2)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1)));
            // big square
            AddAlternatives(new TekAreaDef("Size: 4 (square)", new Point(0, 0), new Point(0, 1), new Point(1, 0), new Point(1, 1)));
        }
        private void Add5FieldAreas()
        {
            // straight
            AddAlternatives(new TekAreaDef("Size: 5 (straight)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(0,4)));
            // cornered 1
            AddAlternatives(new TekAreaDef("Size: 5 (cornered v1)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(1, 3)));
            // cornered 2
            AddAlternatives(new TekAreaDef("Size: 5 (cornered v2)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1), new Point(1, 2)));
            // cornered 3
            AddAlternatives(new TekAreaDef("Size: 5 (cornered v3)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 2), new Point(2, 2)));
            // cornered 4
            AddAlternatives(new TekAreaDef("Size: 5 (cornered v4)", new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 1), new Point(2, 1)));
            // cross 
            AddAlternatives(new TekAreaDef("Size: 5 (cross)", new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(0, 1), new Point(2, 1)));
            // windmill
            AddAlternatives(new TekAreaDef("Size: 5 (windmill)", new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(1, 2), new Point(2, 2)));

        }

        public TekStandardAreas()
        {
            values = new List<TekAreaDef>();
            Add1FieldAreas();
            Add2FieldAreas();
            Add3FieldAreas();
            Add4FieldAreas();
            Add5FieldAreas();
        }

        public List<TekAreaDef> TemplateAreas()
        {
            List<TekAreaDef> result = new List<TekAreaDef>();
            foreach (TekAreaDef value in values)
                if (value.TemplateArea)
                    result.Add(value);
            return result;
        }

        public List<TekAreaDef> AreasFromTemplate(TekAreaDef template)
        {
            List<TekAreaDef> result = new List<TekAreaDef>();
            int index0 = values.IndexOf(template);
            if (index0 != -1 && values.ElementAt(index0).TemplateArea)
            {
                result.Add(template);
                for (int i = index0 + 1; i < values.Count && !values.ElementAt(i).TemplateArea; i++)
                    result.Add(values.ElementAt(i));
            }
            return result;
        }



        public List<TekAreaDef> FittingAreas(List<Point> list, int nPoints)
        {
            List<TekAreaDef> result = new List<TekAreaDef>();
            foreach (TekAreaDef area in values)
            {
                if (area.PointCount == nPoints && area.canFit(list))
                    result.Add(area);
            }
            return result;
        }


    }
}
