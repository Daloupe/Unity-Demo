/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.GenericImage
{
    using System;
    using Codefarts.GeneralTools.Common;
    using System.IO;

    using Codefarts.Localization;

    /// <summary>
    /// Contains extension methods for the <see cref="GenericImage"/> type.
    /// </summary>
    public static class GenericImageExtensionMethods
    {
        #region Public Methods and Operators

        /// <summary>
        /// Clears a image.
        /// </summary>
        /// <param name="image">
        /// The source image.
        /// </param>
        /// <param name="color">
        /// The color to use for the clear.
        /// </param>
        public static void Clear(this GenericImage image, Color color)
        {
            for (var idx = 0; idx < image.Width; idx++)
            {
                for (var idy = 0; idy < image.Height; idy++)
                {
                    image[idx, idy] = color;
                }
            }
        }

        /// <summary>
        /// Clones an image.
        /// </summary>
        /// <param name="image">
        /// The source image.
        /// </param>
        /// <returns>
        /// Returns a new image contains the same pixel data.
        /// </returns>
        public static GenericImage Clone(this GenericImage image)
        {
            var newImage = new GenericImage(image.Width, image.Height);
            for (int idx = 0; idx < image.Width; idx++)
            {
                for (int idy = 0; idy < image.Height; idy++)
                {
                    newImage[idx, idy] = image[idx, idy];
                }
            }

            return newImage;
        }

        /// <summary>
        /// Draws a image within another image.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="source">
        /// The source image.
        /// </param>
        /// <param name="x">
        /// The x position in the destination image.
        /// </param>
        /// <param name="y">
        /// The y position in the destination image.
        /// </param>
        public static void Draw(this GenericImage image, GenericImage source, int x, int y)
        {
            Draw(image, source, x, y, source.Width, source.Height, 0, 0, source.Width, source.Height);
        }

        /// <summary>
        /// Draws a image within another image.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="source">
        /// The source image.
        /// </param>
        /// <param name="x">
        /// The x position in the destination image.
        /// </param>
        /// <param name="y">
        /// The y position in the destination image.
        /// </param>
        /// <param name="width">
        /// The destination width of the drawn image.
        /// </param>
        /// <param name="height">
        /// The destination height of the drawn image.
        /// </param>
        public static void Draw(this GenericImage image, GenericImage source, int x, int y, int width, int height)
        {
            Draw(image, source, x, y, width, height, 0, 0, source.Width, source.Height);
        }

        /// <summary>
        /// Draws a image within another image.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="source">
        /// The source image.
        /// </param>
        /// <param name="x">
        /// The x position in the destination image.
        /// </param>
        /// <param name="y">
        /// The y position in the destination image.
        /// </param>
        /// <param name="srcX">
        /// The x position in the source image.
        /// </param>
        /// <param name="srcY">
        /// The y position in the source image.
        /// </param>
        /// <param name="srcWidth">
        /// The source width.
        /// </param>
        /// <param name="srcHeight">
        /// The source height.
        /// </param>
        public static void Draw(this GenericImage image, GenericImage source, int x, int y, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            Draw(image, source, x, y, srcWidth, srcHeight, srcX, srcY, srcWidth, srcHeight);
        }

        /// <summary>
        /// Draws a image within another image.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="source">
        /// The source image.
        /// </param>
        /// <param name="x">
        /// The x position in the destination image.
        /// </param>
        /// <param name="y">
        /// The y position in the destination image.
        /// </param>
        /// <param name="width">
        /// The destination width of the drawn image.
        /// </param>
        /// <param name="height">
        /// The destination height of the drawn image.
        /// </param>
        /// <param name="srcX">
        /// The x position in the source image.
        /// </param>
        /// <param name="srcY">
        /// The y position in the source image.
        /// </param>
        /// <param name="srcWidth">
        /// The source width.
        /// </param>
        /// <param name="srcHeight">
        /// The source height.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If source is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If width, height, srcWidth or srcHeight are less then 1.
        /// </exception>                                       
        public static void Draw(this GenericImage image, GenericImage source, int x, int y, int width, int height, int srcX, int srcY, int srcWidth, int srcHeight)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var local = LocalizationManager.Instance;
            if (srcWidth < 1)
            {
                throw new ArgumentOutOfRangeException("srcWidth", local.Get("ERR_SourceWidthCanNotBeLessThanOne"));
            }

            if (srcHeight < 1)
            {
                throw new ArgumentOutOfRangeException("srcHeight", local.Get("ERR_SourceHeightCanNotBeLessThanOne"));
            }

            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", local.Get("ERR_WidthCanNotBeLessThanOne"));
            }

            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", local.Get("ERR_HeightCanNotBeLessThanOne"));
            }

            var scaledWidth = (float)width / srcWidth;
            var scaledHeight = (float)height / srcHeight;

            if (x > image.Width - 1)
            {
                return;
            }

            if (y > image.Height - 1)
            {
                return;
            }

            if (srcX > source.Width - 1)
            {
                return;
            }

            if (srcY > source.Height - 1)
            {
                return;
            }

            //if (srcX < -source.Width - 1)
            //{
            //    return;
            //}

            //if (srcY < -source.Height - 1)
            //{
            //    return;
            //}

            // copy src data to temp image
            var tmp = new GenericImage(srcWidth, srcHeight);

            for (int idy = 0; idy < srcHeight; idy++)
            {
                for (int idx = 0; idx < srcWidth; idx++)
                {
                    int xpos = idx + srcX;
                    int ypos = idy + srcY;

                    // this needs to be better optimized. We are processing all pixels even though some of 
                    // the pixels could be cropped off. Need to calc the rect the copy pixels and process only that rect
                    if (xpos > source.Width - 1)
                    {
                        continue;
                    }

                    if (ypos > source.Height - 1)
                    {
                        continue;
                    }

                    if (xpos < 0)
                    {
                        continue;
                    }

                    if (ypos < 0)
                    {
                        continue;
                    }

                    tmp[idx, idy] = source[xpos, ypos];
                }
            }

            var scaled = tmp.Scale(scaledWidth, scaledHeight);

            if (x < -scaled.Width - 1)
            {
                return;
            }

            if (y < -scaled.Height - 1)
            {
                return;
            }

            for (int idy = 0; idy < scaled.Height; idy++)
            {
                for (int idx = 0; idx < scaled.Width; idx++)
                {
                    int destX = idx + x;
                    int destY = idy + y;

                    // this needs to be better optimized. We are processing all pixels even though some of 
                    // the pixels could be cropped off. Need to calc the rect the copy pixels and process only that rect
                    if (destX > image.Width - 1)
                    {
                        continue;
                    }

                    if (destY > image.Height - 1)
                    {
                        continue;
                    }

                    if (destX < 0)
                    {
                        continue;
                    }

                    if (destY < 0)
                    {
                        continue;
                    }

                    // var color = scaled[idx + srcX, idy + srcY];
                    Color color = scaled[idx, idy];
                    image[destX, destY] = image[destX, destY].Blend(color);
                }
            }
        }

        /// <summary>
        /// Draws a line on an <see cref="GenericImage"/>.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="x1">
        /// The x position for the start of the line.
        /// </param>
        /// <param name="y1">
        /// The y position for the start of the line.
        /// </param>
        /// <param name="x2">
        /// The x position for the end of the line.
        /// </param>
        /// <param name="y2">
        /// The y position for the end of the line.
        /// </param>
        /// <param name="color">
        /// The color that the line will be drawn with.
        /// </param>
        public static void DrawLine(this GenericImage image, int x1, int y1, int x2, int y2, Color color)
        {
             var posX = x1;
            var posY = y1;
            var point2Point = new Vector2(x2, y2) - new Vector2(x1, y1);
            var dist = point2Point.Length();
            var dir = Vector2.Normalize(point2Point);
            var vec = new Vector2(posX,posY);

            for (int i = 0; i < dist + 1; i++)
            {
                if (posX < 0)
                {
                    continue;
                }

                if (posX > image.Width - 1)
                {
                    continue;
                }

                if (posY < 0)
                {
                    continue;
                }

                if (posY > image.Height - 1)
                {
                    continue;
                }

                image[posX, posY] = image[posX, posY].Blend(color);

                vec += dir;
                posX = (int)Math.Round(vec.X,4);
                posY = (int)Math.Round(vec.Y,4);
            }
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="x">
        /// The x position of the left side of the rectangle.
        /// </param>
        /// <param name="y">
        /// The y position of the top side of the rectangle.
        /// </param>
        /// <param name="width">
        /// The width of the rectangle.
        /// </param>
        /// <param name="height">
        /// The height of the rectangle.
        /// </param>
        /// <param name="color">
        /// The color to use.
        /// </param>
        public static void DrawRectangle(this GenericImage image, int x, int y, int width, int height, Color color)
        {
            var bottom = y + height - 1;
            var right = x + width - 1;

            // allows negative widths
            if (x > right)
            {
                var tmp = right;
                right = x;
                x = tmp;
            }

            // allows negative heights
            if (y > bottom)
            {
                var tmp = bottom;
                bottom = y;
                y = tmp;
            }

            // top 
            if (y > -1 && y < image.Height)
            {
                var lft = x < 0 ? 0 : x;
                var rgt = right > image.Width - 1 ? image.Width - 1 : right;
                for (int i = lft; i <= rgt; i++)
                {
                    image[i, y] = image[i, y].Blend(color);
                }
            }

            // bottom 
            if (bottom > -1 && bottom < image.Height)
            {
                var lft = x < 0 ? 0 : x;
                var rgt = right > image.Width - 1 ? image.Width - 1 : right;
                for (int i = lft; i <= rgt; i++)
                {
                    image[i, bottom] = image[i, bottom].Blend(color);
                }
            }

            // left 
            if (x > -1 && x < image.Width)
            {
                var tp = y < 0 ? 0 : y;
                var btm = bottom > image.Height - 1 ? image.Height - 1 : bottom;
                for (int i = tp; i <= btm; i++)
                {
                    image[x, i] = image[x, i].Blend(color);
                }
            }

            // right 
            if (right > -1 && right < image.Width)
            {
                var tp = y < 0 ? 0 : y;
                var btm = bottom > image.Height - 1 ? image.Height - 1 : bottom;
                for (int i = tp; i <= btm; i++)
                {
                    image[right, i] = image[right, i].Blend(color);
                }
            }


            //    // top/bottom
            //    for (int i = 0; i < width; i++)
            //    {
            //        if (i + x >= 0 && i + x < image.Width)
            //        {
            //            image[i + x, y] = image[i + x, y].Blend(color); 
            //            if (y + height - 1 >= 0 && y + height - 1 < image.Height)
            //            {
            //                image[i + x, y + height - 1] = image[i + x, y + height - 1].Blend(color);
            //            }
            //        }
            //    }


            //// left/right
            // for (int i = 0; i < height; i++)
            // {
            //     if (i + y >= 0 && i + y < image.Height)
            //     {
            //         image[x, i + y] = image[x, i + y].Blend(color);  
            //         if (x + width - 1 >= 0 && x + width - 1 < image.Width)
            //         {
            //             image[x + width-1, i + y] = image[x + width-1, i + y].Blend(color);
            //         }
            //     }
            // }
        }

        /// <summary>
        /// Draws a filled rectangle.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="x">
        /// The x position of the left side of the rectangle.
        /// </param>
        /// <param name="y">
        /// The y position of the top side of the rectangle.
        /// </param>
        /// <param name="width">
        /// The width of the rectangle.
        /// </param>
        /// <param name="height">
        /// The height of the rectangle.
        /// </param>
        /// <param name="color">
        /// The color to use.
        /// </param>
        public static void FillRectangle(this GenericImage image, int x, int y, int width, int height, Color color)
        {
            for (int idx = 0; idx < width; idx++)
            {
                for (int idy = 0; idy < height; idy++)
                {
                    if (idx + x > image.Width - 1)
                    {
                        continue;
                    }

                    if (idy + y > image.Height - 1)
                    {
                        continue;
                    }

                    if (idx + x < -width)
                    {
                        continue;
                    }

                    if (idy + y < -height)
                    {
                        continue;
                    }

                    image[idx + x, idy + y] = image[idx + x, idy + y].Blend(color);
                }
            }
        }


        /// <summary>
        /// Draws a Ellipse.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="x">
        /// The x position of the left side of the Ellipse.
        /// </param>
        /// <param name="y">
        /// The y position of the top side of the Ellipse.
        /// </param>
        /// <param name="width">
        /// The width of the Ellipse.
        /// </param>
        /// <param name="height">
        /// The height of the Ellipse.
        /// </param>
        /// <param name="color">
        /// The color to use.
        /// </param>
        public static void DrawEllipse(this GenericImage image, int x, int y, int width, int height, Color color)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Draws a filled Ellipse.
        /// </summary>
        /// <param name="image">
        /// The destination image.
        /// </param>
        /// <param name="x">
        /// The x position of the left side of the Ellipse.
        /// </param>
        /// <param name="y">
        /// The y position of the top side of the Ellipse.
        /// </param>
        /// <param name="width">
        /// The width of the Ellipse.
        /// </param>
        /// <param name="height">
        /// The height of the Ellipse.
        /// </param>
        /// <param name="color">
        /// The color to use.
        /// </param>
        public static void FillEllipse(this GenericImage image, int x, int y, int width, int height, Color color)
        {
            throw new NotImplementedException();
        }

        /*  /// <summary>
         /// The get hash.
         /// </summary>
         /// <param name="image">
         /// The image.
         /// </param>
         /// <param name="x">
         /// The x.
         /// </param>
         /// <param name="y">
         /// The y.
         /// </param>
         /// <param name="width">
         /// The width.
         /// </param>
         /// <param name="height">
         /// The height.
         /// </param>
         /// <returns>
         /// The get hash.
         /// </returns>
         public static decimal GetHash(this GenericImage image, int x, int y, int width, int height)
         {
             decimal total = 0m;
             if (x > image.Width - 1)
             {
                 return 0;
             }

             if (y > image.Height - 1)
             {
                 return 0;
             }

             if (x < -image.Width - 1)
             {
                 return 0;
             }

             if (y < -image.Height - 1)
             {
                 return 0;
             }

             for (int idx = 0; idx < width; idx++)
             {
                 for (int idy = 0; idy < height; idy++)
                 {
                     if (idx + x > image.Width - 1)
                     {
                         continue;
                     }

                     if (idy + y > image.Height - 1)
                     {
                         continue;
                     }

                     if (idx + x < 0)
                     {
                         continue;
                     }

                     if (idy + y < 0)
                     {
                         continue;
                     }

                     Color color = image[x + idx, y + idy];

                     // total += (color.A + color.R + color.G + color.B + x + y + idx + idy) / 8;
                     total += (color.A * x) + (color.R * y) + (color.G * x) + (color.B * y) + x + y + idx + idy;
                     continue;
                     total += color.A;
                     total += color.R;
                     total += color.G;
                     total += color.B;
                     total += x + idx;
                     total += y + idy;
                 }
             }

             return total; // image.PixelGrid.Length;
         }
               */

        /// <summary>
        /// Generates a normal map.
        /// </summary>
        /// <param name="image">
        /// The source image to generate the normal map from.
        /// </param>
        /// <returns>
        /// Returns a new <see cref="GenericImage"/> containing the normal map
        /// </returns>
        public static GenericImage NormalMap(this GenericImage image)
        {
            var newImage = new GenericImage(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                var tempVector = new Vector3(0.0f, 0.0f, 0.0f);
                for (int x = 0; x < image.Width; ++x)
                {
                    Color tempPixel = image[x, y];
                    tempVector.X = tempPixel.R / 255.0f;
                    tempVector.Y = tempPixel.R / 255.0f;
                    tempVector.Z = 1.0f;
                    tempVector.Normalize();
                    tempVector.X = ((tempVector.X + 1.0f) / 2.0f) * 255.0f;
                    tempVector.Y = ((tempVector.Y + 1.0f) / 2.0f) * 255.0f;
                    tempVector.Z = ((tempVector.Z + 1.0f) / 2.0f) * 255.0f;
                    newImage[x, y] = new Color((byte)tempVector.X, (byte)tempVector.Y, (byte)tempVector.Z, 255);
                }
            }

            return newImage;
        }

        /// <summary>
        /// Rotates the image.
        /// </summary>
        /// <param name="image">
        /// The source image to rotate.
        /// </param>
        /// <param name="angle">
        /// The angle in degrees to rotate the image.
        /// </param>
        /// <returns>
        /// Returns a new rotated image.
        /// </returns>
        public static GenericImage Rotate(this GenericImage image, float angle)
        {
            return image.Clone();
        }

        /// <summary>
        /// Saves the image data to a stream.
        /// </summary>
        /// <param name="image">
        /// The source image.
        /// </param>
        /// <param name="stream">
        /// The stream that will be written to.
        /// </param>
        /// <remarks>Only writes the raw pixel data and does not include any width, height, color depth info etc.</remarks>
        public static void Save(this GenericImage image, Stream stream)
        {
            byte[] data = image.ToByteArray();
            stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Scales a image.
        /// </summary>
        /// <param name="image">
        /// The source image.
        /// </param>
        /// <param name="x">
        /// The horizontal scale.
        /// </param>
        /// <param name="y">
        /// The vertical scale.
        /// </param>
        /// <returns>
        /// Returns a new scaled image.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// x and y values must be greater then 0.
        /// </exception>
        public static GenericImage Scale(this GenericImage image, float x, float y)
        {
            var manager = LocalizationManager.Instance;
            if (x <= 0)
            {
                throw new ArgumentOutOfRangeException("x", manager.Get("ERR_XMustBeGReaterThanZero"));
            }

            if (y <= 0)
            {
                throw new ArgumentOutOfRangeException("y", manager.Get("ERR_YMustBeGReaterThanZero"));
            }

            if (Math.Abs(x - 1.0f) < 0 && Math.Abs(y - 1.0f) < 0)
            {
                return image.Clone();
            }

            var width = (int)(image.Width * x);
            var height = (int)(image.Height * y);
            if (width < 1)
            {
                width = 1;
            }

            if (height < 1)
            {
                height = 1;
            }

            var scaled = new GenericImage(width, height);

            for (int idy = 0; idy < scaled.Height; idy++)
            {
                for (int idx = 0; idx < scaled.Width; idx++)
                {
                    float u = scaled.Width == 1 ? 1 : idx / (float)(scaled.Width - 1);
                    float v = scaled.Height == 1 ? 1 : idy / (float)(scaled.Height - 1);
                    var srcPos = new Point((int)Math.Round(u * (image.Width - 1)), (int)Math.Round(v * (image.Height - 1)));
                    scaled[idx, idy] = image[srcPos.X, srcPos.Y];
                }
            }

            return scaled;
        }

        /// <summary>
        /// Color tints an image.
        /// </summary>
        /// <param name="image">
        /// The image to color tint.
        /// </param>
        /// <param name="color">
        /// The color to use as the tint.
        /// </param>
        /// <param name="type">
        /// The type to tint to perform.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Unsupported type specified.
        /// </exception>
        public static void Tint(this GenericImage image, Color color, TintType type)
        {
            for (var idx = 0; idx < image.Width; idx++)
            {
                for (var idy = 0; idy < image.Height; idy++)
                {
                    var sourceColor = image[idx, idy];

                    var r = sourceColor.R;
                    var g = sourceColor.G;
                    var b = sourceColor.B;
                    var a = sourceColor.A;

                    switch (type)
                    {
                        case TintType.Alpha:

                            ////////////////////// ALPHA ////////////////////////
                            sourceColor.R = (byte)((r * color.R) / 255);
                            sourceColor.G = (byte)((g * color.G) / 255);
                            sourceColor.B = (byte)((b * color.B) / 255);
                            sourceColor.A = (byte)((a * color.A) / 255);
                            break;

                        case TintType.Multiply:

                            /////////////////////// MULTIPLY //////////////////////////////
                            // Faster than a division like (s * d) / 255 are 2 shifts and 2 adds
                            var ta = (a * color.A) + 128;
                            var tr = (r * color.R) + 128;
                            var tg = (g * color.G) + 128;
                            var tb = (b * color.B) + 128;

                            var ba = ((ta >> 8) + ta) >> 8;
                            var br = ((tr >> 8) + tr) >> 8;
                            var bg = ((tg >> 8) + tg) >> 8;
                            var bb = ((tb >> 8) + tb) >> 8;

                            sourceColor.R = (byte)ba;
                            sourceColor.G = (byte)(ba <= br ? ba : br);
                            sourceColor.B = (byte)(ba <= bg ? ba : bg);
                            sourceColor.A = (byte)(ba <= bb ? ba : bb);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("type");
                    }
                }
            }
        }

        /// <summary>
        /// Converts image pixel data to a byte array.
        /// </summary>
        /// <param name="image">
        /// The source image.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the pixel data.
        /// </returns>
        public static byte[] ToByteArray(this GenericImage image)
        {
            return image.ToByteArray(0, 0, image.Width, image.Height);
        }

        /// <summary>
        /// Converts image pixel data to a byte array.
        /// </summary>
        /// <param name="image">
        /// The source image.
        /// </param>
        /// <param name="x">
        /// The x position of the left side of the rectangle.
        /// </param>
        /// <param name="y">
        /// The y position of the top side of the rectangle.
        /// </param>
        /// <param name="width">
        /// The width of the rectangle.
        /// </param>
        /// <param name="height">
        /// The height of the rectangle.
        /// </param>
        /// <returns>
        /// Returns an array of bytes containing the pixel data.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If x, y is out of bounds of the images dimensions. The width or height is less then 1.
        /// </exception>       
        /// <exception cref="ArgumentException">
        /// Clipped width or height is less then 1.
        /// </exception>                        
        public static byte[] ToByteArray(this GenericImage image, int x, int y, int width, int height)
        {
            var manager = LocalizationManager.Instance;
            if (x > image.Width - 1)
            {
                throw new ArgumentOutOfRangeException("x", manager.Get("ERR_XBeyondImageWidth"));
            }

            if (y > image.Height - 1)
            {
                throw new ArgumentOutOfRangeException("y", manager.Get("ERR_YBeyondImageWidth"));
            }

            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", manager.Get("ERR_WidthCanNotBeLessThanOne"));
            }

            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", manager.Get("ERR_HeightCanNotBeLessThanOne"));
            }

            if (x + width - 1 > image.Width - 1)
            {
                width = image.Width - 1 - x;
            }

            if (y + height - 1 > image.Height - 1)
            {
                height = image.Height - 1 - y;
            }

            if (width < 1)
            {
                throw new ArgumentException(manager.Get("ERR_WidthClipedByteDataLessOne"));
            }

            if (height < 1)
            {
                throw new ArgumentException(manager.Get("ERR_HeightClipedByteDataLessOne"));
            }

            if (x < 0)
            {
                width = x + width;
                x = 0;
            }

            if (y < 0)
            {
                height = y + height;
                y = 0;
            }

            if (width < 1)
            {
                throw new ArgumentException(manager.Get("ERR_WidthClipedByteDataLessOne"));
            }

            if (height < 1)
            {
                throw new ArgumentException(manager.Get("ERR_HeightClipedByteDataLessOne"));
            }

            var data = new byte[width * height * 4];
            var pos = 0;

            for (var idy = 0; idy < height; idy++)
            {
                for (var idx = 0; idx < width; idx++)
                {
                    int ypos = idy + y;
                    int xpos = idx + x;

                    Color color = image[xpos, ypos];
                    data[pos++] = color.R;
                    data[pos++] = color.G;
                    data[pos++] = color.B;
                    data[pos++] = color.A;
                }
            }

            return data;
        }

        public static void FlipVertically(this GenericImage image)
        {
            // TODO: Cloning is easy but memory intensive replace with proper code
            var pixels = image.Clone();
            for (var idy = 0; idy < image.Height; idy++)
            {
                for (var idx = 0; idx < image.Width; idx++)
                {
                    image[idx, idy] = pixels[idx, pixels.Height - 1 - idy];
                }
            }
        }

        public static void FlipHorizontally(this GenericImage image)
        {
            // TODO: Cloning is easy but memory intensive replace with proper code
            var pixels = image.Clone();
            for (var idy = 0; idy < image.Height; idy++)
            {
                for (var idx = 0; idx < image.Width; idx++)
                {
                    image[idx, idy] = pixels[pixels.Width - 1 - idx, idy];
                }
            }
        }

        #endregion
    }
}