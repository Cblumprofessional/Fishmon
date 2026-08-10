using OpenCvSharp;
using System.Linq;

namespace Fishmon.Controller.Detection;

public static class FishDetector{

    public static Point? DetectRedObject(Mat Frame)
    {
        using var hsv = new Mat();
        using var mask1 = new Mat();
        using var mask2 = new Mat();
        using var mask = new Mat();

        Cv2.CvtColor(Frame, hsv, ColorConversionCodes.BGR2HSV);
    // Red wraps around the HSV hue scale,
        // so we need two ranges.
        Scalar lowerRed1 = new Scalar(0, 70, 90);
        Scalar upperRed1 = new Scalar(10, 255, 255);

        Scalar lowerRed2 = new Scalar(170, 40, 20);
        Scalar upperRed2 = new Scalar(180, 255, 255);

        Cv2.InRange(hsv, lowerRed1, upperRed1, mask1);
        Cv2.InRange(hsv, lowerRed2, upperRed2, mask2);

        Cv2.BitwiseOr(mask1, mask2, mask);

        //Cv2.ImShow("Red Mask", mask);

        
        using var kernel = Cv2.GetStructuringElement(
            MorphShapes.Ellipse,
            new Size(5, 5)
        );

        Cv2.MorphologyEx(
            mask,
            mask,
            MorphTypes.Open,
            kernel
        );


        Cv2.FindContours(
            mask,
            out Point[][] contours,
            out _,
            RetrievalModes.External,
            ContourApproximationModes.ApproxSimple
        );

        if( contours.Length == 0)
        {
            return null;
        }

        Point[] largestContour = contours.OrderByDescending(contour => Cv2.ContourArea(contour)).First();
        double area = Cv2.ContourArea(largestContour);

        //ignoring tiny blue noise
        if(area < 500)
        {
            return null;
        }

        Moments moments = Cv2.Moments(largestContour);

        if(moments.M00 == 0)
        {
            return null;
        }

        int centerX = (int)(moments.M10 / moments.M00);
        int centerY = (int)(moments.M01 / moments.M00);

        return new Point(centerX, centerY);
    }

}
