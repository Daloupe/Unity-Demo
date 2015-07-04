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
    using System.Collections.Generic;

    using Codefarts.GeneralTools.Common;
    using Codefarts.GeneralTools.Editor.Utilities;

    using UnityEngine;

    using Color = Codefarts.GeneralTools.Common.Color;
    using Vector2 = Codefarts.GeneralTools.Common.Vector2;

    /// <summary>
    /// provides a control for selecting a tile for freeform rectangle over a preview texture.
    /// </summary>
    public class TileSelectionControl
    {
        /// <summary>
        /// Determines if the tile set starts with spaces.
        /// </summary>
        public bool StartSpacing;

        /// <summary>
        /// Holds the value for spacing between tiles.
        /// </summary>
        public int Spacing = 1;

        /// <summary>
        /// Holds the value for spacing between tiles.
        /// </summary>
        public float Inset = 0.0005f;

        /// <summary>
        /// Holds the value for the tile width.
        /// </summary>
        public int TileWidth = 32;

        /// <summary>
        /// Holds the value for the tile height.
        /// </summary>
        public int TileHeight = 32;

        /// <summary>
        /// Holds a value indicating weather freeform selection is being used.
        /// </summary>
        public bool FreeForm;

        /// <summary>
        /// Stores the location of the first tile location where dragging began.
        /// </summary>
        private Point firstTileLocation;

        /// <summary>
        /// Holds the value of the freeform selection rectangle.
        /// </summary>
        public Rect FreeFormRectangle;

        /// <summary>
        /// Used to determine if the user is dragging a selection rectangle.
        /// </summary>
        private bool isSelecting;

        /// <summary>
        /// Holds a reference to a texture asset.
        /// </summary>
        private Texture2D textureAsset;

        /// <summary>
        /// Holds the scroll values for the main preview.
        /// </summary>
        private UnityEngine.Vector2 mainPreviewScroll;

        /// <summary>
        /// Used to record when the last auto repaint occurred.
        /// </summary>
        private DateTime lastUpdateTime;

        /// <summary>
        /// Holds a list of selected tile locations.
        /// </summary>
        private readonly List<Point> selectedTiles = new List<Point>();

        /// <summary>
        /// Used to hold the current state of the selection.
        /// </summary>
        private TileSelectionStatus selectionStatus;

        private Vector2 min;

        private Vector2 max;

        /// <summary>
        /// Provides an event that gets raised when a different texture is selected.
        /// </summary>
        public event EventHandler TextureChanged;

        /// <summary>
        /// Provides an event that gets raised when tile selection is complete.
        /// </summary>
        public event EventHandler<TileSelectionEventArgs> TileSelection;

        /// <summary>
        /// Provides an event that gets raised when freeform selection is complete.
        /// </summary>
        public event EventHandler<FreeformSelectionEventArgs> FreeformSelection;

        /// <summary>
        /// Provides an event that gets raised when a refresh is requested.
        /// </summary>
        public event EventHandler Refresh;

        /// <summary>
        /// Gets or sets a reference to the texture asset.
        /// </summary>
        public Texture2D TextureAsset
        {
            get
            {
                return this.textureAsset;
            }

            set
            {
                var changed = value != this.textureAsset;
                this.textureAsset = value;
                if (changed && this.TextureChanged != null)
                {
                    this.TextureChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the list of selected tile locations.
        /// </summary>
        public List<Point> SelectedTiles
        {
            get
            {
                return this.selectedTiles;
            }
        }

        /// <summary>
        /// Raises the <see cref="TileSelection" /> event.
        /// </summary>
        public void OnTileSelection()
        {
            if (this.TileSelection != null)
            {
                this.TileSelection(this, new TileSelectionEventArgs { Status = this.selectionStatus, TileLocations = this.selectedTiles, Min = this.min, Max = this.max });
            }
        }

        /// <summary>
        /// Raises the <see cref=" FreeformSelection" /> event.
        /// </summary>
        public void OnFreeformSelection()
        {
            if (this.FreeformSelection != null)
            {
                this.FreeformSelection(this, new FreeformSelectionEventArgs { Status = this.selectionStatus, SelectionRectangle = this.FreeFormRectangle });
            }
        }

        /// <summary>
        /// Raises the <see cref="Refresh" /> event.
        /// </summary>
        public void OnRefresh()
        {
            if (this.Refresh != null)
            {
                this.Refresh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Is used to draw the control.
        /// </summary>
        public void Draw()
        {
            // setup a scroll view so the user can scroll large textures
            this.mainPreviewScroll = GUILayout.BeginScrollView(this.mainPreviewScroll, true, true);

            // check if a texture asset is available
            if (this.textureAsset != null)
            {
                // we draw a label here with the same dimensions of the texture so scrolling will work
                var tex = this.textureAsset;
                GUILayout.Label(string.Empty, GUILayout.Width(tex.width), GUILayout.Height(tex.height));

                // draw the preview texture
                GUI.DrawTexture(
                    new Rect(4, 4, tex.width, tex.height),
                    tex,
                    ScaleMode.StretchToFill,
                    true,
                    1);

                // draw the selection rectangles
                this.DrawSelection();
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw the selection rectangle in the main preview.
        /// </summary>
        private void DrawSelection()
        {
            // draw selection rectangle depending on freeform state
            if (this.FreeForm)
            {
                // freeform state is true so allow the user to select a rectangle anywhere on the texture
                this.DrawFreeformSelection();
            }
            else
            {
                // freeform selection state if false so act like we are selecting from a grid of tiles
                this.DrawTileSelection();
            }

            // get mouse position
            var input = Event.current;

            // only repaint every so often to save cpu cycles
            if (DateTime.Now > this.lastUpdateTime + TimeSpan.FromMilliseconds(25))
            {
                // record the time that we are repainting
                this.lastUpdateTime = DateTime.Now;
                this.OnRefresh();
            }

            // check if the user has stopped selecting tiles
            if (input.isMouse && input.type == EventType.MouseUp && input.button == 0)
            {
                this.isSelecting = false;
            }
        }

        /// <summary>
        /// Used to draw the current freeform selection rectangle.
        /// </summary>
        private void DrawFreeformSelection()
        {
            // get mouse position
            var input = Event.current;
            var mousePosition = new Point((int)input.mousePosition.x, (int)input.mousePosition.y);// -(Vector2.One * 4);

            // determine if user is selecting  
            if (this.isSelecting && input.type == EventType.MouseDrag && input.button == 0)
            {
                // this event has been used
                input.Use();

                // clear any previous selection
                this.selectedTiles.Clear();

                // calc the min and max positions based on the location of the mouse and the first position of mouse down
                var min = new Vector2(Math.Min(this.firstTileLocation.X, mousePosition.X), Math.Min(this.firstTileLocation.Y, mousePosition.Y));
                var max = new Vector2(Math.Max(this.firstTileLocation.X, mousePosition.X), Math.Max(this.firstTileLocation.Y, mousePosition.Y));

                // set the freeform rectangle
                this.FreeFormRectangle.x = min.X;
                this.FreeFormRectangle.y = min.Y;
                this.FreeFormRectangle.xMax = max.X;
                this.FreeFormRectangle.yMax = max.Y;

                // raise the selection changed event
                this.selectionStatus = TileSelectionStatus.Selecting;
                this.OnFreeformSelection();
            }

            // draw the selected freeform rectangle
            Helpers.DrawRect(this.FreeFormRectangle, Color.Red);

            // check if were beginning to select tiles with the left mouse button
            if (!this.isSelecting && input.isMouse && input.type == EventType.MouseDown && input.button == 0)
            {
                this.isSelecting = true;
                this.firstTileLocation = mousePosition;

                // clear any previously selected tiles and add the first tile location
                this.selectedTiles.Clear();
                this.selectedTiles.Add(this.firstTileLocation);

                // raise the selection changed event
                this.selectionStatus = TileSelectionStatus.Begin;
                this.OnFreeformSelection();

                // this event has been used
                input.Use();
            }

            // check if were releasing left mouse button (IE stop selecting)
            if (this.isSelecting && input.isMouse && input.type == EventType.MouseUp && input.button == 0)
            {

                // this event has been used
                input.Use();

                this.isSelecting = false;
                this.selectionStatus = TileSelectionStatus.Complete;
                this.OnFreeformSelection();
            }
        }

        /// <summary>
        /// Used to draw the currently selected tiles.
        /// </summary>
        private void DrawTileSelection()
        {
            // get mouse position
            var input = Event.current;

            // used to store the top left tile coordinates of the tile position
            Point tilePos;

            // determine if the tile set starts with spacing
            var startOffset = this.StartSpacing ? this.Spacing : 0;

            // convert the mouse position into a tile position
            var mousePos = input.mousePosition;// -(Vector2.One * 4);
            tilePos.X = (int)((mousePos.x - startOffset) / (this.TileWidth + this.Spacing));
            tilePos.Y = (int)((mousePos.y - startOffset) / (this.TileHeight + this.Spacing));

            // calculate the min and max positions based on the location of the mouse and the first selected tile location
            this.min = new Vector2(Math.Min(this.firstTileLocation.X, tilePos.X), Math.Min(this.firstTileLocation.Y, tilePos.Y));
            this.max = new Vector2(Math.Max(this.firstTileLocation.X, tilePos.X), Math.Max(this.firstTileLocation.Y, tilePos.Y));

            // cap the min max values to the dimensions of the texture
            this.min = this.CapValues(this.min);
            this.max = this.CapValues(this.max) + new Point(1, 1);

            // determine if user is selecting  
            if (this.isSelecting && input.type == EventType.MouseDrag && input.button == 0)
            {
                // this event has been used
                input.Use();

                // clear any previous selection
                this.selectedTiles.Clear();

                // add tile entries for the selection
                for (var idx = this.min.X; idx < this.max.X; idx++)
                {
                    for (var idy = this.min.Y; idy < this.max.Y; idy++)
                    {
                        this.selectedTiles.Add(new Vector2(idx, idy));
                    }
                }

                // save last mouse position
                this.selectionStatus = TileSelectionStatus.Selecting;
                this.OnTileSelection();
            }

            // draw selected tiles
            float x;
            float y;
            foreach (var tile in this.selectedTiles)
            {
                // calculate rectangle Top Left for tile location
                x = startOffset + (tile.X * (this.TileWidth + this.Spacing));
                y = startOffset + (tile.Y * (this.TileHeight + this.Spacing));

                // draw the selected tile rectangle
                Helpers.DrawRect(new Rect(x, y, this.TileWidth + this.Spacing - startOffset, this.TileHeight + this.Spacing - startOffset), Color.Red);
            }

            // draw blue rectangle indicating what tile the mouse is hovering over
            x = startOffset + (tilePos.X * (this.TileWidth + this.Spacing));
            y = startOffset + (tilePos.Y * (this.TileHeight + this.Spacing));
            Helpers.DrawRect(new Rect(x, y, this.TileWidth + this.Spacing - startOffset, this.TileHeight + this.Spacing - startOffset), Color.Blue);

            // check if were beginning to select tiles with the left mouse button
            if (!this.isSelecting && input.isMouse && input.type == EventType.MouseDown && input.button == 0)
            {
                // this event has been used
                input.Use();

                this.isSelecting = true;
                this.firstTileLocation = tilePos;

                // clear any previously selected tiles and add the first tile location
                this.selectedTiles.Clear();
                this.selectedTiles.Add(this.firstTileLocation);

                // save last mouse position
                this.selectionStatus = TileSelectionStatus.Begin;
                this.OnTileSelection();
            }

            // check if were releasing left mouse button (IE stop selecting)
            if (this.isSelecting && input.isMouse && input.type == EventType.MouseUp && input.button == 0)
            {
                // this event has been used
                input.Use();

                this.isSelecting = false;
                this.selectionStatus = TileSelectionStatus.Complete;
                this.OnTileSelection();
            }
        }

        /// <summary>
        /// Restricts the values of a <see cref="Point"/> to the dimensions of the selected texture.
        /// </summary>
        /// <param name="value">The value to be restricted.</param>
        /// <returns>Returns the restricted <see cref="Point"/> value.</returns>
        private Point CapValues(Point value)
        {
            // prevent values less then 0
            value.X = value.X < 0 ? 0 : value.X;
            value.Y = value.Y < 0 ? 0 : value.Y;

            // prevent values greater then the texture dimensions
            value.X = value.X > (this.textureAsset.width / this.TileWidth) - 1 ? (this.textureAsset.width / this.TileWidth) - 1 : value.X;
            value.Y = value.Y > (this.textureAsset.height / this.TileHeight) - 1 ? (this.textureAsset.height / this.TileHeight) - 1 : value.Y;

            return value;
        }
    }
}