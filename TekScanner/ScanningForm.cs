using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.OCR;
using Emgu.CV.Util;
using Tek1;

namespace TekScanner
{
    public partial class ScanningForm : Form
    {

        Image<Bgr, Byte> imgOriginal;
        Image<Bgr, Byte> lineImage;
        string OriginalFileName;

        TekGridAnalyzer GridAnalyzer;

        public ScanningForm()
        {
            InitializeComponent();
        }

        private void LoadFile(string filename)
        {
            imgOriginal = new Image<Bgr, Byte>(filename);
            pbOriginal.Image = imgOriginal.ToBitmap();
            OriginalFileName = filename;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private string xyS(float x, float y)
        {
            return String.Format("({0};{1})", Math.Round(x, 1), Math.Round(y, 1));
        }

        private void DrawGrid(Image<Bgr, Byte> image, OCVGridDefinition gridDef, Bgr color)
        {
            for (int r = 0; r <= gridDef.Rows; r++)
            {
                image.Draw(new LineSegment2D(new Point(gridDef.TopLeft.X, gridDef.RowLocation(r)),
                                             new Point(gridDef.TopLeft.X + gridDef.Width, gridDef.RowLocation(r))),
                                                  color, 2);
            }
            for (int c = 0; c <= gridDef.Cols; c++)
            {
                image.Draw(new LineSegment2D(new Point(gridDef.ColLocation(c), gridDef.TopLeft.Y),
                                                  new Point(gridDef.ColLocation(c), gridDef.TopLeft.Y + gridDef.Height)),
                                                  color, 2);
            }

            image.Draw(new Rectangle(gridDef.TopLeft.X, gridDef.TopLeft.Y, gridDef.Width, gridDef.Height), color, 2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GridAnalyzer = new TekGridAnalyzer(imgOriginal);
            pbGray.Image = GridAnalyzer.matGrayScaleImage.Bitmap;
            pbEdges.Image = GridAnalyzer.cannyEdges.Bitmap;

            lineImage = imgOriginal.CopyBlank();
            foreach (LineSegment2D line in GridAnalyzer.Lines)
                lineImage.Draw(line, new Bgr(Color.LightGreen), 2);
            pbLines.Image = lineImage.Bitmap;

            Image<Bgr, Byte> image3 = lineImage.CopyBlank();

            foreach (OCVCombinedLinesData line in GridAnalyzer.Grid.HorizontalLines)
            {
                OCVLineData summ = line.GetSummaryLine();
                image3.Draw(summ.Line, new Bgr(Color.Azure), 2);
            }
            foreach (OCVCombinedLinesData line in GridAnalyzer.Grid.VerticalLines)
            {
                OCVLineData summ = line.GetSummaryLine();
                if (summ.Length > 35)
                    image3.Draw(summ.Line, new Bgr(Color.LightPink), 2);
            }
            pbAnalyzed.Image = image3.Bitmap;


            using (GridSizeForm gridForm = new GridSizeForm())
            {
                gridForm.GridAnalyzer = GridAnalyzer;
                gridForm.LoadImage(imgOriginal);
                gridForm.SetGridDef(GridAnalyzer.gridDef);
                if (gridForm.ShowDialog() == DialogResult.OK)
                {
                    GridAnalyzer.gridDef = gridForm.GetGridDef();                                   
                }                    
            }

            Image<Bgr, Byte> lineImage2 = imgOriginal.CopyBlank();
            DrawGrid(lineImage2, GridAnalyzer.gridDef, new Bgr(Color.White));
            pbGrid.Image = lineImage2.Bitmap;

            Image<Bgr, Byte> lineImage4 = imgOriginal.Copy();
            DrawGrid(lineImage4, GridAnalyzer.gridDef, new Bgr(Color.Purple));
            pbCombi.Image = lineImage4.Bitmap;

            tabControl1.SelectedTab = tbCombi;
        }


        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                LoadFile(ofd1.FileName);
                button2.Enabled = true;
                //tabControl1.SelectedTab = tbControls;
            }
            else
                button2.Enabled = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void tbOriginal_Click(object sender, EventArgs e)
        {

        }

        private void bBoard_Click(object sender, EventArgs e)
        {
            CreateBoard();
        }
        public void CreateBoard()
        {
            TekBoardAnalyzer BoardAnalyzer = new TekBoardAnalyzer(GridAnalyzer);
            TekBoardParser parser = new TekBoardParser();

            HeurSolvForm solvForm = new HeurSolvForm();
            string tekfilename = Path.ChangeExtension(OriginalFileName, ".TX");
            parser.Export(BoardAnalyzer.Board, tekfilename);
            solvForm.DoLoad(tekfilename);
            tabControl1.SelectedTab = tbOriginal;
            solvForm.ShowDialog();
        
         }
    }

    
}
