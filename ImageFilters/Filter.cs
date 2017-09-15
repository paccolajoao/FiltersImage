using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageFilters
{
    public static class Filter
    {

        

        public static Bitmap CarregaImagem ()
        {
            OpenFileDialog file = new OpenFileDialog();
            file.Title = "Selecione uma imagem";
            file.Filter = "Formatos(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";


            if (file.ShowDialog() == DialogResult.OK)
            {
                Bitmap pic = new Bitmap(file.FileName);
                return pic;
            }

            

            return null;
        }

        public static Bitmap PretoBranco(Bitmap Bmp)
        {
            
            Bitmap B = (Bitmap)Bmp.Clone();
            int rgb;
            Color c;

            for (int y = 0; y < B.Height; y++)
                for (int x = 0; x < B.Width; x++)
                {
                    c = B.GetPixel(x, y);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    B.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return B;
        }

        public static Bitmap FuzzyBlur(Bitmap Bmp)
        {
            int filterSize = 9;
            float numEdge1 = 3;
            float numEdge2 = 4;

            return Bmp.BooleanEdgeDetectionFilter(numEdge1).MeanFilter(filterSize).BooleanEdgeDetectionFilter(numEdge2);
        }

        public static Bitmap SepiaTone(Bitmap bmp)
        {
            
            Bitmap filtred = (Bitmap)bmp.Clone();

            int width = filtred.Width;
            int height = filtred.Height;

            Color p;

            for ( int y = 0; y < height; y++)
            {
                for ( int x = 0; x < width; x++)
                {
                    p = filtred.GetPixel(x, y);

                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
                    int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
                    int tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

                    if ( tr > 255)
                    {
                        r = 255; 
                    }
                    else
                    {
                        r = tr;
                    }

                    if ( tg > 255)
                    {
                        g = 255;
                    }
                    else
                    {
                        g = tg;
                    }

                    if ( tb > 255)
                    {
                        b = 255;
                    }
                    else
                    {
                        b = tb;
                    }

                    filtred.SetPixel(x, y, Color.FromArgb(a, r, g, b));
                }
            }

            return filtred;
        }

        public static Bitmap Invertido(Bitmap Bmp)
        {

            Bitmap B = (Bitmap)Bmp.Clone();
            
            Color c;

            for (int i = 0; i < B.Width; i++)
            {
                for (int j = 0; j < B.Height; j++)
                {
                    c = B.GetPixel(i, j);
                    B.SetPixel(i, j,
                    Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }


            return B;
        }

        public static Bitmap Contraste(Bitmap Bmp, double valor)
        {

            Bitmap B = (Bitmap)Bmp.Clone();
          
            valor = (100.0 + valor) / 100.0;
            valor *= valor;
            Color c;

            for (int i = 0; i < B.Width; i++)
            {
                for (int j = 0; j < B.Height; j++)
                {
                    c = B.GetPixel(i, j);
                    double pR = c.R / 255.0;
                    pR -= 0.5;
                    pR *= valor;
                    pR += 0.5;
                    pR *= 255;
                    if (pR < 0) pR = 0;
                    if (pR > 255) pR = 255;

                    double pG = c.G / 255.0;
                    pG -= 0.5;
                    pG *= valor;
                    pG += 0.5;
                    pG *= 255;
                    if (pG < 0) pG = 0;
                    if (pG > 255) pG = 255;

                    double pB = c.B / 255.0;
                    pB -= 0.5;
                    pB *= valor;
                    pB += 0.5;
                    pB *= 255;
                    if (pB < 0) pB = 0;
                    if (pB > 255) pB = 255;

                    B.SetPixel(i, j, Color.FromArgb((byte)pR, (byte)pG, (byte)pB));
                }
            }

            return B;
        }

        public static Bitmap BooleanEdgeDetectionFilter(
               this Bitmap sourceBitmap, float edgeFactor)
        {
            byte[] pixelBuffer = sourceBitmap.GetByteArray();
            byte[] resultBuffer = new byte[pixelBuffer.Length];
            Buffer.BlockCopy(pixelBuffer, 0, resultBuffer,
                             0, pixelBuffer.Length);

            List<string> edgeMasks = GetBooleanEdgeMasks();

            int imageStride = sourceBitmap.Width * 4;
            int matrixMean = 0, pixelTotal = 0;
            int filterY = 0, filterX = 0, calcOffset = 0;
            string matrixPatern = String.Empty;

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                matrixPatern = String.Empty;
                matrixMean = 0; pixelTotal = 0;
                filterY = -1; filterX = -1;

                while (filterY < 2)
                {
                    calcOffset = k + (filterX * 4) +
                    (filterY * imageStride);

                    calcOffset = (calcOffset < 0 ? 0 :
                    (calcOffset >= pixelBuffer.Length - 2 ?
                    pixelBuffer.Length - 3 : calcOffset));

                    matrixMean += pixelBuffer[calcOffset];
                    matrixMean += pixelBuffer[calcOffset + 1];
                    matrixMean += pixelBuffer[calcOffset + 2];

                    filterX += 1;

                    if (filterX > 1)
                    { filterX = -1; filterY += 1; }
                }

                matrixMean = matrixMean / 9;
                filterY = -1; filterX = -1;

                while (filterY < 2)
                {
                    calcOffset = k + (filterX * 4) +
                    (filterY * imageStride);

                    calcOffset = (calcOffset < 0 ? 0 :
                    (calcOffset >= pixelBuffer.Length - 2 ?
                    pixelBuffer.Length - 3 : calcOffset));

                    pixelTotal = pixelBuffer[calcOffset];
                    pixelTotal += pixelBuffer[calcOffset + 1];
                    pixelTotal += pixelBuffer[calcOffset + 2];

                    matrixPatern += (pixelTotal > matrixMean
                                                 ? "1" : "0");
                    filterX += 1;

                    if (filterX > 1)
                    { filterX = -1; filterY += 1; }
                }

                if (edgeMasks.Contains(matrixPatern))
                {
                    resultBuffer[k] =
                    ClipByte(resultBuffer[k] * edgeFactor);

                    resultBuffer[k + 1] =
                    ClipByte(resultBuffer[k + 1] * edgeFactor);

                    resultBuffer[k + 2] =
                    ClipByte(resultBuffer[k + 2] * edgeFactor);
                }
            }

            return resultBuffer.GetImage(sourceBitmap.Width, sourceBitmap.Height);
        }

        public static List<string> GetBooleanEdgeMasks()
        {
            List<string> edgeMasks = new List<string>();

            edgeMasks.Add("011011011");
            edgeMasks.Add("000111111");
            edgeMasks.Add("110110110");
            edgeMasks.Add("111111000");
            edgeMasks.Add("011011001");
            edgeMasks.Add("100110110");
            edgeMasks.Add("111011000");
            edgeMasks.Add("111110000");
            edgeMasks.Add("111011001");
            edgeMasks.Add("100110111");
            edgeMasks.Add("001011111");
            edgeMasks.Add("111110100");
            edgeMasks.Add("000011111");
            edgeMasks.Add("000110111");
            edgeMasks.Add("001011011");
            edgeMasks.Add("110110100");

            return edgeMasks;
        }

        private static Bitmap MeanFilter(this Bitmap sourceBitmap,
                                         int meanSize)
        {
            byte[] pixelBuffer = sourceBitmap.GetByteArray();
            byte[] resultBuffer = new byte[pixelBuffer.Length];

            double blue = 0.0, green = 0.0, red = 0.0;
            double factor = 1.0 / (meanSize * meanSize);

            int imageStride = sourceBitmap.Width * 4;
            int filterOffset = meanSize / 2;
            int calcOffset = 0, filterY = 0, filterX = 0;

            for (int k = 0; k + 4 < pixelBuffer.Length; k += 4)
            {
                blue = 0; green = 0; red = 0;
                filterY = -filterOffset;
                filterX = -filterOffset;

                while (filterY <= filterOffset)
                {
                    calcOffset = k + (filterX * 4) +
                    (filterY * imageStride);

                    calcOffset = (calcOffset < 0 ? 0 :
                    (calcOffset >= pixelBuffer.Length - 2 ?
                    pixelBuffer.Length - 3 : calcOffset));

                    blue += pixelBuffer[calcOffset];
                    green += pixelBuffer[calcOffset + 1];
                    red += pixelBuffer[calcOffset + 2];

                    filterX++;

                    if (filterX > filterOffset)
                    {
                        filterX = -filterOffset;
                        filterY++;
                    }
                }

                resultBuffer[k] = ClipByte(factor * blue);
                resultBuffer[k + 1] = ClipByte(factor * green);
                resultBuffer[k + 2] = ClipByte(factor * red);
                resultBuffer[k + 3] = 255;
            }

            return resultBuffer.GetImage(sourceBitmap.Width, sourceBitmap.Height);
        }

        public static byte[] GetByteArray(this Bitmap sourceBitmap)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] sourceBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0,
                                       sourceBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            return sourceBuffer;
        }

        public static Bitmap GetImage(this byte[] resultBuffer, int width, int height)
        {
            Bitmap resultBitmap = new Bitmap(width, height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        private static byte ClipByte(double colour)
        {
            return (byte)(colour > 255 ? 255 :
                   (colour < 0 ? 0 : colour));
        }


        public static Bitmap Cartoon(Bitmap Bmp)
        {

            int level=3;
            int filterSize=50;
            byte threshold=65;

            Bitmap paintFilterImage = Bmp.OilPaintFilter(level, filterSize);

            Bitmap edgeDetectImage = Bmp.GradientBasedEdgeDetectionFilter(threshold);

            BitmapData paintData =
                       paintFilterImage.LockBits(new Rectangle(0, 0,
                       paintFilterImage.Width, paintFilterImage.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] paintPixelBuffer = new byte[paintData.Stride *
                                              paintData.Height];

            Marshal.Copy(paintData.Scan0, paintPixelBuffer, 0,
                                       paintPixelBuffer.Length);

            paintFilterImage.UnlockBits(paintData);

            BitmapData edgeData =
                       edgeDetectImage.LockBits(new Rectangle(0, 0,
                       edgeDetectImage.Width, edgeDetectImage.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] edgePixelBuffer = new byte[edgeData.Stride *
                                             edgeData.Height];

            Marshal.Copy(edgeData.Scan0, edgePixelBuffer, 0,
                                      edgePixelBuffer.Length);

            edgeDetectImage.UnlockBits(edgeData);

            byte[] resultBuffer = new byte[edgeData.Stride *
                                             edgeData.Height];

            for (int k = 0; k + 4 < paintPixelBuffer.Length; k += 4)
            {
                if (edgePixelBuffer[k] == 255 ||
                    edgePixelBuffer[k + 1] == 255 ||
                    edgePixelBuffer[k + 2] == 255)
                {
                    resultBuffer[k] = 0;
                    resultBuffer[k + 1] = 0;
                    resultBuffer[k + 2] = 0;
                    resultBuffer[k + 3] = 255;
                }
                else
                {
                    resultBuffer[k] = paintPixelBuffer[k];
                    resultBuffer[k + 1] = paintPixelBuffer[k + 1];
                    resultBuffer[k + 2] = paintPixelBuffer[k + 2];
                    resultBuffer[k + 3] = 255;
                }
            }

            Bitmap B = new Bitmap(Bmp.Width, Bmp.Height);

            BitmapData resultData =
                       B.LockBits(new Rectangle(0, 0,
                       B.Width, B.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);

            B.UnlockBits(resultData);

            return B;
        }

        public static Bitmap OilPaintFilter(this Bitmap sourceBitmap,
                                               int levels,
                                               int filterSize)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride *
                                          sourceData.Height];

            byte[] resultBuffer = new byte[sourceData.Stride *
                                           sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0,
                                       pixelBuffer.Length);

            sourceBitmap.UnlockBits(sourceData);

            int[] intensityBin = new int[levels];
            int[] blueBin = new int[levels];
            int[] greenBin = new int[levels];
            int[] redBin = new int[levels];

            levels = levels - 1;

            int filterOffset = (filterSize - 1) / 2;
            int byteOffset = 0;
            int calcOffset = 0;
            int currentIntensity = 0;
            int maxIntensity = 0;
            int maxIndex = 0;

            double blue = 0;
            double green = 0;
            double red = 0;

            for (int offsetY = filterOffset; offsetY <
                sourceBitmap.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    sourceBitmap.Width - filterOffset; offsetX++)
                {
                    blue = green = red = 0;

                    currentIntensity = maxIntensity = maxIndex = 0;

                    intensityBin = new int[levels + 1];
                    blueBin = new int[levels + 1];
                    greenBin = new int[levels + 1];
                    redBin = new int[levels + 1];

                    byteOffset = offsetY *
                    sourceData.Stride + offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            currentIntensity = (int)Math.Round(((double)
                                       (pixelBuffer[calcOffset] +
                                       pixelBuffer[calcOffset + 1] +
                                       pixelBuffer[calcOffset + 2]) / 3.0 *
                                       (levels)) / 255.0);

                            intensityBin[currentIntensity] += 1;
                            blueBin[currentIntensity] += pixelBuffer[calcOffset];
                            greenBin[currentIntensity] += pixelBuffer[calcOffset + 1];
                            redBin[currentIntensity] += pixelBuffer[calcOffset + 2];

                            if (intensityBin[currentIntensity] > maxIntensity)
                            {
                                maxIntensity = intensityBin[currentIntensity];
                                maxIndex = currentIntensity;
                            }
                        }
                    }

                    blue = blueBin[maxIndex] / maxIntensity;
                    green = greenBin[maxIndex] / maxIntensity;
                    red = redBin[maxIndex] / maxIntensity;

                    resultBuffer[byteOffset] = ClipByte(blue);
                    resultBuffer[byteOffset + 1] = ClipByte(green);
                    resultBuffer[byteOffset + 2] = ClipByte(red);
                    resultBuffer[byteOffset + 3] = 255;

                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width,
                                             sourceBitmap.Height);

            BitmapData resultData =
                       resultBitmap.LockBits(new Rectangle(0, 0,
                       resultBitmap.Width, resultBitmap.Height),
                       ImageLockMode.WriteOnly,
                       PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0,
                                       resultBuffer.Length);

            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public static Bitmap GradientBasedEdgeDetectionFilter(
                                this Bitmap sourceBitmap,
                                byte threshold = 0)
        {
            BitmapData sourceData =
                       sourceBitmap.LockBits(new Rectangle(0, 0,
                       sourceBitmap.Width, sourceBitmap.Height),
                       ImageLockMode.ReadOnly,
                       PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            sourceBitmap.UnlockBits(sourceData);

            int sourceOffset = 0, gradientValue = 0;
            bool exceedsThreshold = false;

            for (int offsetY = 1; offsetY < sourceBitmap.Height - 1; offsetY++)
            {
                for (int offsetX = 1; offsetX < sourceBitmap.Width - 1; offsetX++)
                {
                    sourceOffset = offsetY * sourceData.Stride + offsetX * 4;
                    gradientValue = 0;
                    exceedsThreshold = true;

                    // Horizontal Gradient
                    CheckThreshold(pixelBuffer,
                                   sourceOffset - 4,
                                   sourceOffset + 4,
                                   ref gradientValue, threshold, 2);
                    // Vertical Gradient
                    exceedsThreshold =
                    CheckThreshold(pixelBuffer,
                                   sourceOffset - sourceData.Stride,
                                   sourceOffset + sourceData.Stride,
                                   ref gradientValue, threshold, 2);

                    if (exceedsThreshold == false)
                    {
                        gradientValue = 0;

                        // Horizontal Gradient
                        exceedsThreshold =
                        CheckThreshold(pixelBuffer,
                                       sourceOffset - 4,
                                       sourceOffset + 4,
                                       ref gradientValue, threshold);

                        if (exceedsThreshold == false)
                        {
                            gradientValue = 0;

                            // Vertical Gradient
                            exceedsThreshold =
                            CheckThreshold(pixelBuffer,
                                           sourceOffset - sourceData.Stride,
                                           sourceOffset + sourceData.Stride,
                                           ref gradientValue, threshold);

                            if (exceedsThreshold == false)
                            {
                                gradientValue = 0;

                                // Diagonal Gradient : NW-SE
                                CheckThreshold(pixelBuffer,
                                               sourceOffset - 4 - sourceData.Stride,
                                               sourceOffset + 4 + sourceData.Stride,
                                               ref gradientValue, threshold, 2);
                                // Diagonal Gradient : NE-SW
                                exceedsThreshold =
                                CheckThreshold(pixelBuffer,
                                               sourceOffset - sourceData.Stride + 4,
                                               sourceOffset - 4 + sourceData.Stride,
                                               ref gradientValue, threshold, 2);

                                if (exceedsThreshold == false)
                                {
                                    gradientValue = 0;

                                    // Diagonal Gradient : NW-SE
                                    exceedsThreshold =
                                    CheckThreshold(pixelBuffer,
                                                   sourceOffset - 4 - sourceData.Stride,
                                                   sourceOffset + 4 + sourceData.Stride,
                                                   ref gradientValue, threshold);

                                    if (exceedsThreshold == false)
                                    {
                                        gradientValue = 0;

                                        // Diagonal Gradient : NE-SW
                                        exceedsThreshold =
                                        CheckThreshold(pixelBuffer,
                                                       sourceOffset - sourceData.Stride + 4,
                                                       sourceOffset + sourceData.Stride - 4,
                                                       ref gradientValue, threshold);
                                    }
                                }
                            }
                        }
                    }

                    resultBuffer[sourceOffset] = (byte)(exceedsThreshold ? 255 : 0);
                    resultBuffer[sourceOffset + 1] = resultBuffer[sourceOffset];
                    resultBuffer[sourceOffset + 2] = resultBuffer[sourceOffset];
                    resultBuffer[sourceOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                    resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        private static bool CheckThreshold(byte[] pixelBuffer,
                                           int offset1, int offset2,
                                           ref int gradientValue,
                                           byte threshold,
                                           int divideBy = 1)
        {
            gradientValue +=
            Math.Abs(pixelBuffer[offset1] -
            pixelBuffer[offset2]) / divideBy;

            gradientValue +=
            Math.Abs(pixelBuffer[offset1 + 1] -
            pixelBuffer[offset2 + 1]) / divideBy;

            gradientValue +=
            Math.Abs(pixelBuffer[offset1 + 2] -
            pixelBuffer[offset2 + 2]) / divideBy;

            return (gradientValue >= threshold);
        }


    }
}
