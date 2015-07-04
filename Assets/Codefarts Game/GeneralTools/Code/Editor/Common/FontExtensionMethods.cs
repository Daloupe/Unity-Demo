namespace Codefarts.GeneralTools.Common
{
    using System;
    using System.Collections.Generic;

    using UnityEngine;

    public static class FontExtensionMethods
    {
        public static Rect MeasureString(this Font font, string text, int size, FontStyle style)
        {
            if (font == null)
            {
                throw new ArgumentNullException("font");
            }

            // get info for each unique char in string 
            var info = new Dictionary<char, CharacterInfo>();
            var lineHeight = 0f;
            var totalHeight = 0f;
            var lineWidth = 0f;
            var maxWidth = 0f;
            foreach (var c in text)
            {
                if (!info.ContainsKey(c))
                {
                    CharacterInfo ci;
                    if (!font.GetCharacterInfo(c, out ci, size, style))
                    {
                        continue;
                    }

                    info.Add(c, ci);
                    lineHeight = Math.Max(lineHeight, ci.size);
                    if (c == '\n')
                    {
                        totalHeight += lineHeight;
                        lineHeight = 0;
                        maxWidth = Math.Max(maxWidth, lineWidth);
                        lineWidth = 0;
                    }
                    else
                    {
                        lineWidth += ci.width;
                    }
                }
            }

            return new Rect(0, 0, maxWidth, totalHeight);
        }
    }
}
