using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wow钓鱼
{
    class Class1
    {
        public static Bitmap captureScreen(int x, int y, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(new Point(x, y), new Point(0, 0), bmp.Size);
                g.Dispose();
            }
            //bit.Save(@"capture2.png");
            return bmp;
        }

        /// <summary>
        /// 找颜色
        /// </summary>
        /// <param name="parPic">查找的图片的绝对路径</param>
        /// <param name="searchColor">查找的16进制颜色值，如#0C5FAB</param>
        /// <param name="searchRect">查找的矩形区域范围内</param>
        /// <param name="errorRange">容错</param>
        /// <returns></returns>
        public System.Drawing.Point FindColor(string searchColor, System.Drawing.Rectangle searchRect, byte errorRange = 10)
        {
            var colorX = System.Drawing.ColorTranslator.FromHtml(searchColor);
            Size screenSize = Screen.PrimaryScreen.Bounds.Size;
            var parBitmap = captureScreen(0, 0, screenSize.Width, screenSize.Height);// new Bitmap(parPic);
            var parData = parBitmap.LockBits(new System.Drawing.Rectangle(0, 0, parBitmap.Width, parBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var byteArraryPar = new byte[parData.Stride * parData.Height];
            Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);
            if (searchRect.IsEmpty)
            {
                searchRect = new System.Drawing.Rectangle(0, 0, parBitmap.Width, parBitmap.Height);
            }
            var searchLeftTop = searchRect.Location;
            var searchSize = searchRect.Size;
            var iMax = searchLeftTop.Y + searchSize.Height;//行
            var jMax = searchLeftTop.X + searchSize.Width;//列
            int pointX = -1; int pointY = -1;
            for (int m = searchRect.Y; m < iMax; m++)
            {
                for (int n = searchRect.X; n < jMax; n++)
                {
                    int index = m * parBitmap.Width * 4 + n * 4;
                    var color = System.Drawing.Color.FromArgb(byteArraryPar[index + 3], byteArraryPar[index + 2], byteArraryPar[index + 1], byteArraryPar[index]);
                    if (ColorAEqualColorB(color, colorX, errorRange))
                    {
                        pointX = n;
                        pointY = m;
                        goto END;
                    }
                }
            }
        END:
            parBitmap.UnlockBits(parData);
            return new System.Drawing.Point(pointX, pointY);
        }


        public bool ColorAEqualColorB(System.Drawing.Color colorA, System.Drawing.Color colorB, byte errorRange = 10)
        {
            return colorA.A <= colorB.A + errorRange && colorA.A >= colorB.A - errorRange &&
                colorA.R <= colorB.R + errorRange && colorA.R >= colorB.R - errorRange &&
                colorA.G <= colorB.G + errorRange && colorA.G >= colorB.G - errorRange &&
                colorA.B <= colorB.B + errorRange && colorA.B >= colorB.B - errorRange;

        }
        

    }
}
