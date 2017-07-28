﻿using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Threading;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net.WebSockets;

namespace AmbiLight
{

    static class Program
    {
        private static ClientWebSocket ws;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            // this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            ws = new ClientWebSocket();
            Uri uri = new Uri("ws://localhost:3000/");
            ws.ConnectAsync(uri, CancellationToken.None);

            Application.Run(new Form1());
        }

        private static Image Resize(Image input, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destBitmap = new Bitmap(width, height);
            destBitmap.SetResolution(input.HorizontalResolution, input.VerticalResolution);

            Graphics destGraphics = Graphics.FromImage(destBitmap);

            destGraphics.CompositingMode = CompositingMode.SourceCopy;
            destGraphics.CompositingQuality = CompositingQuality.HighSpeed;
            destGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            destGraphics.SmoothingMode = SmoothingMode.None;
            destGraphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;

            ImageAttributes wrapMode = new ImageAttributes();
            
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            destGraphics.DrawImage(input, destRect, 0, 0, input.Width, input.Height, GraphicsUnit.Pixel, wrapMode);

            input.Dispose();
            destGraphics.Dispose();
            return (Image)destBitmap;
        }

        public static Image Capture()
        {
            try
            {
                Screen screen = Screen.PrimaryScreen;
                Rectangle captureRectangle = screen.Bounds;

                //Creating a new Bitmap object
                Bitmap captureBitmap = new Bitmap(captureRectangle.Size.Width, captureRectangle.Size.Height, PixelFormat.Format32bppArgb);
                
                //Creating a New Graphics Object
                Graphics captureGraphics = Graphics.FromImage(captureBitmap);

                //Copying Image from The Screen
                captureGraphics.CopyFromScreen(0, 0, 0, 0, captureRectangle.Size);

                int width = 800;
                int height = 450;

                Image ambiImage = Resize(captureBitmap, 32, 18);
                captureGraphics.Dispose();
                captureBitmap.Dispose();
                
                return Resize(ambiImage, width, height);
                
                //Saving the Image File (I am here Saving it in My E drive).
                //captureBitmap.Save(@"E:\Capture.jpg", ImageFormat.Jpeg);

                //Displaying the Successfull Result

                // MessageBox.Show("Screen Captured");
            }
            catch (Exception ex)
            {
                return null;
                // MessageBox.Show(ex.Message);
            }
        }
    }
}
