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

    using Codefarts.Localization;

    using UnityEditor;

    using UnityEngine;

    using Object = UnityEngine.Object;

    public class GenericMaterialCreationControl : IDisposable
    {
        /// <summary>
        /// Holds the value of the materials color.
        /// </summary>
        private Color materialColor;

        /// <summary>
        /// Gets or sets the value of the materials color.
        /// </summary>
        public Color MaterialColor
        {
            get
            {
                return this.materialColor;
            }
            set
            {
                if (this.materialColor == value)
                {
                    return;
                }

                this.materialColor = value;

                if (this.material != null)
                {
                    this.material.color = this.materialColor;
                }

                if (this.MaterialColorChanged != null)
                {
                    this.MaterialColorChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Holds a reference to a material reference.
        /// </summary>
        private Material material;

        /// <summary>
        /// Gets a reference to the material.
        /// </summary>
        public Material Material
        {
            get
            {
                return this.material;
            }
        }

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
        /// Determines if the tile set starts with spaces.
        /// </summary>
        private bool startSpacing;

        /// <summary>
        /// Gets or sets if the tile set starts with spaces.
        /// </summary>
        public bool StartSpacing
        {
            get
            {
                return this.startSpacing;
            }
            set
            {
                if (this.startSpacing == value)
                {
                    return;
                }

                this.startSpacing = value;

                if (this.StartSpacingChanged != null)
                {
                    this.StartSpacingChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Provides an event that gets raised when <see cref="MaterialColor" /> changes.
        /// </summary>
        public event EventHandler MaterialColorChanged;

        /// <summary>
        /// Provides an event that gets raised when <see cref="StartSpacing" /> changes.
        /// </summary>
        public event EventHandler StartSpacingChanged;

        /// <summary>
        /// Provides an event that gets raised when <see cref="Spacing" /> changes.
        /// </summary>
        public event EventHandler SpacingChanged;

        /// <summary>
        /// Provides an event that gets raised when <see cref="FreeForm" /> changes.
        /// </summary>
        public event EventHandler FreeformChanged;

        /// <summary>
        /// Provides an event that gets raised when shader selection index changes.
        /// </summary>
        public event EventHandler ShaderChanged;

        /// <summary>
        /// Holds the value for spacing between tiles.
        /// </summary>
        private int spacing = 1;

        /// <summary>
        /// Gets or sets the value for spacing between tiles.
        /// </summary>
        public int Spacing
        {
            get
            {
                return this.spacing;
            }
            set
            {
                if (this.spacing == value)
                {
                    return;
                }

                this.spacing = value;

                if (this.SpacingChanged != null)
                {
                    this.SpacingChanged(this, EventArgs.Empty);
                }
            }
        }

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
        private bool freeForm;

        /// <summary>
        /// Gets or sets a value indicating weather freeform selection is being used.
        /// </summary>
        public bool FreeForm
        {
            get
            {
                return this.freeForm;
            }
            set
            {
                // if ShowFreeform is false or the values match just exit
                if (!this.showFreeform || this.freeForm == value)
                {
                    return;
                }

                this.freeForm = value;

                if (this.FreeformChanged != null)
                {
                    this.FreeformChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Holds a reference to a texture asset.
        /// </summary>
        private Texture2D textureAsset;

        /// <summary>
        /// Provides an event that gets raised when <see cref="TextureAsset" />  is selected or changes.
        /// </summary>
        public event EventHandler TextureChanged;

        /// <summary>
        /// Provides an event that gets raised when the tile size fields change.
        /// </summary>
        public event EventHandler TileSizeChanged;

        /// <summary>
        /// Holds the selection index for the specified shader.
        /// </summary>
        private int shaderSelectionIndex;

        /// <summary>
        /// Holds a pre-defined list of shader names.
        /// </summary>
        private readonly List<string> shaderNames;

        /// <summary>
        /// Gets or sets a value indicating whether or not to show the <see cref="freeForm"/> checkbox.
        /// </summary>
        public bool ShowFreeform
        {
            get
            {
                return this.showFreeform;
            }
            set
            {
                this.showFreeform = value;

                // if false ensure that the freeform checkbox is unchecked
                if (!value)
                {
                    this.FreeForm = false;
                }
            }
        }

        /// <summary>
        /// Holds a value indicating whether the object has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Holds a Boolean value indicating whether the freeform checkbox will be shown.
        /// </summary>
        private bool showFreeform = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMaterialCreationControl"/> class.
        /// </summary>
        public GenericMaterialCreationControl()
        {
            // NOTE: Not sure if this will work if running under a different culture (Brazil & Asian regions etc) it should but hmmm...
            // wondering if localizing these strings is wise or unwise ...
            this.shaderNames = new List<string>(new[] { "Diffuse", "Transparent/Diffuse" });

            this.material = new Material(Shader.Find(this.shaderNames[this.shaderSelectionIndex]));
            this.materialColor = Color.white;
            this.material.color = this.materialColor;
        }

        public List<string> ShaderNames
        {
            get
            {
                return this.shaderNames;
            }
        }

        public string SelectedShaderName
        {
            get
            {
                return this.shaderNames[this.shaderSelectionIndex];
            }
        }

        public void Draw()
        {
            if (this.disposed)
            {
                return;
            }

            // get reference to localization manager
            var local = LocalizationManager.Instance;

            GUILayout.BeginVertical();

            // draw tile size controls
            GUILayout.BeginHorizontal();
            GUILayout.Label(local.Get("Width"));
            var width = EditorGUILayout.IntField(this.TileWidth);
            GUILayout.Label(local.Get("Height"));
            var height = EditorGUILayout.IntField(this.TileHeight);
            GUILayout.EndHorizontal();

            // clamp tile dimensions
            width = width < 1 ? 1 : width;
            height = height < 1 ? 1 : height;
            width = width > 4096 ? 4096 : width;
            height = height > 4096 ? 4096 : height;

            if (width != this.TileWidth || height != this.TileHeight)
            {
                this.TileWidth = width;
                this.TileHeight = height;
                if (this.TileSizeChanged != null)
                {
                    this.TileSizeChanged(this, EventArgs.Empty);
                }
            }

            // draw a texture selection control
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label(local.Get("Color"));
            // have to wrap this in try/catch cause unity will throw exceptions in the console.
            // I suspect it may have something to do with the nested BeginHorizontal BeginVertical
            this.MaterialColor = EditorGUILayout.ColorField(this.materialColor);

            // draw control for specifying if starts with spacing 
            this.StartSpacing = GUILayout.Toggle(this.startSpacing, local.Get("StartsWithSpacing"));
            if (this.ShowFreeform)
            {
                this.FreeForm = GUILayout.Toggle(this.freeForm, local.Get("Freeform"));
            }

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(local.Get("Texture"));
            
            // have to wrap this in try/catch cause unity will throw exceptions in the console.
            // I suspect it may have something to do with the nested BeginHorizontal BeginVertical
            try
            {
                this.TextureAsset = EditorGUILayout.ObjectField(this.TextureAsset, typeof(Texture2D), false, GUILayout.Width(64), GUILayout.Height(64)) as Texture2D;
            }
            catch (Exception)
            {
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            // draw control for specifying spacing
            var value = EditorGUILayout.IntField(local.Get("Spacing"), this.spacing, GUILayout.MaxWidth(200));
            value = value < 0 ? 0 : value;
            this.Spacing = value;

            // draw control for specifying inset
            this.Inset = EditorGUILayout.FloatField(local.Get("Inset"), this.Inset, GUILayout.MaxWidth(200));
            this.Inset = this.Inset < 0 ? 0 : this.Inset;

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // draw a popup that allows for shader type selection
            GUILayout.Label(local.Get("ShaderType"));
            this.shaderSelectionIndex = this.shaderSelectionIndex > this.shaderNames.Count - 1
                                             ? this.shaderNames.Count - 1
                                             : this.shaderSelectionIndex;
            value = EditorGUILayout.Popup(this.shaderSelectionIndex, this.shaderNames.ToArray());
            if (value != this.shaderSelectionIndex)
            {
                this.shaderSelectionIndex = value;

                this.material.shader = Shader.Find(this.shaderNames[this.shaderSelectionIndex]);
                this.material.color = this.materialColor;

                if (this.ShaderChanged != null)
                {
                    this.ShaderChanged(this, EventArgs.Empty);
                }
            }

            GUILayout.EndVertical();
        }

        public void Dispose()
        {
            // create the material for drawing the preview tile
            if (this.material != null)
            {
                Object.DestroyImmediate(this.material);
                this.material = null;
            }

            this.disposed = true;
        }
    }
}