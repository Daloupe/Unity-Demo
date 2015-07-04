/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.Utilities
{
    using System;
    using System.Collections.Generic;
    using Codefarts.GeneralTools.Common;
    using System.Threading;

    using Codefarts.GeneralTools.GenericImage;

    /// <summary>
    /// Utility for detecting 2D tiles in a image.
    /// </summary>
    public class TileSetDetector
    {
        #region Constants and Fields

        ///// <summary>
        ///// The byte data.
        ///// </summary>
        //private readonly Dictionary<string, long> byteData = new Dictionary<string, long>();

        /// <summary>
        /// The map texture that will be searched for tiles.
        /// </summary>
        private readonly GenericImage mapTexture;

        /// <summary>
        /// The tile locations.
        /// </summary>
        private readonly Dictionary<uint, IList<Point>> tileLocations = new Dictionary<uint, IList<Point>>();

        /// <summary>
        /// The current step during processing.
        /// </summary>
        private int currentStep;

        /// <summary>
        /// Holds value whether processing is running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Used to determine the value of the next detected tile id.
        /// </summary>
        private int nextInt;

        /// <summary>
        /// The percentage completed.
        /// </summary>
        private volatile float percentageCompleted;

        /// <summary>
        /// The processed tiles.
        /// </summary>
        private volatile int processedTiles;

        /// <summary>
        /// The processing point.
        /// </summary>
        private Point processingPoint = Point.Empty;

        /// <summary>
        /// Holds the tile width used for tile detection.
        /// </summary>
        private int tileSizeWidth;

        /// <summary>
        /// Holds the tile height used for tile detection.
        /// </summary>
        private int tileSizeHeight;

        /// <summary>
        /// The total tiles.
        /// </summary>
        private volatile int totalTiles;

        /// <summary>
        /// The unique tiles.
        /// </summary>
        private volatile int uniqueTiles;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TileSetDetector"/> class.
        /// </summary>
        /// <param name="texture">
        /// The texture.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the texture argument is null.
        /// </exception>
        public TileSetDetector(GenericImage texture)
        {
            this.tileSizeWidth = 16;
            this.tileSizeHeight = 16;
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            this.mapTexture = texture;
            this.NumberOfTilesToProcessAtATime = 10;

            // this.md5CryptoServiceProvider = new MD5CryptoServiceProvider();
        }

        #endregion

        ///// <summary>
        ///// Allows the game component to update itself.
        ///// </summary>
        // public void Update()
        // {
        // if(isRunning) return;
        // // TODO: Add your update code here
        // switch (this.currentStep)
        // {
        // case 0:
        // // load the map layer image
        // this.DoInitVars();
        // break;

        // case 1:
        // // this step placed in 'for' loop to increase map processing speed
        // this.DoProcessing();
        // break;

        // }
        // }

        // private long CalcHash(GenericImage bmp, int x, int y)
        // {
        // var mem = new System.IO.MemoryStream();
        // var tmpBmp = new GenericImage(this.TileSize.Width, this.TileSize.Height);
        // tmpBmp.Clear(Color.Transparent);
        // tmpBmp.Draw(bmp, 0, 0, x, y, this.TileSize.Width, this.TileSize.Height);
        // return tmpBmp.ToByteArray().GetHashCode();
        // return tmpBmp.GetHash();
        // var crc = new ICSharpCode.SharpZipLib.Checksums.Crc32();
        // crc.Update(tmpBmp.ToByteArray());
        // return crc.Value;

        // tmpBmp.Save(mem);
        // //bmp.Save(mem);
        // mem.Position = 0;
        // var data = new byte[mem.Length];
        // mem.Read(data, 0, data.Length);
        // mem.Close();
        // mem.Dispose();

        // var tmpHash = new MD5CryptoServiceProvider().ComputeHash(data);
        // var tmpHash = this.md5CryptoServiceProvider.ComputeHash(data);
        // var tmpHash = this.md5CryptoServiceProvider.ComputeHash(tmpBmp.ToByteArray());
        // return tmpHash.GetHashCode();
        // var str = this.ByteArrayToString(tmpHash);
        // var str = this.ByteArrayToHashString(tmpBmp.ToByteArray());
        // var str = this.ByteArrayToHashString(bmp.ToByteArray(x, y, this.TileSize.Width, this.TileSize.Height));
        // return str.GetHashCode();
        // }

        // private string ByteArrayToString(byte[] arrInput)
        // {
        // int i;
        // var sOutput = string.Empty;
        // for (i = 0; i < arrInput.Length; i++)
        // {
        // sOutput.Append(arrInput[i].ToString("X2"));
        // }
        // return sOutput.ToString();
        // }

        // private string ByteArrayToHashString(byte[] arrInput)
        // {
        // int i;
        // var sOutput = string.Empty;// new StringBuilder(arrInput.Length);
        // for (i = 0; i < arrInput.Length; i++)
        // {
        // sOutput.Append(arrInput[i].ToString("X2"));
        // sOutput += (char)arrInput[i];//.ToString("X2");}
        // }
        // return sOutput;
        // string someText = "The quick brown fox jumps over the lazy dog.";
        // byte[] bytes = Encoding.Unicode.GetBytes(someText);
        // char[] chars = Encoding.Unicode.GetChars(arrInput);
        // return new string(chars);
        // }

        // private string ByteArrayToHashString(byte[] arrInput)
        // {
        // int i;
        // var sOutput = new StringBuilder(arrInput.Length);
        // for (i = 0; i < arrInput.Length; i++)
        // {
        // sOutput.Append(arrInput[i].ToString("X"));
        // }
        // return sOutput.ToString();
        // }
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether or not the processing has completed.
        /// </summary>
        public bool Complete
        {
            get
            {
                return this.currentStep > 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether processing is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        /// <summary>
        /// Gets or sets the number of tiles to process at a time.
        /// </summary>
        public int NumberOfTilesToProcessAtATime { get; set; }

        /// <summary>
        /// Gets the percentage completed.
        /// </summary>
        /// <remarks>The value will be between 0.0 and 1.0</remarks>
        public float PercentageCompleted
        {
            get
            {
                return this.percentageCompleted;
            }
        }

        /// <summary>
        /// Gets the number of processed tiles.
        /// </summary>
        public int ProcessedTiles
        {
            get
            {
                return this.processedTiles;
            }
        }

        /// <summary>
        /// Gets a IEnumerable of tile id's.
        /// </summary>
        public IEnumerable<uint> TileIds
        {
            get
            {
                return this.tileLocations.Keys;
            }
        }

        /// <summary>
        /// Gets or sets the width of the tiles to be detected.
        /// </summary>
        /// <remarks>If <see cref="IsRunning"/> property is true setting this property does nothing.</remarks>
        public int TileWidth
        {
            get
            {
                return this.tileSizeWidth;
            }

            set
            {
                if (this.isRunning)
                {
                    return;
                }

                this.tileSizeWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the tiles to be detected.
        /// </summary>
        /// <remarks>If <see cref="IsRunning"/> property is true setting this property does nothing.</remarks>
        public int TileHeight
        {
            get
            {
                return this.tileSizeHeight;
            }

            set
            {
                if (this.isRunning)
                {
                    return;
                }

                this.tileSizeHeight = value;
            }
        }

        /// <summary>
        /// Gets the total possible tiles given the source image and tile size.
        /// </summary>
        public int TotalTiles
        {
            get
            {
                return this.totalTiles;
            }
        }

        /// <summary>
        /// Gets the number of unique tiles that have been detected.
        /// </summary>
        public int UniqueTiles
        {
            get
            {
                return this.uniqueTiles;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Cancels processing.
        /// </summary>
        public void Cancel()
        {
            this.isRunning = false;
        }

        /// <summary>
        /// Gets a <see cref="GenericImage"/> using a tile id.
        /// </summary>
        /// <param name="id">
        /// The id of a tile to get the image of.
        /// </param>
        /// <returns>
        /// Returns a <see cref="GenericImage"/> of the tile.
        /// </returns>
        public GenericImage GetTileImage(uint id)
        {
            if (this.mapTexture == null)
            {
                return null;
            }

            IList<Point> tile = this.tileLocations[id];
            var img = new GenericImage(this.tileSizeWidth, this.tileSizeHeight);

            // img.Draw(this.mapTexture, 0, 0, tile[0].X * this.TileSize.Width, tile[0].Y * this.TileSize.Height, this.TileSize.Width, this.TileSize.Height);
            img.Draw(this.mapTexture, 0, 0, img.Width, img.Height, tile[0].X, tile[0].Y, img.Width, img.Height);
            return img;
        }

        /// <summary>
        /// Gets the tile positions for a tile id.
        /// </summary>
        /// <param name="tileId">
        /// The tile id to retrieve additional locations for.
        /// </param>
        /// <returns>
        /// A detected tile may be detected at different locations within the image. The first entry in the list at index 0 is always the first detected instance of that tile.
        /// Any other points in the list are the pixel location
        /// </returns>
        public IList<Point> GetTilePoints(uint tileId)
        {
            return this.tileLocations[tileId];
        }

        ///// <summary>
        ///// The get tile string.
        ///// </summary>
        ///// <param name="id">
        ///// The id of the tile to get.
        ///// </param>
        ///// <returns>
        ///// Returns a string containing the tile pixel data.
        ///// </returns>
        //public string GetTileString(long id)
        //{
        //    foreach (var pair in this.byteData)
        //    {
        //        if (pair.Value == id)
        //        {
        //            return pair.Key;
        //        }
        //    }

        //    return null;
        //}

        /// <summary>
        /// Begins running the process of detecting tiles.
        /// </summary>
        public void Run()
        {
            if (this.isRunning)
            {
                return;
            }

            this.isRunning = true;
            ThreadPool.QueueUserWorkItem(
                cb =>
                    {
                        this.DoInitVars();
                        while (!this.Complete && this.isRunning)
                        {
                            this.DoProcessing();
                        }

                        this.isRunning = false;
                    });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes internal variables and gets ready for processing.
        /// </summary>
        private void DoInitVars()
        {
            this.totalTiles = (this.mapTexture.Width / this.tileSizeWidth) * (this.mapTexture.Height / this.tileSizeHeight);
            this.processedTiles = 0;
            this.currentStep++;
        }

        /// <summary>
        /// The do processing.
        /// </summary>
        private void DoProcessing()
        {
            // lock (thisLock)
            // {
            for (int processIndexer = 0; processIndexer < this.NumberOfTilesToProcessAtATime; processIndexer++)
            {
                // calc the hash for the next block location
                // var r = new Rect(this.processingPoint.X, this.processingPoint.Y, this.TileSize.Width, this.TileSize.Height);
                // var hashValue = this.CalcHash(this.mapTexture, this.processingPoint.X, this.processingPoint.Y);
                // var hashValue = this.mapTexture.GetHash(this.processingPoint.X, this.processingPoint.Y, this.TileSize.Width, this.TileSize.Height);

                // var str = this.ByteArrayToHashString(this.mapTexture.ToByteArray(this.processingPoint.X, this.processingPoint.Y, this.TileSize.Width, this.TileSize.Height));
                // var str = Convert.ToBase64String(this.mapTexture.ToByteArray(this.processingPoint.X, this.processingPoint.Y, this.TileSize.Width, this.TileSize.Height));
                byte[] bytes = this.mapTexture.ToByteArray(this.processingPoint.X, this.processingPoint.Y, this.tileSizeWidth, this.tileSizeHeight);

                uint hashCode = this.GetHashCodeFromByteArray(bytes);
                //   string str = string.Empty;
              //  UnityEngine.Debug.Log("here");
             //   for (int i = 4; i < bytes.Length; i += 4)
              //  {
              //      hashCode ^= bytes[i] * bytes[i + 1] * bytes[i + 2] * bytes[i + 3];

                    //     str += bytes[i] + " ";
              //  }

                // str = str.Trim().Replace(" ", ",");

                // check if the hash value has already been added
                 if (!this.tileLocations.ContainsKey(hashCode))
                //if (!this.byteData.ContainsKey(str))
                {
                    // it has not yet been added so add it
                    //this.byteData.Add(str, this.nextInt);
                    //this.tileLocations.Add(this.nextInt, new List<Point>());
                    //this.nextInt++;

                    this.tileLocations.Add(hashCode, new List<Point>());

                    // this.tileLocations.Add(hashValue, new List<Point>());
                    this.uniqueTiles++;
                }

                // add the location of the block to the location list for that block type
                 this.tileLocations[hashCode].Add(this.processingPoint);// new Point(this.processingPoint.X / this.TileSize.Width, this.processingPoint.Y / this.TileSize.Height));
                //this.tileLocations[this.byteData[str]].Add(this.processingPoint); // new Point(this.processingPoint.X / this.TileSize.Width, this.processingPoint.Y / this.TileSize.Height));

                // move to next block location
                this.processingPoint.X += this.tileSizeWidth;
                if (this.processingPoint.X >= this.mapTexture.Width)
                {
                    this.processingPoint.X = 0;
                    this.processingPoint.Y += this.tileSizeHeight;
                }

                // check if it have moved out side the dimensions of the map image and if so proceed to the next step
                if (this.processingPoint.Y >= this.mapTexture.Height)
                {
                    this.currentStep++;
                    break;
                }

                this.processedTiles++;
            }

            // calc percentage completed
            this.percentageCompleted = (float)this.processedTiles / this.totalTiles;

            // }
        }

        public uint GetHashCodeFromByteArray(byte[] data)
        {
            var hash = new HashBuilder();
            return hash.ComputeHash(data);
            //unchecked
            //{
            //    var result = 0;
            //    foreach (byte b in data)
            //    {
            //        result = (result * 31) ^ b;
            //    }

            //    return result;
            //}
        }

        #endregion
    }

    // http://bretm.home.comcast.net/~bretm/hash/7.html
    internal class HashBuilder  
    {
        uint a, b, c;

        void Mix()
        {
            a -= b; a -= c; a ^= (c >> 13);
            b -= c; b -= a; b ^= (a << 8);
            c -= a; c -= b; c ^= (b >> 13);
            a -= b; a -= c; a ^= (c >> 12);
            b -= c; b -= a; b ^= (a << 16);
            c -= a; c -= b; c ^= (b >> 5);
            a -= b; a -= c; a ^= (c >> 3);
            b -= c; b -= a; b ^= (a << 10);
            c -= a; c -= b; c ^= (b >> 15);
        }

        public   uint ComputeHash(byte[] data)
        {
            int len = data.Length;
            a = b = 0x9e3779b9;
            c = 0;
            int i = 0;
            while (i + 12 <= len)
            {
                a += (uint)data[i++] |
                    ((uint)data[i++] << 8) |
                    ((uint)data[i++] << 16) |
                    ((uint)data[i++] << 24);
                b += (uint)data[i++] |
                    ((uint)data[i++] << 8) |
                    ((uint)data[i++] << 16) |
                    ((uint)data[i++] << 24);
                c += (uint)data[i++] |
                    ((uint)data[i++] << 8) |
                    ((uint)data[i++] << 16) |
                    ((uint)data[i++] << 24);
                Mix();
            }
            c += (uint)len;
            if (i < len)
                a += data[i++];
            if (i < len)
                a += (uint)data[i++] << 8;
            if (i < len)
                a += (uint)data[i++] << 16;
            if (i < len)
                a += (uint)data[i++] << 24;
            if (i < len)
                b += (uint)data[i++];
            if (i < len)
                b += (uint)data[i++] << 8;
            if (i < len)
                b += (uint)data[i++] << 16;
            if (i < len)
                b += (uint)data[i++] << 24;
            if (i < len)
                c += (uint)data[i++] << 8;
            if (i < len)
                c += (uint)data[i++] << 16;
            if (i < len)
                c += (uint)data[i++] << 24;
            Mix();
            return c;
        }
    }
}