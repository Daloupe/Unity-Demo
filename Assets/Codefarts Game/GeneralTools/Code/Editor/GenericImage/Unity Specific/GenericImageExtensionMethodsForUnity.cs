/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.GenericImage.Unity
{
    using System;

    using Codefarts.GeneralTools.Common;
    using Codefarts.GeneralTools.GenericImage;

    using UnityEngine;

    using Vector2 = UnityEngine.Vector2;

    /// <summary>
    /// <see cref="GenericImage"/> extension methods for unity.
    /// </summary>
    public static class GenericImageExtensionMethodsForUnity
    {
        /// <summary>
        /// Converts a <see cref="GenericImage"/> to a <see cref="Texture2D"/> type.
        /// </summary>
        /// <param name="image">The image to be converted.</param>
        /// <returns>Returns a new <see cref="Texture2D"/> type.</returns>   
        public static Texture2D ToTexture2D(this GenericImage image)
        {
            var texture = new Texture2D(image.Width, image.Height, TextureFormat.ARGB32, false);
            var flippedImage = image.Clone();
            flippedImage.FlipVertically();
            texture.SetPixels32(flippedImage.ToUnityColor32Array());
            texture.Apply();
            return texture;
        }

        public static void DrawString(this GenericImage image, Font font, string text, float x, float y)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            throw new NotImplementedException();
            //Debug.Log(string.Format("font material is null {0}", font.material == null));
            //Debug.Log(string.Format("font material texture is null {0}", font.material.mainTexture == null));
            //Debug.Log(string.Format("font asset path {0}", AssetDatabase.GetAssetPath(font)));
            //Debug.Log(string.Format("font texture asset path {0}", AssetDatabase.GetAssetPath(font.material.mainTexture)));
            //Debug.Log(string.Format("font texture is 2d {0}", font.material.mainTexture is Texture2D));

          //  var tex = font.material.mainTexture as Texture2D;

            // Convert Alpha8 texture to ARGB32 texture so it can be saved as a PNG
            
            //var newTex = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
            // newTex.PackTextures(new[] { tex }, 0);
            // Debug.Log(string.Format("font texture name {0}", newTex.name));

            // var newTex =  Object.Instantiate(tex) as Texture2D;
          // Debug.Log(string.Format("newTex is null {0}", newTex == null));
           // AssetDatabase.CreateAsset(tex, "Assets/test.png");
            //System.IO.File.WriteAllBytes("Assets/test.png", newTex.EncodeToPNG());

        }

        /// <summary>
        /// Converts a <see cref="Texture2D"/> to a <see cref="GenericImage"/> type.
        /// </summary>
        /// <param name="image">The image to be converted.</param>
        /// <returns>Returns a new <see cref="GenericImage"/> type.</returns>
        /// <remarks>The source Texture2D image must be readable.</remarks>
        public static GenericImage ToGenericImage(this Texture2D image)
        {
            var colors = image.GetPixels32(0);
            var texture = new GenericImage(image.width, image.height);
            var index = 0;
            for (var y = texture.Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < texture.Width; x++)
                {
                    texture[x, y] = colors[index++];
                }
            }

            return texture;
        }

        /// <summary>
        /// Converts a <see cref="GenericImage"/> to an array of <see cref="Color32"/>.
        /// </summary>
        /// <param name="image">The image to be converted.</param>
        /// <returns>Returns a new array of <see cref="Color32"/>.</returns>   
        public static Color32[] ToUnityColor32Array(this GenericImage image)
        {
            var destination = new Color32[image.Width * image.Height];
            var index = 0;
            for (var y = 0; y < image.Height; y++)
            {
                for (var x = 0; x < image.Width; x++)
                {
                    destination[index++] = image[x, y];
                }
            }

            return destination;
        }

        /// <summary>
        /// Converts a <see cref="GenericImage"/> to an array of <see cref="UnityEngine.Color"/>.
        /// </summary>
        /// <param name="image">The image to be converted.</param>
        /// <returns>Returns a new array of <see cref="UnityEngine.Color"/>.</returns>   
        public static UnityEngine.Color[] ToUnityColorArray(this GenericImage image)
        {
            var dest = new UnityEngine.Color[image.Width * image.Height];
            var pos = 0;
            for (int idy = 0; idy < image.Height; idy++)
            {
                for (int idx = 0; idx < image.Width; idx++)
                {
                    //  var color = image[idx, idy];
                    dest[pos++] = image[idx, idy];// new UnityEngine.Color(color.R / byte.MaxValue, color.G / byte.MaxValue, color.B / byte.MaxValue, color.A / byte.MaxValue);
                }
            }

            return dest;
        }
        public static void Draw(this Texture2D texture, GenericImage src, Rect srcRect, Vector2 pos)
        {
            Draw(texture, src, (int)pos.x, (int)pos.y, (int)srcRect.x, (int)srcRect.y, (int)srcRect.width, (int)srcRect.height, false, false);
        }

        public static void Draw(this Texture2D texture, GenericImage src, Rect srcRect, Vector2 pos, bool flipHorizontally, bool flipVertically)
        {
            Draw(texture, src, (int)pos.x, (int)pos.y, (int)srcRect.x, (int)srcRect.y, (int)srcRect.width, (int)srcRect.height, flipHorizontally, flipVertically);
        }

        public static void Draw(this Texture2D texture, GenericImage src, int x, int y, int srcX, int srcY, int srcWidth, int srcHeight, bool flipHorizontally, bool flipVertically)
        {
            //if(srcWidth==0)
            //{
            //    Debug.Log("w zero");
            //}
            //if (srcHeight == 0)
            //{
            //    Debug.Log("h zero");
            //}
            //srcWidth = srcWidth == 0 ? 1 : srcWidth;
            //srcHeight = srcHeight == 0 ? 1 : srcHeight;
            var texRect = new Rect(0, 0, texture.width, texture.height);
            var srcRect = new Rect(x, y, srcWidth, srcHeight);
            var intersect = texRect.Intersect(srcRect);
            //  Debug.Log(string.Format("intersection: {0}", intersect));

            if (!intersect.Intersects(new Rect(0, 0, src.Width, src.Height)))
            {
                //      Debug.Log("does not intersect");

                return;
            }
            //intersect = srcRect;

            //  Debug.Log(string.Format("intersect {0}", intersect));
            //  Debug.Log("it intersect");

            // var tmpimg = new GenericImage(texture.width, texture.height);
            //tmpimg.Draw(src, 0, 0, srcX, srcY, srcWidth, srcHeight);
            var tmpimg = new GenericImage((int)intersect.width, (int)intersect.height);
            tmpimg.Draw(src, 0, 0, srcX, srcY, tmpimg.Width, tmpimg.Height);

            if (flipHorizontally)
            {
                tmpimg.FlipHorizontally();
            }

            if (flipVertically)
            {
                tmpimg.FlipVertically();
            }

            var colors = tmpimg.ToUnityColorArray();
            // texture.SetPixels(0, 0, texture.width, texture.height, colors);
            // texture.SetPixels(x, y, srcWidth, srcHeight, colors);
            texture.SetPixels(x, y, (int)intersect.width, (int)intersect.height, colors);
            texture.Apply();
        }
    }
}
