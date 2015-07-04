/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.Controls
{
    using System;

    using UnityEngine;
    using Vector2 = Codefarts.GeneralTools.Common.Vector2;

    public static class TileSelectionControlExtensionMethods
    {
        /// <summary>
        /// Calculates the texture co-ordinates for the selection.
        /// </summary>
        /// <returns>Returns the textures co-ordinates for the selection in UV space.</returns>
        public static Rect GetTextureCoords(this TileSelectionControl preview, GenericMaterialCreationControl material, int orientationIndex)
        {
            var startOffset = material.StartSpacing ? material.Spacing : 0;

            var tileWidth = material.TileWidth;
            var tileHeight = material.TileHeight;
            var spacing = material.Spacing;
            var textureAsset = material.TextureAsset;
            var inset = material.Inset;
            var texCoords = new Rect();

            // if in free form mode return the freeform selection rectangle
            if (material.FreeForm)
            {
                var src = preview.FreeFormRectangle;
                texCoords.xMin = src.xMin / preview.TextureAsset.width;
                texCoords.yMin = src.yMin / preview.TextureAsset.height;
                texCoords.width = src.width / preview.TextureAsset.width;
                texCoords.height = src.height / preview.TextureAsset.height;
            }
            else
            {
                // calc selection rectangle size
                var size = preview.GetSelectionSize();

                // get min X/Y position of the selection rectangle
                var minX = Single.MaxValue;
                var minY = Single.MaxValue;
                foreach (var tile in preview.SelectedTiles)
                {
                    if (tile.X < minX)
                    {
                        minX = tile.X;
                    }

                    if (tile.Y < minY)
                    {
                        minY = tile.Y;
                    }
                }

                // setup a rect containing the US co-ordinates
                texCoords = new Rect(
                    (startOffset + (minX * (tileWidth + spacing))) / textureAsset.width,
                    (startOffset + (minY * (tileHeight + spacing))) / textureAsset.height,
                    ((size.X * (tileWidth + spacing)) - spacing - startOffset) / textureAsset.width,
                    ((size.Y * (tileHeight + spacing)) - spacing - startOffset) / textureAsset.height);

                // apply inset
                texCoords.x += inset;
                texCoords.y += inset;
                texCoords.width -= inset * 2;
                texCoords.height -= inset * 2;
            }

            // textures co-ordinates originate from the lower left corner of the texture so adjust y to accommodate
            texCoords.y = 1 - (texCoords.y / 1) - texCoords.height;

            // check to flip vertically
            if (orientationIndex == 1 || orientationIndex == 3)
            {
                var tmp = texCoords.yMin;
                texCoords.yMin = texCoords.yMax;
                texCoords.yMax = tmp;
            }

            // check to flip horizontally
            if (orientationIndex == 2 || orientationIndex == 3)
            {
                var tmp = texCoords.xMin;
                texCoords.xMin = texCoords.xMax;
                texCoords.xMax = tmp;
            }

            return texCoords;
        }

        /// <summary>
        /// Calculates the size of the selection rectangle in tile co-ordinates.
        /// </summary>
        /// <returns>Returns the size of the selection rectangle in tile co-ordinates as a <see cref="Vector2"/> type.</returns>
        public static Vector2 GetSelectionSize(this TileSelectionControl preview)
        {
            // calculate the min and max values of the selection rectangle
            var minX = Int32.MaxValue;
            var maxX = Int32.MinValue;
            var minY = Int32.MaxValue;
            var maxY = Int32.MinValue;
            foreach (var tile in preview.SelectedTiles)
            {
                if (tile.X < minX)
                {
                    minX = tile.X;
                }

                if (tile.X > maxX)
                {
                    maxX = tile.X;
                }

                if (tile.Y < minY)
                {
                    minY = tile.Y;
                }

                if (tile.Y > maxY)
                {
                    maxY = tile.Y;
                }
            }

            // store the width and height
            var selectionWidth = maxX - minX + 1;
            var selectionHeight = maxY - minY + 1;

            // return the size
            return new Vector2(selectionWidth, selectionHeight);
        }
    }
}
