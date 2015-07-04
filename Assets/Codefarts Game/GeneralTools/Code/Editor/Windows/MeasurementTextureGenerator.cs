/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/

namespace Codefarts.GeneralTools.Editor.Windows
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Codefarts.GeneralTools.Editor;
    using Codefarts.GeneralTools.Editor.Controls;
    using Codefarts.GeneralTools.Editor.Utilities;
    using Codefarts.GeneralTools.GenericImage;
    using Codefarts.GeneralTools.GenericImage.Unity;

    using UnityEditor;

    using UnityEngine;

    public class MeasurementTextureGenerator : EditorWindow
    {
        public class PreviewItem
        {
            public Texture2D Texture;

            public GenericImage Image;

            public string Name;

            public Vector2 Position;
        }

        private List<PreviewItem> previewItems = new List<PreviewItem>();
        int selectedSize = 512;
        bool chkMipMaps;
        Color fillColor = Color.red;
        Color borderColor = Color.white;
        Color dimensionTextColor = Color.white;
        Color textColor = Color.white;
        Color arrowColor = Color.white;
        Vector2 pnlPreview;
        string txtNames = string.Empty;
        bool chkDimensions;
        bool chkArrows;
        private int currentImage = -1;
        private Font dimensionFont;
        private Font cbLabelFont;
        private int labelFontSize = 8;
        private int dimensionFontSize = 8;
        private int borderSize;
        private int cbType;
        private Color gridColor = Color.white;
        private bool chkGrid;
        private int gridSizeX = 32;
        private int gridSizeY = 32;
        private Color angleColor = Color.white;
        private float startAngle;
        private int angleCount;
        private float angleStep = 15;
        private Color angleTextColor = Color.white;
        private bool chkAngleText;
        private bool chkAngleSymbol;
        private int angleTextOffset;
        private int angleTextDistance = 64;
        private int angleOriginX;
        private int angleOriginY;
        private int angleDistance = 5000;
        private Font cbAngleFont;
        private int angleFontSize = 8;
        private int vecArrowXOffset;
        private int vecArrowYOffset;
        private int vecDimensionXOffset;
        private int vecDimensionYOffset;

        private SelectOutputFolderControl outputFolder;

        private Vector2 scroll;

        private bool expander;

        private readonly int[] sizeItems = new[] { 32, 64, 128, 256, 512, 1024, 2048, 4096 };

        private readonly string[] displayedSizeOptions = new[] { "32", "64", "128", "256", "512", "1024", "2048", "4096" };

        public MeasurementTextureGenerator()
        {
            this.outputFolder = new SelectOutputFolderControl();
        }

        // ReSharper disable UnusedMember.Local
        private void OnGUI()
        // ReSharper restore UnusedMember.Local
        {
            this.scroll = GUILayout.BeginScrollView(this.scroll, GUILayout.MaxHeight(1108));
            GUILayout.BeginVertical();

            this.outputFolder.Draw();

            GUILayout.Label("Type");
            this.cbType = EditorGUILayout.Popup(this.cbType, new[] { "Square", "Circle" });

            GUILayout.Label("Size");
            this.selectedSize = EditorGUILayout.IntPopup(this.selectedSize, this.displayedSizeOptions, this.sizeItems);

            this.dimensionFont = EditorGUILayout.ObjectField("Dimension Font", this.dimensionFont, typeof(Font), false) as Font;
            this.cbLabelFont = EditorGUILayout.ObjectField("Label Font", this.cbLabelFont, typeof(Font), false) as Font;
            this.cbAngleFont = EditorGUILayout.ObjectField("Angle Font", this.cbAngleFont, typeof(Font), false) as Font;

            this.dimensionFontSize = EditorGUILayout.IntField("Dimension Font Size", this.dimensionFontSize);
            this.dimensionFontSize = this.dimensionFontSize < 1 ? 1 : this.dimensionFontSize;
            this.dimensionFontSize = this.dimensionFontSize > 200 ? 200 : this.dimensionFontSize;

            this.angleFontSize = EditorGUILayout.IntField("Angle Font Size", this.angleFontSize);
            this.angleFontSize = this.angleFontSize < 1 ? 1 : this.angleFontSize;
            this.angleFontSize = this.angleFontSize > 200 ? 200 : this.angleFontSize;

            this.labelFontSize = EditorGUILayout.IntField("Label Font Size", this.labelFontSize);
            this.labelFontSize = this.labelFontSize < 1 ? 1 : this.labelFontSize;
            this.labelFontSize = this.labelFontSize > 200 ? 200 : this.labelFontSize;

            this.borderSize = EditorGUILayout.IntField("Border Size", this.borderSize);
            this.borderSize = this.borderSize < 1 ? 1 : this.borderSize;
            this.borderSize = this.borderSize > 200 ? 200 : this.borderSize;

            // create texture color controls
            GUILayout.BeginVertical();

            // mip maps
            this.chkMipMaps = GUILayout.Toggle(this.chkMipMaps, "Mipmaps");

            // dimension text
            this.chkDimensions = GUILayout.Toggle(this.chkDimensions, "Dimensions");

            // draw arrows?
            this.chkArrows = GUILayout.Toggle(this.chkArrows, "Arrows");

            // draw angle text?
            this.chkAngleText = GUILayout.Toggle(this.chkAngleText, "Angle Text");

            // draw angle symbol?
            this.chkAngleSymbol = GUILayout.Toggle(this.chkAngleSymbol, "Angle Degree Symbol");

            GUILayout.EndVertical();

            // grid size
            this.chkGrid = GUILayout.Toggle(this.chkGrid, "Grid");

            //colors
            // pnlStack = new StackPanel() { Orientation = Orientation.Vertical, Height = 125, MaxHeight = 125 };
            this.expander = EditorGUILayout.Foldout(this.expander, "Colors");
            if (this.expander)
            {
                this.fillColor = EditorGUILayout.ColorField("Fill Color", this.fillColor);
                this.borderColor = EditorGUILayout.ColorField("Border Color", this.borderColor);
                this.textColor = EditorGUILayout.ColorField("Text Color", this.textColor);
                this.dimensionTextColor = EditorGUILayout.ColorField("Dimension Text Color", this.dimensionTextColor);
                this.arrowColor = EditorGUILayout.ColorField("Arrow Color", this.arrowColor);
                this.gridColor = EditorGUILayout.ColorField("Grid Color", this.gridColor);
                this.angleColor = EditorGUILayout.ColorField("Angle Color", this.angleColor);
                this.angleTextColor = EditorGUILayout.ColorField("Angle Text Color", this.angleTextColor);
            }

            // grid size label
            GUILayout.Label("Grid Size");

            // grid size
            GUILayout.BeginHorizontal();
            GUILayout.Label("X:");
            this.gridSizeX = EditorGUILayout.IntField(this.gridSizeX);
            this.gridSizeX = this.gridSizeX < 1 ? 1 : this.gridSizeX;
            GUILayout.Label("Y:");
            this.gridSizeY = EditorGUILayout.IntField(this.gridSizeY);
            this.gridSizeY = this.gridSizeY < 1 ? 1 : this.gridSizeY;
            GUILayout.EndHorizontal();

            // arrow offset label
            GUILayout.Label("Arrow Offset");

            // arrow offset
            GUILayout.BeginHorizontal();
            GUILayout.Label("X:");
            this.vecArrowXOffset = EditorGUILayout.IntField(this.vecArrowXOffset);
            GUILayout.Label("Y:");
            this.vecArrowYOffset = EditorGUILayout.IntField(this.vecArrowYOffset);
            GUILayout.EndHorizontal();

            // dimension offset label
            GUILayout.Label("Dimensions Offset");

            // dimension offset
            GUILayout.BeginHorizontal();
            GUILayout.Label("X:");
            this.vecDimensionXOffset = EditorGUILayout.IntField(this.vecDimensionXOffset);
            GUILayout.Label("Y:");
            this.vecDimensionYOffset = EditorGUILayout.IntField(this.vecDimensionYOffset);
            GUILayout.EndHorizontal();

            // angle list
            this.angleCount = EditorGUILayout.IntField("Angle Count", this.angleCount);
            this.angleCount = this.angleCount < 0 ? 0 : this.angleCount;
            this.angleCount = this.angleCount > 359 ? 359 : this.angleCount;

            this.angleStep = EditorGUILayout.FloatField("Angle Step", this.angleStep);
            this.angleStep = this.angleStep < 1 ? 1 : this.angleStep;
            this.angleStep = this.angleStep > 359 ? 359 : this.angleStep;

            this.startAngle = EditorGUILayout.FloatField("Start Angle", this.startAngle);
            this.startAngle = this.startAngle < -359 ? -359 : this.startAngle;
            this.startAngle = this.startAngle > 359 ? 359 : this.startAngle;

            this.angleTextOffset = EditorGUILayout.IntField("Angle Text Offset", this.angleTextOffset);
            this.angleTextOffset = this.angleTextOffset < -359 ? -359 : this.angleTextOffset;
            this.angleTextOffset = this.angleTextOffset > 359 ? 359 : this.angleTextOffset;

            this.angleDistance = EditorGUILayout.IntField("Angle Distance", this.angleDistance);
            this.angleDistance = this.angleDistance < 0 ? 0 : this.angleDistance;
            this.angleDistance = this.angleDistance > 5000 ? 5000 : this.angleDistance;

            this.angleTextDistance = EditorGUILayout.IntField("Angle Text Distance", this.angleTextDistance);
            this.angleTextDistance = this.angleTextDistance < 0 ? 0 : this.angleTextDistance;
            this.angleTextDistance = this.angleTextDistance > 5000 ? 5000 : this.angleTextDistance;

            // angle offset label
            GUILayout.Label("Angle Origin");

            // angle offset
            GUILayout.BeginHorizontal();
            GUILayout.Label("X:");
            this.angleOriginX = EditorGUILayout.IntField(this.angleOriginX);
            GUILayout.Label("Y:");
            this.angleOriginY = EditorGUILayout.IntField(this.angleOriginY);
            GUILayout.EndHorizontal();

            // create texture names text box
            GUILayout.BeginVertical();
            GUILayout.Label("Labels");

            this.txtNames = GUILayout.TextArea(this.txtNames);
            GUILayout.EndVertical();

            // add generate button
            if (GUILayout.Button("Preview"))
            {
                this.GenerateTextures();
                Selection.objects = this.previewItems.Select(x => x.Texture).ToArray();
            }

            // add generate button
            if (GUILayout.Button("Generate"))
            {
                this.GenerateClick();
            }

            // preview panel for textures
            this.pnlPreview = GUILayout.BeginScrollView(this.pnlPreview);
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private GenericImage GenerateTexture(string name)
        {
            var size = this.selectedSize;
            var bmp = new GenericImage(size, size);

            // Color32 col;
            switch (this.cbType)
            {
                case 0: //"Square":
                    // fill
                    bmp.FillRectangle(0, 0, bmp.Width, bmp.Height, this.fillColor);

                    // border
                    for (var i = 0; i < this.borderSize; i++)
                    {
                        bmp.DrawRectangle(i, i, size - (i * 2), size - (i * 2), this.borderColor);
                    }

                    break;

                case 1: //"Circle":
                    // fill
                    bmp.FillEllipse(0, 0, size, size, this.fillColor);

                    // border
                    for (var i = 0; i < this.borderSize; i++)
                    {
                        bmp.DrawEllipse(i, i, size - (i * 2), size - (i * 2), this.borderColor);
                    }

                    break;
            }

            // grid
            if (this.chkGrid)
            {
                for (var i = this.gridSizeX; i < bmp.Width; i += this.gridSizeX)
                {
                    bmp.DrawLine(i, 0, i, size, this.gridColor);
                }

                for (var i = this.gridSizeY; i < bmp.Height; i += this.gridSizeY)
                {
                    bmp.DrawLine(0, i, size, i, this.gridColor);
                }
            }

            // angles
            //  col = this.angleColor;
            //  pen = new Pen(dColor.FromArgb(col.a, col.r, col.g, col.b));
            //  col = this.angleTextColor;
            //  brush = new SolidBrush(dColor.FromArgb(col.a, col.r, col.g, col.b));
            var angle = this.startAngle;
            for (var i = 0; i < this.angleCount; i++)
            {
                var pos = new Vector2(this.angleOriginX, this.angleOriginY);
                var endPos = new Vector2((float)Math.Cos(Math.PI / 180 * angle), (float)Math.Sin(Math.PI / 180 * angle));
                endPos.Normalize();
                endPos *= this.angleDistance;
                endPos += pos;
                bmp.DrawLine((int)pos.x, (int)pos.y, (int)endPos.x, (int)endPos.y, this.angleColor);

                if (this.chkAngleText && this.cbAngleFont)
                {
                    // draw angle text

                    //    var fnt = new Font((string)this.cbAngleFont.Items[this.cbAngleFont.SelectedIndex], this.angleFontSize.Value);
                    //  var text = (angle + this.angleTextOffset).ToString() + (this.chkAngleSymbol ? "°" : string.Empty);


                    //endPos = new Vector2((float)Math.Cos(Math.PI / 180 * angle), (float)Math.Sin(Math.PI / 180 * angle));
                    //endPos.Normalize();
                    //endPos *= this.angleTextDistance;
                    //endPos += pos;

                    // bmp.DrawString(text, fnt, brush, endPos.x - (txtSize.Width / 2), endPos.y - (txtSize.Height / 2));
                }

                angle += this.angleStep;
            }


            if (this.chkDimensions && this.dimensionFont)
            {
                // Dimensions
                //   font = new Font((string)this.dimensionFont.Items[this.dimensionFont.SelectedIndex], this.dimensionFontSize.Value);
                //  var dimentionString = string.Format("{0}x{0}", size);

                // var dimentionStringSize = gfx.MeasureString(dimentionString, font);
                // col = this.dimensionTextColor;
                //  brush = new SolidBrush(dColor.FromArgb(col.a, col.r, col.g, col.b));
                //   bmp.DrawString(dimentionString, font, brush, borderSize + 4 + this.vecDimensionXOffset.Value, borderSize + 4 + this.vecDimensionYOffset.Value);
            }

            // label
            if (this.cbLabelFont)
            {
                //  font = new Font((string)this.cbLabelFont.Items[this.cbLabelFont.SelectedIndex], this.labelFontSize.Value);
               // var labelStringSize = this.cbLabelFont.MeasureString(name, this.labelFontSize, FontStyle.Normal);
                //var color = this.textColor;
                // var solidBrush = new SolidBrush(dColor.FromArgb(color.a, color.r, color.g, color.b));
              //  bmp.DrawString(this.cbLabelFont, name, size / 2f - labelStringSize.width / 2, size / 2f - labelStringSize.height / 2);
            }

            // orientation arrows
            if (this.chkArrows)
            {
                //  color = this.arrowColor;
                // pen = new Pen(dColor.FromArgb(color.a, color.r, color.g, color.b));
                // pen.EndCap = LineCap.ArrowAnchor;
                if (this.vecArrowXOffset != 0 | this.vecArrowYOffset != 0)
                {
                    bmp.DrawLine(this.vecArrowXOffset, this.vecArrowYOffset, this.vecArrowXOffset, this.vecArrowYOffset + (size / 4), this.arrowColor);
                    bmp.DrawLine(this.vecArrowXOffset, this.vecArrowYOffset, this.vecArrowXOffset + (size / 4), this.vecArrowYOffset, this.arrowColor);
                }
                else
                {
                    bmp.DrawLine(this.borderSize + 4, this.borderSize + 4 + this.dimensionFontSize, this.borderSize + 4, this.borderSize + 4 + this.dimensionFontSize + (size / 4), this.arrowColor);
                    bmp.DrawLine(this.borderSize + 4, this.borderSize + 4 + this.dimensionFontSize, this.borderSize + 4 + (size / 4), this.borderSize + 4 + this.dimensionFontSize, this.arrowColor);
                }
            }

            return bmp;
        }

        private void GenerateClick()
        {
            var outputPath = this.outputFolder.OutputPath;
            outputPath = outputPath.Trim();
            if (string.IsNullOrEmpty(outputPath))
            {
                Debug.LogError("No output folder specified!");
            }

            this.GenerateTextures();

            // save to output folder
            foreach (var item in this.previewItems)
            {
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                var path = Path.Combine(Path.Combine("Assets", outputPath), string.IsNullOrEmpty(item.Name) ? "Empty" : item.Name);
                path = Path.ChangeExtension(path, ".png");

                File.WriteAllBytes(path, item.Texture.EncodeToPNG());
                AssetDatabase.ImportAsset(path, ImportAssetOptions.Default);
                Debug.Log(string.Format("Saved texture file {0}", path));
            }
        }

        private void GenerateTextures()
        {
            this.previewItems.Clear();
            var lines = this.txtNames.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                lines = new[] { string.Empty };
            }

            var size = this.selectedSize;

            for (var index = 0; index < lines.Length; index++)
            {
                var image = new PreviewItem();
                this.previewItems.Add(image);
                image.Position = new Vector2(4, index * (size + 4));
            }

            this.currentImage = 0;

            foreach (var image in this.previewItems)
            {
                var label = lines[this.currentImage];
                image.Image = this.GenerateTexture(label.Replace(@"\n", "\n"));
                image.Texture = image.Image.ToTexture2D();
                image.Name = label.Trim();
                this.currentImage++;
            }
        }

        [MenuItem("Codefarts/General Utilities/Texture Generator")]
        public static void ShowTextureGenerator()
        {
            var win = GetWindow<MeasurementTextureGenerator>();
            win.title = "Texture Generator";
            win.Show();
            win.Focus();
        }
    }
}