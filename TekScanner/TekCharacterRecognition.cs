using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.OCR;



namespace TekScanner
{
    class TekCharacterRecognition
    {
        public string TessDataPath = @"C:\Program Files (x86)\Tesseract-OCR\tessdata";
        private Tesseract _ocr;
        public int[,] Values;

        public TekCharacterRecognition(UMat matGray, OCVGridDefinition gridDef)
        {
            //create OCR engine
            _ocr = new Tesseract(TessDataPath, "eng", OcrEngineMode.TesseractOnly);
            _ocr.SetVariable("tessedit_char_whitelist", "12345");
            Values = new int[gridDef.Rows, gridDef.Cols];
            Image<Gray, Byte> testImage = matGray.ToImage<Gray, Byte>().Copy();
            for (int row = 0; row < gridDef.Rows; row++)
                for (int col = 0; col < gridDef.Cols; col++)
                {
                    Values[row, col] = FindValueInField(testImage, gridDef, row, col);
                }
        }
        
        private Rectangle gridRect(OCVGridDefinition gridDef, int row, int col)
        {
            return new Rectangle((int)gridDef.ColLocation(col), (int)gridDef.RowLocation(row), (int)gridDef.ColSize, (int)gridDef.RowSize);
        }
        private int FindValueInField(Image<Gray, Byte> image, OCVGridDefinition gridDef, int row, int col)
        {
            const int MARGIN = 7;
            int result;
            Rectangle Rect = gridRect(gridDef, row, col);
            Rect.X += MARGIN;
            Rect.Y += MARGIN;
            Rect.Width -= 2 * MARGIN;
            Rect.Height -= 2 * MARGIN;
            Image<Gray, Byte> fieldImage = image.Copy(Rect);
            Image<Gray, Byte> fieldImage2 = fieldImage.CopyBlank();
            // note: threshold should be investigated here, these may be suboptimal
            CvInvoke.Threshold(fieldImage, fieldImage2, 100, 255, ThresholdType.Binary);
            _ocr.SetImage(fieldImage2);
            if (_ocr.Recognize() != 0)
                throw new Exception("Failed to recognize the image");
            foreach (Tesseract.Character ch in _ocr.GetCharacters())
                if (Int32.TryParse(ch.Text, out result))
                    return result;
            return 0;
        }
    }
}
