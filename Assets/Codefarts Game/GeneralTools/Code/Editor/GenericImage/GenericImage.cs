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

    using Codefarts.Localization;

    /// <summary>
    /// Provides a class for a platform independent image.
    /// </summary>
    [Serializable]
    public class GenericImage
    {
        #region Constants and Fields

        /// <summary>
        /// Holds the image height.
        /// </summary>
        private readonly int height;

        /// <summary>
        /// Holds the actual pixel data.
        /// </summary>
        private Color[] pixelGrid;

        /// <summary>
        /// Holds the image width.
        /// </summary>
        private int width;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericImage"/> class.
        /// </summary>
        /// <param name="width">
        /// The width of the image.
        /// </param>
        /// <param name="height">
        /// The height of the image.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Width or Height can not be less then 1.
        /// </exception>
        public GenericImage(int width, int height)
        {
            var manager = LocalizationManager.Instance;
            if (width < 1)
            {
                throw new ArgumentOutOfRangeException("width", manager.Get("ERR_WidthCanNotBeLessThanOne"));
            }

            if (height < 1)
            {
                throw new ArgumentOutOfRangeException("height", manager.Get("ERR_HeightCanNotBeLessThanOne"));
            }

            this.width = width;
            this.height = height;
            this.pixelGrid = new Color[width * height];
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the height of the image.
        /// </summary>
        public int Height
        {
            get
            {
                return this.height;
            }

            set
            {
                if (this.height == value)
                {
                    return;
                }

                if (value < 1)
                {
                    var manager = LocalizationManager.Instance;
                    throw new ArgumentOutOfRangeException("value", manager.Get("ERR_HeightCanNotBeLessThanOne"));
                }

                var tmp = new GenericImage(this.width, value);
                tmp.Draw(this, 0, 0);
                this.pixelGrid = tmp.PixelGrid;
            }
        }

        /// <summary>
        /// Gets a array of <see cref="Color"/> types.
        /// </summary>
        public Color[] PixelGrid
        {
            get
            {
                return this.pixelGrid;
            }
        }

        /// <summary>
        /// Gets or sets the width of the image.
        /// </summary>
        public int Width
        {
            get
            {
                return this.width;
            }

            set
            {
                if (this.width == value)
                {
                    return;
                }

                if (value < 1)
                {
                    var manager = LocalizationManager.Instance;
                    throw new ArgumentOutOfRangeException("value", manager.Get("ERR_WidthCanNotBeLessThanOne"));
                }

                this.width = value;
                var tmp = new GenericImage(value, this.height);
                tmp.Draw(this, 0, 0);
                this.pixelGrid = tmp.PixelGrid;
            }
        }

        #endregion

        #region Public Indexers

        /// <summary>
        /// Gets or sets the color at a specific pixel position.
        /// </summary>
        /// <param name="x">
        /// The x position of the pixel.
        /// </param>
        /// <param name="y">
        /// The y position of the pixel.
        /// </param>
        /// <returns>Returns the color at the specified pixel location.</returns>
        public Color this[int x, int y]
        {
            get
            {
                return this.PixelGrid[(y * this.width) + x];
            }

            set
            {
                this.PixelGrid[(y * this.width) + x] = value;
            }
        }

        #endregion


        /// <summary>
        /// Gets the hash code for the image.
        /// </summary>
        /// <returns>
        /// The get hash code.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var data = this.ToByteArray();
                var result = 0;
                foreach (byte b in data)
                {
                    result = (result * 31) ^ b;
                }

                return result;
            }


            //int hash = data[0] | data[1] >> 8 | data[2] >> 16 | data[3] >> 24;
            //for (int i = 4; i < data.Length; i += 4)
            //{
            //    hash ^= data[i] | data[i + 1] >> 8 | data[i + 2] >> 16 | data[i + 3] >> 24;
            //}

            //return hash;
        }
    }
}