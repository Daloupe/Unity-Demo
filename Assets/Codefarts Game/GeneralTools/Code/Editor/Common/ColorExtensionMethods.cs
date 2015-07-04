/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Common
{
    /// <summary>
    /// Extension methods for the <see cref="Color"/> type.
    /// </summary>
    public static class ColorExtensionMethods
    {
        #region Public Methods and Operators

        /// <summary>
        /// Blends two colors together.
        /// </summary>
        /// <param name="color">
        /// The source color.
        /// </param>
        /// <param name="src">
        /// The blend color.
        /// </param>
        /// <returns>
        /// Returns the result as a <see cref="Color"/> type.
        /// </returns>
        /// <remarks>
        /// From Wikipedia https://en.wikipedia.org/wiki/Alpha_compositing -> "Alpha blending" 
        /// </remarks>
        public static Color Blend(this Color color, Color src)
        {
            float sr = src.R / 255f;
            float sg = src.G / 255f;
            float sb = src.B / 255f;
            float sa = src.A / 255f;
            float dr = color.R / 255f;
            float dg = color.G / 255f;
            float db = color.B / 255f;
            float da = color.A / 255f;

            float oa = sa + (da * (1 - sa));
            float r = ((sr * sa) + ((dr * da) * (1 - sa))) / oa;
            float g = ((sg * sa) + ((dg * da) * (1 - sa))) / oa;
            float b = ((sb * sa) + ((db * da) * (1 - sa))) / oa;
            float a = oa;

            return new Color((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
        }

        #endregion
    }
}