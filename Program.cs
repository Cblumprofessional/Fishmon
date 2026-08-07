using Fish.Controller.Actions;
using OpenCvSharp;

namespace Fishmon.Controller;

internal static class Program
{
    private const string WindowName = "Fishmon";

    public static void Main()
    {
        using var camera = new VideoCapture(0);

        if (!camera.IsOpened())
        {
            Console.WriteLine("Could not open webcam");
            return;
        }

        //setting resolution to 1280 by 720 at 30fps, that is what the camera I bought supports
        //The camera used is EMEET C950 1080p webcam bought off Amazon
        camera.Set(VideoCaptureProperties.FrameWidth, 1280);
        camera.Set(VideoCaptureProperties.FrameHeight, 720);
        camera.Set(VideoCaptureProperties.Fps, 30);

        using var frame = new Mat();

        Console.WriteLine("Camera started");
        Console.WriteLine("Press ESC to quit");

        while (true)
        {
            if(!camera.Read(frame) || frame.Empty())
            {
                Console.WriteLine("Failed to read frame."); //we dont really care if a frame is missing, we will continue and hope the next frame is there
                continue;
            }

            DrawControllerGrid(frame);

            Cv2.ImShow(WindowName, frame);

            int key = Cv2.WaitKey(1);

            if(key == 27) // code for ESC
            {
                break;
            }
        }
        Cv2.DestroyAllWindows();
    }

    private static void DrawControllerGrid(Mat frame)
    {
        //dividing by 3 so the buttons can fit evenly on a 3 by 3 grid
        int cellWidth = frame.Width / 3;
        int cellHeight = frame.Height /3;

        //Button labels for a gameboy advanced
        string[,] labels =
        {
            {"UP", "A", "START"},
            {"LEFT", "IDLE", "RIGHT"},
            {"B", "DOWN", "SELECT"}
        };

        //Visual vertical lines separating the buttons
        Cv2.Line(
            frame,
            new Point(cellWidth, 0),
            new Point(cellWidth, frame.Height),
            Scalar.White,
            2
        );
        
        Cv2.Line(
            frame,
            new Point(cellWidth * 2, 0),
            new Point(cellWidth*2, frame.Height),
            Scalar.White,
            2
        );

        //Visual horizontal lines separating the buttons
        Cv2.Line(
            frame,
            new Point(0, cellHeight),
            new Point(frame.Width, cellHeight),
            Scalar.White,
            2
        );

        Cv2.Line(
            frame,
            new Point(0, cellHeight*2),
            new Point(frame.Width, cellHeight*2),
            Scalar.White,
            2
        );


        //labels
        for(int row = 0; row < 3; row++)
        {
            for(int col = 0; col < 3; col++)
            {
                FishAction action = FishActionMapper.GetActionFromZone(row,col);

                string label;

                if(action == FishAction.None)
                {
                    label = "IDLE";
                }
                else
                {
                    label = action.ToString().ToUpper();
                }

                int x = col * cellWidth + 15;
                int y = row * cellHeight + 35;

                Cv2.PutText(
                    frame,
                    label,
                    new Point(x, y),
                    HersheyFonts.HersheySimplex,
                    0.8,
                    Scalar.White,
                    2
                );
            }
        }


    
    }
}