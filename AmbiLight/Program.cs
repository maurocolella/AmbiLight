using System;
using System.Collections.Generic;
//using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace AmbiLight
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            Thread.Sleep(Timeout.Infinite);
        }

        private static void Capture()
        {
            try
            {
                Screen screen = Screen.PrimaryScreen;
                Rectangle captureRectangle = screen.Bounds;

                //Creating a new Bitmap object
                Bitmap captureBitmap = new Bitmap(captureRectangle.Size.Width, captureRectangle.Size.Height, PixelFormat.Format32bppArgb);

                //Creating a Rectangle object which will  
                //capture our Current Screen
                
                //Creating a New Graphics Object
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);

                //Copying Image from The Screen
                captureGraphics.CopyFromScreen(0, 0, 0, 0, captureRectangle.Size);

                int width = 16;
                int height = 9;
                
                Rectangle destRect = new Rectangle(0, 0, width, height);
                Bitmap destBitmap = new Bitmap(width, height);
                Graphics destGraphics = Graphics.FromImage(destBitmap);

                destBitmap.SetResolution(captureBitmap.HorizontalResolution, captureBitmap.VerticalResolution);

                destGraphics.CompositingMode = CompositingMode.SourceCopy;
                destGraphics.CompositingQuality = CompositingQuality.HighQuality;
                destGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                destGraphics.SmoothingMode = SmoothingMode.HighQuality;
                destGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    destGraphics.DrawImage(captureBitmap, destRect, 0, 0, captureBitmap.Width, captureBitmap.Height, GraphicsUnit.Pixel, wrapMode);
                }



                //Saving the Image File (I am here Saving it in My E drive).
                //captureBitmap.Save(@"E:\Capture.jpg", ImageFormat.Jpeg);

                //Displaying the Successfull Result

                // MessageBox.Show("Screen Captured");
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }
    }
}
