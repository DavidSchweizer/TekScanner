using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace TekScanner
{
    public class TekGridAnalyzer
    {
        public UMat matGrayScaleImage;
        public UMat cannyEdges;
        public LineSegment2D[] Lines;
        public OCVGrid Grid;
        public OCVGridDefinition gridDef;

        public double cannyThreshold = 180.0;
        public double cannyThresholdLinking = 120.0;
        public int houghThreshold = 3;
        public double houghMinLineWidth = 3;
        public double houghGap = 2;

        public TekGridAnalyzer(Image<Bgr, Byte> imgOriginal)
        {
            matGrayScaleImage = GrayScaleImage(imgOriginal);
            cannyEdges = FindEdges();
            Lines = CvInvoke.HoughLinesP(
                   cannyEdges,
                   1, //Distance resolution in pixel-related units
                   Math.PI / 45.0, //Angle resolution measured in radians.
                   houghThreshold, //threshold
                   houghMinLineWidth, //min Line width
                   houghGap); //gap between lines
            OCVGridData gridData = new OCVGridData(Lines);
            Grid = new OCVGrid(gridData);
            gridDef = Grid.Analyze();
        }
 
        private UMat FindEdges()
        {
            // Canny and edge detection
            UMat cannyEdges = new UMat();
            CvInvoke.Canny(matGrayScaleImage, cannyEdges, cannyThreshold, cannyThresholdLinking);
            return cannyEdges;
        }
        private UMat GrayScaleImage(Image<Bgr, Byte> imgOriginal)
        {
            //Convert the image to grayscale and filter out the noise
            UMat result = new UMat();
            CvInvoke.CvtColor(imgOriginal, result, ColorConversion.Bgr2Gray);
            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(result, pyrDown);
            CvInvoke.PyrUp(pyrDown, result);
            return result;
        }
    }

}
