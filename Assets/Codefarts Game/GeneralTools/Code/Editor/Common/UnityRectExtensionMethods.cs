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
    using System;

    using UnityEngine;

    public static class UnityRectExtensionMethods
    {
        /// <summary>
        /// Returns a third Rect structure that represents the intersection of two other Rect structures. 
        /// If there is no intersection, an empty Rect is returned.
        /// </summary>
        /// <param name="a">
        /// A rectangle to intersect.   
        /// </param>
        /// <param name="b">
        /// B rectangle to intersect.  
        /// </param>
        /// <returns>
        /// A Rect that represents the intersection of a and b.
        /// </returns>
        public static Rect Intersect(this Rect a, Rect b)
        {
            float x = Math.Max((sbyte)a.x, (sbyte)b.x);
            float num2 = Math.Min(a.x + a.width, b.x + b.width);
            float y = Math.Max((sbyte)a.y, (sbyte)b.y);
            float num4 = Math.Min(a.y + a.height, b.y + b.height);
            if ((num2 >= x) && (num4 >= y))
            {
                return new Rect(x, y, num2 - x, num4 - y);
            }

            return new Rect();
        }

        /// <summary>
        /// Determines if this rectangle intersects with rect.
        /// </summary>
        /// <param name="rect">
        /// The rectangle to test.
        /// </param>
        /// <returns>
        /// This method returns true if there is any intersection, otherwise false.
        /// </returns>
        public static bool Intersects(this Rect src, Rect rect)
        {
            return !((src.x > rect.xMax) || (src.xMax< rect.x) || (src.y > rect.yMax) || (src.yMax< rect.y));
        }
    }
}