using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
namespace ToMau
{
    class StackLinearFloodFill:IStrategy
    {
        byte[] bits;
        byte[] byteFillColor;
        byte[] byteBoundColor;
        byte[] startColor;
        int bitmapWidth;
        int bitmapHeight;
        int pixelFormatSize;
        int bitmapStride;
        int padding;
        int bitmapPixelFormatSize;

        internal GCHandle handle;
        internal IntPtr bitPtr;
        Bitmap bitmap;

        protected byte[] tolerance = new byte[] { 0, 0, 0 };

        public int Fill(Context context)
        {
            Init(context);
            Process(context.x, context.y);
            bitmap = new Bitmap(bitmapWidth, bitmapHeight, bitmapStride, PixelFormat.Format32bppArgb, bitPtr);
            context.bitmap = this.bitmap;
            return 1;
        }
        void Init(Context context)
        {
            startColor = new byte[] { context.colorStart.B, context.colorStart.G, context.colorStart.R };
            byteFillColor = new byte[] { context.colorFill.B, context.colorFill.G, context.colorFill.R };
            byteBoundColor = new byte[] { context.colorBoundary.B, context.colorBoundary.G, context.colorBoundary.R };
            bitmapWidth = context.img.Width;
            bitmapHeight = context.img.Height;

            pixelFormatSize = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            bitmapStride = bitmapWidth * pixelFormatSize;
            padding = (bitmapStride % 4);
            bitmapStride += padding == 0 ? 0 : 4 - padding;//pad out to multiple of 4
            bitmapPixelFormatSize = pixelFormatSize;

            bits = new byte[bitmapStride * bitmapWidth];
            handle = GCHandle.Alloc(bits, GCHandleType.Pinned);
            bitPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bits, 0);
            bitmap = new Bitmap(bitmapWidth, bitmapHeight, bitmapStride, PixelFormat.Format32bppArgb, bitPtr);
            Graphics g = Graphics.FromImage(bitmap);
            Bitmap bmImg = new Bitmap(context.img);
            g.DrawImageUnscaledAndClipped(bmImg, new Rectangle(0, 0, bitmapWidth, bitmapHeight));
            g.Dispose();
        }

        void Process(int _x, int _y)
        {
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(_x, _y));

            while (stack.Count > 0)
            {
                Point P = stack.Pop();
                int increaseX = 0;
                bool havePointUpQueue = false;
                bool havePointDownQueue = false;
                int x, y;
                x = P.X; y = P.Y;
                int idx = CoordsToByteIndex(ref x, ref y);
                while (P.X < bitmapWidth - increaseX
                    && P.Y > 0 && P.Y < bitmapHeight - 1
                    && CheckPixel(ref idx))
                {

                    bits[idx] = byteFillColor[0];
                    bits[idx + 1] = byteFillColor[1];
                    bits[idx + 2] = byteFillColor[2];


                    if (!havePointUpQueue)
                        if (checkSeed(new Point(P.X + increaseX, P.Y + 1), stack)) havePointUpQueue = true;
                    if (!havePointDownQueue)
                        if (checkSeed(new Point(P.X + increaseX, P.Y - 1), stack)) havePointDownQueue = true;

                    int idxUp2;
                    int idxDown2;
                    int x3;
                    int y3;
                    x3 = P.X + increaseX;
                    y3 = P.Y + 1;
                    idxUp2 = CoordsToByteIndex(ref x3, ref y3);
                    if (!CheckPixel(ref idxUp2))
                        havePointUpQueue = false;

                    int x4;
                    int y4;
                    x4 = P.X + increaseX;
                    y4 = P.Y - 1;
                    idxDown2 = CoordsToByteIndex(ref x4, ref y4);
                    if (!CheckPixel(ref idxDown2))
                        havePointDownQueue = false;

                    increaseX++;

                    x = P.X + increaseX;
                    y = P.Y;
                    idx = CoordsToByteIndex(ref x, ref y);

                }
            }
        }


        bool checkSeed(Point _p, Stack<Point> _stack)
        {
            int x, y;
            x = _p.X; y = _p.Y;
            int idx = CoordsToByteIndex(ref x, ref y);
            if (!CheckPixel(ref idx))
            {
                return false;
            }

            int h = 0;
            int x2, y2;
            x2 = _p.X - h;
            y2 = _p.Y;
            int idx2 = CoordsToByteIndex(ref x2, ref y2);
            while (CheckPixel(ref idx2))
            {
                if (_p.X - h == 0)
                {
                    _stack.Push(new Point(0, _p.Y));
                    return true;
                }
                h++;
                x2 = _p.X - h;
                y2 = _p.Y;
                idx2 = CoordsToByteIndex(ref x2, ref y2);
            }
            _stack.Push(new Point(_p.X - h + 1, _p.Y));
            return true;
        }
        protected bool CheckPixel(ref int px)
        {


            return (
                (
                !((bits[px] >= (byteFillColor[0] - tolerance[0])) && bits[px] <= (byteFillColor[0] + tolerance[0]) &&
                (bits[px + 1] >= (byteFillColor[1] - tolerance[1])) && bits[px + 1] <= (byteFillColor[1] + tolerance[1]) &&
                (bits[px + 2] >= (byteFillColor[2] - tolerance[2])) && bits[px + 2] <= (byteFillColor[2] + tolerance[2]))
                ) &&
                !((bits[px] >= (byteBoundColor[0] - tolerance[0])) && bits[px] <= (byteBoundColor[0] + tolerance[0]) &&
                (bits[px + 1] >= (byteBoundColor[1] - tolerance[1])) && bits[px + 1] <= (byteBoundColor[1] + tolerance[1]) &&
                (bits[px + 2] >= (byteBoundColor[2] - tolerance[2])) && bits[px + 2] <= (byteBoundColor[2] + tolerance[2]))
                );

            /*
            return (
                
                !(bits[px] ==byteFillColor[0] &&
                bits[px + 1] == byteFillColor[1]  &&
                bits[px + 2] == byteFillColor[2] 
                ) &&
                !(bits[px] == byteBoundColor[0]  &&
                bits[px + 1] == byteBoundColor[1] &&
                bits[px + 2] == byteBoundColor[2] )
                );
             */
        }

        ///<summary>Calculates and returns the byte index for the pixel (x,y).</summary>
        ///<param name="x">The x coordinate of the pixel whose byte index should be returned.</param>
        ///<param name="y">The y coordinate of the pixel whose byte index should be returned.</param>
        protected int CoordsToByteIndex(ref int x, ref int y)
        {
            return (bitmapStride * y) + (x * bitmapPixelFormatSize);
        }

        /// <summary>
        /// Returns the linear index for a pixel, given its x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns></returns>
        protected int CoordsToPixelIndex(int x, int y)
        {
            return (bitmapWidth * y) + x;
        }
    }
}
