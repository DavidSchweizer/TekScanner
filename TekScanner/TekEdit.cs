using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Tek1
{
    class TekEdit : TekView
    {
        TekStandardAreas StandardAreas;
        List<TekAreaDef> TemplateAreas;
        Random R = new Random();

        public TekEdit(Control parent, Point TopLeft, Point BottomRight) : base(parent, TopLeft, BottomRight)
        {
            // tbd
            TekFieldView.IgnoreInitial = true;
            StandardAreas = new TekStandardAreas();
            TemplateAreas = StandardAreas.TemplateAreas();
            //using (StreamWriter sw = new StreamWriter("areas.log"))
            //{
            //    int LastTemplate = -1;
            //    for (int i = 0; i < StandardAreas.Count; i++)
            //    {
            //        TekAreaDef area = StandardAreas.GetValue(i);
            //        if (area.TemplateArea)
            //        {
            //            sw.WriteLine("TEMPLATE area {0} --- {1}:", i, area.Description);
            //            LastTemplate = i;
            //        }
            //        else
            //            sw.WriteLine("---{0} ({1}) --- {2}:", i, LastTemplate, area.Description);
            //        area.DumpAsAsciiArt(sw);
            //        //area.Dump(sw);
            //    }
            //}
        }

        public void ResetBoard()
        {
            if (Board == null)
                return;
            int i = Board.Areas.Count - 1;
            while (i >= 0)
            { 
                Board.DeleteArea(Board.Areas.ElementAt(i--));
            }
            SetBoard(Board);
        }

        public void ResizeBoard(int rows, int cols)
        {
            if (Board == null)
                return;
            Board.Resize(rows, cols);
            SetBoard(Board);
        }

        private void UpdateArea(TekArea area)
        {
            _view.SetAreaColors(Board);
            foreach (TekField field in area.Fields)
            {
                TekFieldView view = _view.GetField(field.Row, field.Col);
                _view.SetPanelColors(view);
                _view._SetBorders(view);
            }
            _view.Refresh();
        }

        public void DeleteArea(TekArea area)
        {
            if (area == null)
                return;
            Board.DeleteArea(area);
            UpdateArea(area);
        }

        public TekArea SelectArea(int row, int col)
        {
            TekFieldView view = _view.GetField(row, col);
            Selector.CurrentMode = TekSelect.SelectMode.smMultiple;
            Selector.MultiselectFieldView.Clear();
            if (view == null)
                return null;
            TekArea area = view.Field.Area;
            if (area == null)
                return null;
            foreach (TekField field in area.Fields)
            {
                Selector.SelectCurrentField(_view.GetField(field.Row, field.Col));
            }
            _view.Refresh();
            return area;
        }

        private List<TekField> GetAreaFields(Point TopLeft, TekAreaDef sArea)
        {
            List<TekField> fields = new List<TekField>();
            for (int i = 0; i < sArea.PointCount; i++)
            {
                Point P = sArea.GetPoint(i);
                fields.Add(Board.Fields[TopLeft.Y + P.Y, TopLeft.X + P.X]);
            }
            return fields;
        }

        private void AddAreaToBoard(Point TopLeft, TekAreaDef sArea)
        {
            UpdateArea(Board.DefineArea(GetAreaFields(TopLeft, sArea)));
        }

        public bool canFit(Point TopLeft, TekAreaDef sArea)
        {
            if (TopLeft.X + sArea.xSize > Board.Cols || TopLeft.Y + sArea.ySize > Board.Rows)
                return false;
            foreach (TekField field in GetAreaFields(TopLeft, sArea))
                if (field.Area != null)
                    return false;
            return true;
        }

        private Point FirstEmptyPoint(int r0, int c0)
        {

            int r = r0, c = c0;
            while (r < Board.Rows && c < Board.Cols)
            {
                if (Board.Fields[r, c].Area == null)
                    break;
                c++;
                if (c >= Board.Cols)
                {
                    c = 0; r++;
                }
            }
            if (r < Board.Rows && c < Board.Cols)
                return new Point(c, r);
            else
                return new Point(-1, -1);
        }

        private List<Point> FirstEmptyArea()
        {

            List<Point> result = new List<Point>();
            Point P = FirstEmptyPoint(0, 0);
            if (P.X != -1 && P.Y != -1)
            {
                int r0 = P.Y, c0 = P.X;
                int r1 = r0, c1 = c0;
                // go to the right as far as possible and then go down as far as possible
                while (r1 < Board.Rows && c1 < Board.Cols && Board.Fields[r1, c1].Area == null)
                {
                    c1++;
                }
                int c = c0;
                r1++;      
                while (r1 < Board.Rows && c < c1 && Board.Fields[r1, c].Area == null)
                {
                    r1++;
                }
                // now add all candidate points in this area
                for (int r = r0; r < r1; r++)
                    for (c = c0; c < c1; c++)
                    {
                        if (Board.Fields[r, c].Area == null)
                            result.Add(new Point(c, r));
                    }
            }
            return result;
        }

        private Point TopLeftPoint(List<Point> points)
        {
            if (points.Count == 0)
                return new Point(-1,-1);
            int index = 0;
            int xMin = points[index].X;
            int yMin = points[index].Y;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X <= xMin && points[i].Y <= yMin)
                {
                    index = i;
                    xMin = points[index].X;
                    yMin = points[index].Y;
                }
            }
            return points[index];
        }

        public bool AddRandomArea()
        {
            // find next open area
            List<Point> areaPoints = FirstEmptyArea();
            if (areaPoints.Count == 1) // one point only
            {
                AddAreaToBoard(areaPoints.ElementAt(0), StandardAreas.GetValue(0));
                return true;
            }
            int nPoints = areaPoints.Count >= Const.MAXTEK ? Const.MAXTEK : areaPoints.Count;

            List<TekAreaDef> possibleMatches = StandardAreas.FittingAreas(areaPoints, nPoints);
            while (nPoints > 0)
            {
                if (possibleMatches.Count > 0)
                { // there is at least one match, but we still should check it fits in case the fields are already occupied

                    int index0 = R.Next(0, possibleMatches.Count), index;
                    Point topLeft = TopLeftPoint(areaPoints);
                    index = index0;
                    do
                    {
                        TekAreaDef area = possibleMatches.ElementAt(index);
                        if (canFit(topLeft, area))
                        {
                            AddAreaToBoard(topLeft, area);
                            return true;
                        }
                        else
                        {
                            index = (index + 1) % possibleMatches.Count; 
                        }
                    } while (index != index0);
                    return false;
                }
                if (--nPoints > 0)
                    possibleMatches = StandardAreas.FittingAreas(areaPoints, nPoints);
            }            
            return false;
        }

        public void FillRandomAreas()
        {
            while (!Board.IsValidAreas())
                AddRandomArea();
        }
    }
}
