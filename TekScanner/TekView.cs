using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Threading.Tasks;


namespace Tek1
{
    public delegate void PlayAction(int row, int col, TekMove move, int value);

    public class TekView
    {
        public PlayAction PlayActionHandler;
        protected TekBoardView _view;
        public TekBoard Board { get { return _view.Board; } set { SetBoard(value); } }
        public TekMoves Moves = null;

        public TekSelect Selector { get { if (_view == null) return null; return _view.Selector; } }


        public TekView(Control parent, Point TopLeft, Point BottomRight)
        {
            _view = new TekBoardView();
            parent.Controls.Add(_view);
            _view.Top = TopLeft.Y;
            _view.Left = TopLeft.X;
            _view.Width = BottomRight.X - TopLeft.X;
            _view.Height = BottomRight.Y - TopLeft.Y;
        }

        public void SetBoard(TekBoard board)
        {
            _view.Board = board;
            Moves = new TekMoves(board);
        }

        public void SetSize(int width, int height)
        {
            Width = width - TekBoardView.PADDING;
            Height = height - TekBoardView.PADDING;
        }

        public bool LoadFromFile(string FileName)
        {
            TekBoardParser tbp = new TekBoardParser();
            TekBoard board = null;
            try
            {
                board = tbp.Import(FileName);
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
            if (board != null)
            {
                SetBoard(board);
                return true;
            }
            else
                return false;
        }

        public bool SetShowErrors(bool onoff = true)
        {
            _view.SetShowErrors(onoff);
            return onoff;
        }

        public bool SaveToFile(string FileName)
        {
            if (_view.Board == null)
                return false;
            TekBoardParser tbp = new TekBoardParser();
            tbp.Export(_view.Board, FileName);
            return true;
        }

        public bool ShowDefaultNotes()
        {
            if (Board == null)
                return false;
            _view.ShowDefaultNotes();
            return true;
        }

        public bool ToggleSelectedValue(int value)
        {
            if (Board != null && _view.Selector.CurrentFieldView != null)
            {
                Moves.PlayValue(_view.Selector.CurrentFieldView.Row, _view.Selector.CurrentFieldView.Col, value);
                _view.Refresh();
                PlayActionHandler?.Invoke(_view.Selector.CurrentFieldView.Row, _view.Selector.CurrentFieldView.Col, TekMove.tmValue, value);
                return true;
            }
            else
                return false;
        }

        public bool Solve()
        {
            if (Board == null)
                return false;

            // rewrite this with heuristic solver
            //TekSolver solver = new TekSolver(Board);
            bool result = false; // solver.Solve();

            _view.Refresh();

            return result;
        }

        public bool ResetValues()
        {
            if (Board == null)
                return false;
            Board.ResetValues();
            if (Board.AutoNotes)
                Board.SetDefaultNotes();
            Moves.Clear();
            _view.Refresh();
            return true;
        }

        public void Refresh()
        {
            _view.Refresh();
        }
        public bool ToggleSelectedNoteValue(int value)
        {
            if (Board != null && _view.Selector.CurrentFieldView != null)
            {
                _view.Selector.ToggleSelectedNoteValue(value, Moves);
                PlayActionHandler?.Invoke(_view.Selector.CurrentFieldView.Row, _view.Selector.CurrentFieldView.Col, TekMove.tmNote, value);
                return true;
            }
            else return false;
        }

        public bool UnPlay()
        {
            if (Moves != null)
            {
                Moves.UnPlay();
                _view.Refresh();
                return true;
            }
            else
                return false;
        }

        public int SnapshotCount()
        {
            if (Moves == null)
                return 0;
            else
                return Moves.SnapshotCount();
        }

        public bool TakeSnapshot(string name)
        {
            if (Moves != null)
            {
                Moves.TakeSnapshot(name);
                return true;
            }
            else
                return false;
        }

        public bool RestoreSnapshot(string name)
        {
            if (Moves != null)
            {
                Moves.RestoreSnapshot(name);
                _view.Refresh();
                return true;
            }
            else
                return false;

        }

        public void MoveSelected(int deltaR, int deltaC)
        {
            TekFieldView v = _view.Selector.CurrentFieldView;
            if (v == null)
                return;
           _view.SelectField(v.Field.Row + deltaR, v.Field.Col + deltaC);
           // v.Refresh();
        }
        public void SelectFields(params TekField[] fields)
        {
            _view.Selector.SetCurrentMode(TekSelect.SelectMode.smMultiple);
            _view.Selector.ClearMultiSelect();
            foreach (TekField field in fields)
                _view.SelectField(field.Row, field.Col);
            _view.Refresh();
        }
        public void SelectFields(List <TekField> fields, bool onoff = true)
        {
            _view.Selector.ClearMultiSelect();
            if (onoff)
            {
                _view.Selector.SetCurrentMode(TekSelect.SelectMode.smMultiple);
                foreach (TekField field in fields)
                    _view.SelectField(field.Row, field.Col);
            }
            _view.Refresh();
        }

        public void HighlightFields(List<TekField> fields, bool onoff = true)
        {
            foreach (TekField field in fields)
                _view.HighlightField(field.Row, field.Col, onoff);
            _view.Refresh();
        }
        public void HandleKeyDown(ref Message msg, Keys keyData)
        {
            if (_view.Selector.CurrentFieldView != null)
            {
                switch (keyData)
                {
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                        ToggleSelectedValue(1 + keyData - Keys.D1);
                        break;
                    case Keys.Alt | Keys.D1:
                    case Keys.Alt | Keys.D2:
                    case Keys.Alt | Keys.D3:
                    case Keys.Alt | Keys.D4:
                    case Keys.Alt | Keys.D5:
                        ToggleSelectedNoteValue(1 + (int)keyData - Keys.Alt - Keys.D1);
                        break;
                    case Keys.Up:
                        MoveSelected(-1, 0);
                        break;
                    case Keys.Down:
                        MoveSelected(1, 0);
                        break;
                    case Keys.Left:
                        MoveSelected(0, -1);
                        break;
                    case Keys.Right:
                        MoveSelected(0, 1);
                        break;
                    case Keys.Back:
                    case Keys.Control | Keys.Z:
                        UnPlay();
                        break;
                }
            }
            
        }

        public int Width { get { return _view.Width; } set { _view.Width = value; } }
        public int Height { get { return _view.Height; } set { _view.Height = value; } }

    }

    public class TekSelect
    {
        public enum SelectMode { smNone, smSingle, smMultiple };
        private TekFieldView _currentFieldView = null;
        public TekFieldView CurrentFieldView { get { return _currentFieldView; } }
        public List<TekFieldView> MultiselectFieldView = new List<TekFieldView>();

        private SelectMode _currentMode = SelectMode.smSingle;
        public SelectMode CurrentMode { get { return _currentMode; } set { SetCurrentMode(value); } }

        public void SelectCurrentField(TekFieldView newfield)
        {
            if (newfield == null)
                CurrentMode = SelectMode.smNone;
            else
            {
                if (CurrentMode == SelectMode.smNone)
                    _currentMode = SelectMode.smSingle;
                switch (CurrentMode)
                {
                    case SelectMode.smSingle:
                        if (_currentFieldView != null && _currentFieldView.IsSelected)
                            _currentFieldView.SetSelected(false);
                        newfield.SetSelected(!newfield.IsSelected);
                        foreach (TekFieldView field in MultiselectFieldView)
                        {
                            field.SetMultiSelected(false);
                        }
                        MultiselectFieldView.Clear();
                        break;
                    case SelectMode.smMultiple:
                        newfield.SetMultiSelected(!newfield.IsMultiSelected);
                        if (!MultiselectFieldView.Contains(newfield))
                            MultiselectFieldView.Add(newfield);
                        else
                            MultiselectFieldView.Remove(newfield);
                        break;
                }
                _currentFieldView = newfield;
            }
        }

        
        public void SetCurrentMode(SelectMode value)
        {
            if (CurrentMode == value)
                return;

            if (_currentFieldView != null)
                switch (CurrentMode)
                {
                    case SelectMode.smNone:
                        _currentFieldView.SetSelected(false);
                        _currentFieldView.SetMultiSelected(false);
                        break;
                    case SelectMode.smSingle:
                        _currentFieldView.SetSelected(false);
                        break;
                    case SelectMode.smMultiple:
                        _currentFieldView.SetMultiSelected(false);
                        break;
                }
            _currentMode = value;
            if (_currentFieldView != null)
            {
                switch (CurrentMode)
                {
                    case SelectMode.smNone:
                        _currentFieldView = null;
                        break;
                    case SelectMode.smSingle:
                        SelectCurrentField(_currentFieldView);
                        break;
                    case SelectMode.smMultiple:
                        _currentFieldView.SetSelected(true);
                        break;
                }
                if (_currentFieldView != null)
                    _currentFieldView.Refresh();
            }
        }

        public void ClearMultiSelect()
        {
            foreach (TekFieldView v in MultiselectFieldView)
                v.SetMultiSelected(false);
            MultiselectFieldView.Clear();
        }
        public void Reset()
        {
            CurrentMode = SelectMode.smNone;
            _currentFieldView = null;
            MultiselectFieldView.Clear();
        }

        private void _toggleFieldNoteValue(TekFieldView view, int value, TekMoves Moves)
        {
            if (view != null)
            {
                Moves.PlayNote(view.Row, view.Col, value);
                view.Refresh();
            }
        }
        public void ToggleSelectedNoteValue(int value, TekMoves Moves)
        {
            switch (CurrentMode)
            {
                case SelectMode.smSingle:
                    _toggleFieldNoteValue(CurrentFieldView, value, Moves);
                    break;
                case SelectMode.smMultiple:
                    foreach(TekFieldView fieldView in MultiselectFieldView)
                    {
                        _toggleFieldNoteValue(fieldView, value, Moves);                        
                    }
                    break;
            } 
        }

}

    public class TekBoardView : Panel
    {
        static System.Drawing.Color[] AreaColors =
            { Color.LightGreen, Color.Orange, Color.LightSkyBlue,
              Color.LightPink, Color.LightYellow, Color.LightSalmon,
              Color.LightGray, Color.Beige, Color.DeepPink
            };
        static System.Drawing.Color[] SelectedAreaColors =
             { Color.MediumSeaGreen, Color.DarkOrange, Color.DeepSkyBlue,
              Color.HotPink, Color.Goldenrod, Color.Salmon,
              Color.SlateGray, Color.SaddleBrown, Color.Fuchsia
            };
        static System.Drawing.Color NoAreaColor = Color.BlanchedAlmond;
        static System.Drawing.Color NoAreaSelectedColor = Color.BlueViolet;
        const int MAXCOLOR = 9;
        static int[] AreaColorIndex = null;
        public TekSelect Selector = new TekSelect();

        private TekFieldView[,] _Panels = null;

        private TekPanelData data = new TekPanelData();

        private TekBoard board = null;
        public TekBoard Board { get { return board; } set { SetBoard(value); } }
        private void SetBoard(TekBoard value)
        {
            board = value;
            SetAreaColors(board);
            Selector.Reset();
            initializePanels();
            SetBorders();
        }

        private void removeBoard()
        {
            if (_Panels == null)
                return;
            Selector.Reset();
            for (int r = 0; r < _Panels.GetLength(0); r++)
                for (int c = 0; c < _Panels.GetLength(1); c++)
                {
                    this.Controls.Remove(_Panels[r, c]);
                    _Panels[r, c] = null;
                }
            _Panels = null;
        }

        public TekBoardView()
        {
            this.Resize += Board_Resize;
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            if (Board == null)
                return;
            if (sender is TekFieldView)
            {
                Selector.SelectCurrentField(sender as TekFieldView);
            }
        }

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (Board == null)
                return;
            if (sender is TekFieldView)
            {
                Selector.SelectCurrentField(sender as TekFieldView);
            }
        }

        private void Panel_MouseClick(object sender, MouseEventArgs e)
        {
            if (Board == null)
                return;
            if (sender is TekFieldView)
            {
                Selector.SelectCurrentField(sender as TekFieldView);
            }
        }

        private void Board_Resize(object sender, EventArgs e)
        {
            if (Board != null)
            {
                data.TileSize = ComputeTileSize(ClientRectangle.Width, ClientRectangle.Height);
                for (int r = 0; r < Board.Rows; r++)
                    for (int c = 0; c < Board.Cols; c++)
                        ReSizeFieldView(_Panels[r, c], data.TileSize, r, c);
                Refresh();
            }
                
        }
        public const int PADDING = 6;

        private int ComputeTileSize(int width, int height)
        {
            if (Board != null)
                return Math.Min((width - PADDING) / Board.Cols, (height - PADDING) / Board.Rows);
            else
                return 0;
        }

        private void ReSizeFieldView(TekFieldView v, int TileSize, int r, int c)
        {
            v.Size = new Size(TileSize, TileSize);
            v.Location = new Point(PADDING / 2 + data.TileSize * c, PADDING / 2 + data.TileSize * r);
       }

        public void SetPanelColors(TekFieldView panel)
        {
            TekArea area = panel.Field.Area;
            if (area == null)
            {
                panel.NormalColor = NoAreaColor;
                panel.SelectedColor = NoAreaSelectedColor;
            }
            else
            {
                panel.NormalColor = AreaColors[AreaColorIndex[area.AreaNum]];
                panel.SelectedColor = SelectedAreaColors[AreaColorIndex[area.AreaNum]];
            }
        }
        private void initializePanels()
        {
           
            var clr1 = Color.DarkGray;
            var clr2 = Color.White;
            Random R = new Random();

            removeBoard();
            if (Board == null)
                return;

            data.TileSize = ComputeTileSize(this.ClientRectangle.Width, this.ClientRectangle.Height);

            _Panels = new TekFieldView[Board.Rows, Board.Cols];

            for (int r = 0; r < Board.Rows; r++)
                for (int c = 0; c < Board.Cols; c++)
                {
                    TekFieldView newP = new TekFieldView();
                    ReSizeFieldView(newP, data.TileSize, r, c);
                    
                    newP.Data = data;
                    newP.Field = Board.Fields[r, c];
                    SetPanelColors(newP);

                    //newP.Click += new EventHandler(Panel_Click);
                    //newP.MouseClick += new MouseEventHandler(Panel_MouseClick);
                    newP.MouseClick += new MouseEventHandler(Panel_MouseDown);
                    //(newP as Control).KeyDown+= new EventHandler(Panel_KeyDown);

                    this.Controls.Add(newP);
                    _Panels[r, c] = newP;
                }
            this.Width = PADDING + Board.Cols * data.TileSize;
            this.Height = PADDING + Board.Rows * data.TileSize;
        }

        public void SetAreaColors(TekBoard board)
        {
            if (board.Areas.Count == 0)
                return;
            int index0 = 0;
            AreaColorIndex = new int[board.Areas.Count];
            for (int i = 0; i < AreaColorIndex.Length; i++)
                AreaColorIndex[i] = -1;
                       
            foreach (TekArea area in board.Areas)
            {
                List<TekArea> neighbours = area.GetAdjacentAreas();
                List<int> inUseByNeighbours = new List<int>();
                foreach (TekArea area2 in neighbours)
                    if (AreaColorIndex[area2.AreaNum] != -1)
                        inUseByNeighbours.Add(AreaColorIndex[area2.AreaNum]);
                int index = (index0 + 1) % MAXCOLOR;
                while (index != index0)
                {
                    if (inUseByNeighbours.Contains(index))
                        index = (index + 1) % MAXCOLOR;
                    else
                        break;
                }
                AreaColorIndex[area.AreaNum] = index;
            }
        }

        private void __SetBorder(TekFieldView p, TekField neighbour, TekFieldView.TekBorder border)
        {
            if (neighbour.Area == null)
            {
                if (p.Field.Area == null)
                    p.Borders[(int)border] = TekFieldView.TekBorderStyle.tbsNone;
                else
                    p.Borders[(int)border] = TekFieldView.TekBorderStyle.tbsNone; // tbsExternal;
            }
            else if (p.Field.Area == null)
                p.Borders[(int)border] = TekFieldView.TekBorderStyle.tbsNone; // tbsExternal;
            else
            {
                if (neighbour.Area.AreaNum == p.Field.Area.AreaNum)
                    p.Borders[(int)border] = TekFieldView.TekBorderStyle.tbsInternal;
                else
                    p.Borders[(int)border] = TekFieldView.TekBorderStyle.tbsExternal;
            }
        }

        public void _SetBorders(TekFieldView p)
        {
            int row = p.Row;
            int col = p.Col;
            if (row == 0)
                p.Borders[(int)TekFieldView.TekBorder.bdTop] = TekFieldView.TekBorderStyle.tbsBoard;
            else
            {
                __SetBorder(p, Board.Fields[row - 1, col], TekFieldView.TekBorder.bdTop);
            }
            if (col == 0)
                p.Borders[(int)TekFieldView.TekBorder.bdLeft] = TekFieldView.TekBorderStyle.tbsBoard;
            else
            {
                __SetBorder(p, Board.Fields[row, col - 1], TekFieldView.TekBorder.bdLeft);
            }
            if (row == Board.Rows - 1)
                p.Borders[(int)TekFieldView.TekBorder.bdBottom] = TekFieldView.TekBorderStyle.tbsBoard;
            else
            {
                __SetBorder(p, Board.Fields[row + 1, col], TekFieldView.TekBorder.bdBottom);
            }
            if (col == Board.Cols - 1)
                p.Borders[(int)TekFieldView.TekBorder.bdRight] = TekFieldView.TekBorderStyle.tbsBoard;
            else
            {
                __SetBorder(p, Board.Fields[row, col + 1], TekFieldView.TekBorder.bdRight);
            }
            p.Invalidate();
        }

        private void SetBorders()
        {
            if (Board == null || _Panels == null)
                return;
            for (int r = 0; r < Board.Rows; r++)
                for (int c = 0; c < Board.Cols; c++)
                {
                    _SetBorders(_Panels[r, c]);
                }            
        }

        public void HighlightField(int row, int col, bool onoff)
        {
            TekFieldView field = GetField(row, col);
            if (onoff)
                for (int b = 0; b < (int) TekFieldView.TekBorder.bdLast; b++)
                    field.Borders[b] = TekFieldView.TekBorderStyle.tbsHighlight;
            else
                _SetBorders(field);
        }

        public TekFieldView GetField(int row, int col)
        {

            if (Board != null && row >= 0 && row < Board.Rows && col >= 0 && col < Board.Cols)
                return _Panels[row, col];
            else
                return null;
        }

        public TekFieldView SelectField(int row, int col)
        {
            TekFieldView result = Selector.CurrentFieldView;
            if (row >= 0 && row < Board.Rows && col >= 0 && col < Board.Cols)
               Selector.SelectCurrentField(_Panels[row, col]);
            return result;
        }

         public void SetShowErrors(bool onoff = true)
        {
            if (Board == null)
                return;
            foreach (TekFieldView P in _Panels)
                P.FieldError = onoff && P.Field != null && !P.Field.IsValid();
            Refresh();
        }

        public void ShowDefaultNotes()
        {
            if (Board == null)
                return;
            Board.SetDefaultNotes();
            Refresh();
        }
    } // TekBoardView

    public class TekFieldView : Panel
    {
        public enum TekBorder { bdTop, bdRight, bdBottom, bdLeft, bdLast };
        public enum TekBorderStyle { tbsNone, tbsInternal, tbsExternal, tbsBoard, tbsSelected, tbsHighlight };


        private System.Drawing.Color _NormalColor;
        public System.Drawing.Color NormalColor
        {
            get { return _NormalColor; }
            set { _NormalColor = value; if (!IsSelected) BackColor = value; }
        }

        private System.Drawing.Color _SelectedColor;
        public System.Drawing.Color SelectedColor
        {
            get { return _SelectedColor; }
            set { _SelectedColor = value; if (IsSelected) BackColor = value; }
        }

        private System.Drawing.Color _MultiSelectedColor;
        public System.Drawing.Color MultiSelectedColor
        {
            get { return _MultiSelectedColor; }
            set { _MultiSelectedColor = value; if (IsMultiSelected) BackColor = value; }
        }
        private TekField field;

        public bool FieldError { get; set; } = false;


        private bool _isSelected;
        public bool IsSelected { get { return _isSelected; } set { SetSelected(value); Refresh(); } }

        public void SetSelected(bool onoff = true)
        {
            if (Field != null && !IgnoreInitial && Field.Initial)
                return;
            _isSelected = onoff;
            if (IsSelected)
            {
                BackColor = SelectedColor;
            }
            else
            {
                BackColor = NormalColor;
            }
            Refresh();
        }

        private bool _isMultiSelected;
        public bool IsMultiSelected { get { return _isMultiSelected; } set { SetMultiSelected(value); } }

        static public bool IgnoreInitial = false;
        public void SetMultiSelected(bool onoff = true)
        {
            if (Field != null && !IgnoreInitial && Field.Initial)
                return;
            _isMultiSelected = onoff;
            if (IsMultiSelected)
                BackColor = MultiSelectedColor;
            else
                BackColor = NormalColor;
            Refresh();
        }

        private TekPanelData _data;

        public void ToggleNote(int value)
        {
            if (field != null)
            {
                field.ToggleNote(value);
                Refresh();
            }
        }

        public int Row { get { return field == null ? -1 : field.Row; } }
        public int Col { get { return field == null ? -1 : field.Col; } }
        public int Value { get { return field == null ? 0 : field.Value; } set { if (field != null && !field.Initial) { field.Value = value; Refresh(); } } }

        public TekBorderStyle[] Borders { get; set; }

        public TekPanelData Data { get { return _data; } set { _data = value; this.Refresh(); } }

        public TekFieldView() : base()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            Borders = new TekBorderStyle[(int)TekBorder.bdLast];
            for (int b = 0; b <= (int)TekBorder.bdTop; b++)
                Borders[b] = TekBorderStyle.tbsNone;
            NormalColor = Color.AliceBlue; 
            SelectedColor = Color.YellowGreen;
            MultiSelectedColor = Color.DarkBlue;
        }



        private void SetField(TekField value)
        {
            field = value;

            this.Refresh();
        }

        public TekField Field
        {
            get { return field; }
            set { SetField(value); }
        }

        private void DrawBorders(PaintEventArgs e)
        {
            DrawBorderType(e, TekBorderStyle.tbsInternal);
            DrawBorderType(e, TekBorderStyle.tbsExternal);
            DrawBorderType(e, TekBorderStyle.tbsBoard);
            DrawBorderType(e, TekBorderStyle.tbsHighlight);
        }

        private void DrawBorderType(PaintEventArgs e, TekBorderStyle BS)
        {
            for (TekBorder border = TekBorder.bdTop; border < TekBorder.bdLast; border++)
            {
                if (IsSelected)
                    DrawSingleBorder(e, border, TekBorderStyle.tbsSelected);
                else if (Borders[(int)border] == BS)
                    DrawSingleBorder(e, border, BS);
            }
        }

        private void DrawSingleBorder(PaintEventArgs e, TekBorder border, TekBorderStyle BS)
        {
            //tbsNone, tbsInternal, tbsExternal, tbsBoard, tbsSelected, tbsHighlight
            int[] penSizes = { 0, 1, 1, 1, 1, 2 };
            int iBS = (int)BS;
            int iBorder = (int)border;
            System.Drawing.Color[] bColors = { Color.White, Color.DarkGray, Color.Black, Color.Black, Color.AntiqueWhite, Color.Lime };

            int pensize = penSizes[iBS];


            int[] X1 = { 0, e.ClipRectangle.Width - 1, e.ClipRectangle.Width - 1, 0 };
            int[] X2 = { e.ClipRectangle.Width - 1, e.ClipRectangle.Width - 1, 0, 0 };
            int[] Y1 = { 0, 0, e.ClipRectangle.Height - 1, e.ClipRectangle.Height - 1 };
            int[] Y2 = { 0, e.ClipRectangle.Height - 1, e.ClipRectangle.Height - 1, 0 };
            if (pensize > 1)
            {
                int pensize2 = pensize / 2;
                switch (border)
                {
                    case TekBorder.bdTop:
                        Y1[iBorder] += pensize2; Y2[iBorder] += pensize2;
                        break;
                    case TekBorder.bdLeft:
                        X1[iBorder] += pensize2; X2[iBorder] += pensize2;
                        break;
                }
            }
            Pen pen = new Pen(new SolidBrush(bColors[iBS]), pensize);
            if (BS == TekBorderStyle.tbsHighlight)
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            e.Graphics.DrawLine(pen, X1[iBorder], Y1[iBorder], X2[iBorder], Y2[iBorder]);
        }
        private void DisplayNote(PaintEventArgs e, int value)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            Data.SetCenterAlignment();
            e.Graphics.DrawString(value.ToString(),
                Data.ValueFont[TekPanelData.FONT_NOTE],
                    Data.TextBrush[(IsSelected || IsMultiSelected) ? TekPanelData.PANEL_SELECTED : TekPanelData.PANEL_NORMAL],
                    Data.NotePoint[value - 1], Data.Format);
        }

        private void DisplayNotes(PaintEventArgs e)
        {
            if (Field != null && Field.Value == 0 && Field.Notes.Count > 0)
            {
                foreach (int value in Field.Notes)
                    DisplayNote(e, value);
            }
        }
        private void DisplayValue(PaintEventArgs e)
        {
            if (Field != null && Field.Value > 0)
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                Data.SetCenterAlignment();
                SolidBrush textBrush;
                if (FieldError)
                    textBrush = Data.TextBrush[TekPanelData.PANEL_ERROR];
                else
                    textBrush = Data.TextBrush[(IsSelected || IsMultiSelected) ? TekPanelData.PANEL_SELECTED : TekPanelData.PANEL_NORMAL];
                e.Graphics.DrawString(Field.Value.ToString(),
                    Data.ValueFont[field.Initial ? TekPanelData.FONT_INITIAL : TekPanelData.FONT_NORMAL],
                        textBrush, Data.ValuePoint, Data.Format);
            }
        } 
     
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DisplayValue(e);
            DisplayNotes(e);
            DrawBorders(e);
        }
    }

    public class TekPanelData
    {
        // data to assist in displaying the fields
        const int MAXTILESIZE = 60;
        const int MINTILESIZE = 25;
        FontFamily fontFamily = new FontFamily("Calibri");
        int FontSize;
        int FontSize2;
        public Font[] ValueFont;
        public SolidBrush[] TextBrush;
        public Point ValuePoint;
        public Point[] NotePoint;
        private StringFormat format = new StringFormat();
        public StringFormat Format { get { return format; } }

        private int _tileSize;
        public int TileSize { get { return _tileSize; } set { SetTileSize(value); } }

        public const int FONT_NORMAL = 0;
        public const int FONT_INITIAL = 1;
        public const int FONT_NOTE = 2;

        public const int PANEL_NORMAL   = 0;
        public const int PANEL_SELECTED = 1;
        public const int PANEL_ERROR    = 2;

        public void SetTileSize(int value)
        {
            if (value == _tileSize)
                return;
            if (value > MAXTILESIZE)
                _tileSize = MAXTILESIZE;
            else
                if (value < MINTILESIZE)
                _tileSize = MINTILESIZE;
            else
                _tileSize = value;
            FontSize = Convert.ToInt32(TileSize * 0.7);
            FontSize2 = Convert.ToInt32(FontSize / 2.5);
            ValueFont = new Font[3];
            ValueFont[FONT_NORMAL] =
                new Font(fontFamily, FontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            ValueFont[FONT_INITIAL] =
                new Font(fontFamily, FontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            ValueFont[FONT_NOTE] =
                new Font(fontFamily, FontSize2, FontStyle.Regular, GraphicsUnit.Pixel);
            TextBrush = new SolidBrush[3];
            TextBrush[PANEL_NORMAL] = new SolidBrush(Color.Black);
            TextBrush[PANEL_SELECTED] = new SolidBrush(Color.White);
            TextBrush[PANEL_ERROR] = new SolidBrush(Color.Red);

            ValuePoint = new Point(TileSize / 2, TileSize / 2);

            NotePoint = new Point[Const.MAXTEK];
            int d = TileSize / 5;
            NotePoint[0] = new Point(d, d);
            NotePoint[1] = new Point(TileSize - d, d);
            NotePoint[2] = new Point(TileSize / 2, TileSize / 2);
            NotePoint[3] = new Point(d, TileSize - d);
            NotePoint[4] = new Point(TileSize - d, TileSize - d);
        }

        public void SetCenterAlignment()
        {
            Format.LineAlignment = StringAlignment.Center;
            Format.Alignment = StringAlignment.Center;
        }
    }

}
