using FFmpeg.AutoGen;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.UserInterface;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace TelloSharp.WinformsExample
{
    internal class CameraModule
    {
        private static readonly CascadeClassifier faceClassifier = new("haarcascade_frontalface_default.xml");
        public event EventHandler<CoordinatesEventArgs> NewCoordInfo;
        BitmapsFromStream bitmapsFromStream = new();

        public CameraModule()
        {
        }

        private void ObjectDetection_NewCoordInfo(object? sender, CoordinatesEventArgs e)
        {
            NewCoordInfo?.Invoke(this, new CoordinatesEventArgs(e.Location, e.Area, e.Label));
        }

        public void Init()
        {
            if (bitmapsFromStream.IsRunning)
            {
                bitmapsFromStream.Stop();
                bitmapsFromStream = new BitmapsFromStream();
            }

            Task.Run(() => bitmapsFromStream.ReadVideo(AVHWDeviceType.AV_HWDEVICE_TYPE_NONE));
            Thread.Sleep(3000);
        }

        public Mat Capture()
        {
            Mat img = new();
            Bitmap frame = bitmapsFromStream.GetNextFrame();
            if (frame != null)
            {
                img = BitmapConverter.ToMat(frame);
            }
            return img;
        }

        public void FindPerson(PictureBoxIpl pictureBox)
        {
            while (true)
            {
                Mat capture = Capture();
                if (capture.Data == IntPtr.Zero) continue;

                Cv2.Resize(capture, capture, new Size(480, 360));

                Mat grayImage = new();
                Cv2.CvtColor(capture, grayImage, ColorConversionCodes.BGR2GRAY);
                Rect[] rectangles = faceClassifier.DetectMultiScale(grayImage, 1.2, 8, HaarDetectionTypes.ScaleImage, null, null);

                if (rectangles.Length == 0)
                {
                    NewCoordInfo?.Invoke(this, new CoordinatesEventArgs(new System.Drawing.Point(0, 0), 0, string.Empty));
                }

                foreach (Rect rect in rectangles)
                {
                    var cx = rect.X + rect.Width / 2;
                    var cy = rect.Y + rect.Height / 2;
                    var area = rect.Width * rect.Height;

                    Cv2.Circle(capture, cx, cy, 10, Scalar.LimeGreen, 5, LineTypes.Link8, 0);
                    Cv2.Rectangle(capture, new Point(rect.BottomRight.X, rect.BottomRight.Y), new Point(rect.TopLeft.X, rect.TopLeft.Y), 55, 2);

                    NewCoordInfo?.Invoke(this, new CoordinatesEventArgs(new System.Drawing.Point(cx, cy), area, "person"));

                }
                var bmp = BitmapConverter.ToBitmap(capture);

                lock (pictureBox)
                {
                    pictureBox.Image = bmp;
                }
            }
        }
    }
}