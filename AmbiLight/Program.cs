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
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

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

        private static unsafe int[,] CreateArray(Bitmap bmp)
        {
            // Note that is it somewhat a standard
            // to define 2d array access by [y,x] (not [x,y])
            bool[,] bwValues = new bool[bmp.Height, bmp.Width];

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData =
                bmp.LockBits(rect, ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            byte* ptr = (byte*)bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap. 
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            int[,] result = new int[bmp.Width * 2 + (bmp.Height - 2) * 2,3];
            int pos = 0;

            // Right column, bottom to top
            for (int y = bmp.Height - 2; y > 0; y--)
            {
                var row = ptr + (y * bmpData.Stride);
                var pixel = row + ((bmp.Width - 1) * 4);

                result[pos, 0] = pixel[2];
                result[pos, 1] = pixel[1];
                result[pos, 2] = pixel[0];
                pos++;
            }

            // Top row, right to left
            for (int x = bmp.Width - 1; x >= 0; x--)
            {
                var row = ptr;
                var pixel = row + x * 4;

                result[pos, 0] = pixel[2];
                result[pos, 1] = pixel[1];
                result[pos, 2] = pixel[0];
                pos++;
            }

            
            // Left column, top to bottom
            for (int y = 1; y < bmp.Height - 1; y++)
            {
                var row = ptr + (y * bmpData.Stride);
                var pixel = row;

                result[pos, 0] = pixel[2];
                result[pos, 1] = pixel[1];
                result[pos, 2] = pixel[0];
                pos++;
            }

            // Bottom row, left to right
            for (int x = 0; x < bmp.Width; x++)
            {
                var row = ptr + ((bmp.Height - 1) * bmpData.Stride);
                var pixel = row + x * 4;

                result[pos, 0] = pixel[2];
                result[pos, 1] = pixel[1];
                result[pos, 2] = pixel[0];
                pos++;
            }

            return result;
            // Do whatever you want with vwValues here
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

                int[,] colorArray = CreateArray(new Bitmap(ambiImage));
                JavaScriptSerializer json = new JavaScriptSerializer();
                byte[] encoded = Encoding.UTF8.GetBytes(json.Serialize(colorArray));
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);

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
