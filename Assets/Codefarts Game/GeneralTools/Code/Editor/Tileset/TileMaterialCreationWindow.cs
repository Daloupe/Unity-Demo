/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.TileSets
{
    using System;
    using System.IO;
    using System.Linq;

    using Codefarts.CoreProjectCode;
    using Codefarts.CoreProjectCode.Services;
    using Codefarts.CoreProjectCode.Settings;
    using Codefarts.GeneralTools.Editor;

    using Codefarts.GeneralTools.GenericImage;
    using Codefarts.GeneralTools.GenericImage.Unity;

    using Codefarts.GeneralTools.Editor.Controls;
    using Codefarts.Localization;              

    using UnityEditor;

    using UnityEngine;

    using Color = UnityEngine.Color;
    using Vector2 = UnityEngine.Vector2;

    /// <summary>
    /// Provides a window for selecting tiles from a tile set and creating materials from them
    /// </summary>
    [InitializeOnLoad]
    public class TileMaterialCreationWindow : EditorWindow
    {
        /// <summary>
        /// Holds a predefined list of orientation names.
        /// </summary>
        private string[] orientationNames;

        /// <summary>
        /// Stores the index into the scaleValues field.
        /// </summary>
        private int previewScaleIndex;

        /// <summary>
        /// Holds an array of scale values.
        /// </summary>
        private readonly string[] scaleValues = { "1", "2", "3", "4", "5", "6" };

        /// <summary>
        /// Holds the scroll values for the selection preview.
        /// </summary>
        private Vector2 selectionPreviewScroll;

        /// <summary>
        /// Used to store the material name.
        /// </summary>
        private string textName = string.Empty;

        /// <summary>
        /// Used to store the names of labels.
        /// </summary>
        private string textLabels = string.Empty;

        /// <summary>
        /// Used to determine weather the error text should be drawn.
        /// </summary>
        private bool showErrorText;

        /// <summary>
        /// Used to store what error message to report.
        /// </summary>
        private string errorText = string.Empty;
       
        /// <summary>
        /// Holds the index selection for the orientation popup.
        /// </summary>
        private int orientationIndex;

        /// <summary>
        /// Holds a reference to a temporary texture used to draw the preview.
        /// </summary>
        private Texture2D previewTexture;

        /// <summary>
        /// Holds a value indicating whether the p[review texture needs to be updated.
        /// </summary>
        private bool previewNeedsUpdate;

        /// <summary>
        /// Used to hold a temporary reference to a readable texture.
        /// </summary>
        private GenericImage readableTexture;

        /// <summary>
        /// Provides a common reusable control.
        /// </summary>
        private GenericMaterialCreationControl materialControls;

        /// <summary>
        /// Holds a reference to the controls that handles selecting tiles.
        /// </summary>
        private TileSelectionControl mainPreview;

        /// <summary>
        /// Holds a reference to an object responcible for selecting an output folder.
        /// </summary>
        private SelectOutputFolderControl selectOutputFolderControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMaterialCreationWindow"/> class.
        /// </summary>
        public TileMaterialCreationWindow()
        {
            this.selectOutputFolderControl=new SelectOutputFolderControl();

            // register for callback to set localization names
            EditorCallbackService.Instance.Register(this.LoadLocalizeStrings);

            // localize strings
            //   this.LoadLocalizeStrings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileMaterialCreationWindow"/> class.
        /// </summary>
        static TileMaterialCreationWindow()
        {
            EditorCallbackService.Instance.Register(() =>
                {
                    var settings = UnityPreferencesManager.Instance;
                    var local = LocalizationManager.Instance;

                    // register tile material creation settings
                    settings.Register(local.Get("SETT_TileMaterialCreation"), TileMaterialCreationGeneralMenuItem.Draw);
                });
        }

        /// <summary>
        /// Is called when the window need to be re-drawn.
        /// </summary>
// ReSharper disable UnusedMember.Local
        private void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            try
            {
                GUILayout.BeginHorizontal();

                // draw the main preview 
                this.mainPreview.Draw();

                // draw the side controls
                this.DrawSideControls();

                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
            }
        }

        /// <summary>
        /// OnDestroy is called when the EditorWindow is closed.
        /// </summary>
        private void OnDestroy()
        {
            // dispose of the material controls  and clean up variables
            if (this.materialControls != null)
            {
                this.materialControls.Dispose();
            }

            this.materialControls = null;
            this.mainPreview = null;
        }

        /// <summary>
        /// handles when the window is enabled.
        /// </summary>
// ReSharper disable UnusedMember.Local
        private void OnEnable()
// ReSharper restore UnusedMember.Local
        {
            // ensure destroy was called first (sometimes when compiling scripts unity does not call this)
            this.OnDestroy();

            // create a control that will handle tile selection
            this.mainPreview = new TileSelectionControl();

            // update the preview texture after tile selection occurs
            this.mainPreview.TileSelection += (s, e) =>
                  {
                      this.UpdateTilePreviewTextureSize();
                      this.UpdateTilePreviewTexture();
                      this.previewNeedsUpdate = true;
                      this.UpdatePreview();
                  };

            // update the preview texture after freeform selection occurs
            this.mainPreview.FreeformSelection += (s, e) =>
              {
                  this.UpdateFreeFormPreviewTextureSize();
                  this.UpdateFreeFormPreviewTexture();
                  this.previewNeedsUpdate = true;
                  this.UpdatePreview();
              };

            // create a control that will handle drawing the general material creation ui
            this.materialControls = new GenericMaterialCreationControl();

            // when a texture selection change occurs update main preview texture and be sure to update the selection preview 
            this.materialControls.TextureChanged += (s, e) =>
            {
                this.mainPreview.TextureAsset = this.materialControls.TextureAsset;
                this.SetupSelectedTextureAsset(((GenericMaterialCreationControl)s).TextureAsset);
                this.previewNeedsUpdate = true;
                this.UpdatePreview();
            };

            // ensure preview texture size is updated when tile size changes
            this.materialControls.TileSizeChanged += (s, e) =>
                {
                    this.mainPreview.TileHeight = this.materialControls.TileHeight;
                    this.mainPreview.TileWidth = this.materialControls.TileWidth;
                    this.previewNeedsUpdate = true;
                    this.UpdatePreview();
                };

            // ensure preview texture is updated when starts with spacing changes
            this.materialControls.StartSpacingChanged += (s, e) =>
                {
                    this.mainPreview.StartSpacing = this.materialControls.StartSpacing;
                    this.previewNeedsUpdate = true;
                    this.UpdatePreview();
                };

            // ensure preview texture is updated when spacing changes
            this.materialControls.SpacingChanged += (s, e) =>
                {
                    this.mainPreview.Spacing = this.materialControls.Spacing;
                    this.previewNeedsUpdate = true;
                    this.UpdatePreview();
                };

            // ensure preview texture is updated when freeform selection changes changes
            this.materialControls.FreeformChanged += (s, e) =>
                {
                    this.mainPreview.FreeForm = this.materialControls.FreeForm;
                    this.previewNeedsUpdate = true;
                    this.UpdatePreview();
                };

            // when main preview refresh event occurs be sure to repaint the window.
            this.mainPreview.Refresh += (s, e) => this.Repaint();

            // sync variable values between the controls
            this.mainPreview.StartSpacing = this.materialControls.StartSpacing;
            this.mainPreview.Spacing = this.materialControls.Spacing;
            this.mainPreview.FreeForm = this.materialControls.FreeForm;

            // create initial empty preview texture
            this.previewTexture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
            this.SetupSelectedTextureAsset(this.materialControls.TextureAsset);

            // load available shaders for shader popup list
            var settings = SettingsManager.Instance;
            this.materialControls.ShaderNames.Clear();

            // setup a default shader array to use if unable to retrieve the setting
            var defaultShaderList = new[] { "Diffuse", "Transparent/Diffuse", "Transparent/Cutout/Diffuse", "Transparent/Cutout/Soft Edge Unlit" };

            // try to get the shader array from settings
            var strings = settings.GetSetting(GlobalConstants.TileMaterialCreationShadersKey, string.Join("\r\n", defaultShaderList));

            // set the shaders that will be available in the shader popup
            this.materialControls.ShaderNames.AddRange(strings.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim()));
        }

        /// <summary>
        /// Used to localize the orientation names and other strings.
        /// </summary>
        private void LoadLocalizeStrings()
        {
            // set localized strings for various controls
            var local = LocalizationManager.Instance;
            this.orientationNames = new string[] { local.Get("None"), local.Get("FlipVertical"), local.Get("FlipHorizontal"), local.Get("FlipBoth") };
            this.title = local.Get("TileMaterials");
        }     

        /// <summary>
        /// Draws controls on the side for creating materials.
        /// </summary>
        private void DrawSideControls()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            GUILayout.BeginVertical(GUILayout.Width(200), GUILayout.MinWidth(200));

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // draw the materials control
            this.materialControls.Draw();

            // place some spacing of a few pixels
            GUILayout.Space(4);

              // setup output folder display
            this.selectOutputFolderControl.Draw( );

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // provide a text field for specifying labels
            GUILayout.Label(local.Get("LabelsToApply"));
            this.textLabels = GUILayout.TextField(this.textLabels);

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // provide a text field for specifying a material name
            GUILayout.Label(local.Get("MaterialName"));
            this.textName = GUILayout.TextField(this.textName);

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // provide a rotation field for specifying a rotation name
            GUILayout.Label(local.Get("Orientation"));
            var index = EditorGUILayout.Popup(this.orientationIndex, this.orientationNames);
            if (index != this.orientationIndex)
            {
                this.orientationIndex = index;
                this.previewNeedsUpdate = true;
                this.UpdatePreview();
            }

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // provide a create button
            if (GUILayout.Button(local.Get("Create")))
            {
                this.CreateNewMaterial();
            }

            // place some spacing of a few pixels
            GUILayout.Space(4);

            // if there was an error draw it here
            if (this.showErrorText)
            {
                GUILayout.Label(this.errorText, "ErrorLabel");
            }

            // draw a small selection preview so the user can see what they selected
            this.DrawSelectionPreview();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Updates the <see cref="previewTexture"/> variable.
        /// </summary>
        private void UpdatePreview()
        {
            if (this.materialControls.FreeForm)
            {
                this.UpdateFreeFormPreviewTextureSize();
                this.UpdateFreeFormPreviewTexture();
            }
            else
            {
                this.UpdateTilePreviewTextureSize();
                this.UpdateTilePreviewTexture();
            }
        }

        /// <summary>
        /// Used to create a new material using the provided information from the user
        /// </summary>
        private void CreateNewMaterial()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            // trim material name and labels
            this.textName = this.textName.Trim();
            this.textLabels = this.textLabels.Trim();

            // make sure we don't display any error. We assume up front that there will not be any
            this.showErrorText = false;

            // if no material name specified just exit
            if (string.IsNullOrEmpty(this.textName))
            {
                // report error
                this.showErrorText = true;
                this.errorText = local.Get("ERR_NoMaterialName");
                return;
            }

            // check if a texture has been specified and if not just exit
            if (this.materialControls.TextureAsset == null)
            {
                // report error
                this.showErrorText = true;
                this.errorText = local.Get("ERR_NoTextureSelected");
                return;
            }

            // attempt to create a a new material 
            Material material;
            try
            {
                material = new Material(this.materialControls.Material);
            }
            catch (Exception ex)
            {
                // report error
                this.showErrorText = true;
                this.errorText = ex.Message;
                return;
            }

            // get texture co-ordinates for the material
            var coords = this.mainPreview.GetTextureCoords(this.materialControls, this.orientationIndex);
            material.mainTextureOffset = new Vector2(coords.xMin, coords.yMin);
            material.mainTextureScale = new Vector2(coords.width, coords.height);
            material.mainTexture = this.materialControls.TextureAsset;

            // attempt to construct a file path
            string file;
            try
            {
                // try to build a file path
                file = Path.Combine(this.selectOutputFolderControl.OutputPath, this.textName);
                file = Path.Combine("Assets/", file);

                // include the *.mat file extension
                file = Path.ChangeExtension(file, ".mat");
            }
            catch (Exception ex)
            {
                // report error
                this.showErrorText = true;
                this.errorText = ex.Message;
                return;
            }

            // attempt to split any labels by spaces
            var labelParts =
                this.textLabels.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Where(x => !string.IsNullOrEmpty(x.Trim()))
                    .ToArray();

            // check if the file exists already and if the file exists ask the user if they want to override the file or cancel    
            if (File.Exists(file))
            {
                switch (EditorUtility.DisplayDialogComplex(local.Get("Warning"), local.Get("ERR_AMaterialWithNameExists"), local.Get("Override"), local.Get("Update"), local.Get("Cancel")))
                {
                    case 0:
                        // create the material asset and override any file that may already exist
                        this.CreateMaterialFile(material, file);
                        break;

                    case 1:
                        // create the material asset and override any file that may already exist
                        var sourceMaterial = AssetDatabase.LoadAssetAtPath(file, typeof(Material)) as Material;
                        if (sourceMaterial == null)
                        {
                            // report error
                            this.showErrorText = true;
                            this.errorText = local.Get("ERR_ExistingMaterialCouldotLoad");
                            return;
                        }

                        // update properties
                        sourceMaterial.shader = material.shader;
                        sourceMaterial.mainTextureScale = material.mainTextureScale;
                        sourceMaterial.mainTextureOffset = material.mainTextureOffset;
                        sourceMaterial.mainTexture = material.mainTexture;

                        // reference the source mat
                        material = sourceMaterial;
                        break;

                    case 2:
                        // do nothing
                        return;
                }
            }
            else
            {
                // create the material asset and override any file that may already exist
                this.CreateMaterialFile(material, file);
            }

            // if labels were specified then set the labels
            if (labelParts.Any())
            {
                AssetDatabase.SetLabels(material, labelParts);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a material file.
        /// </summary>
        /// <param name="mat">A reference to the material to save.</param>
        /// <param name="file">The filename of the material.</param>
        private void CreateMaterialFile(Material mat, string file)
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            // get directory
            var directory = Path.GetDirectoryName(file);

            // check if directory was found
            if (directory == null)
            {
                // report error
                this.showErrorText = true;
                this.errorText = local.Get("ERR_NoOutputDirectory");
                return;
            }

            // check if directory exists and if not create it
            if (!Directory.Exists(directory))
            {
                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (Exception ex)
                {
                    // report error
                    this.showErrorText = true;
                    this.errorText = local.Get("ERR_CouldNotCreateDirectory");
                    Debug.LogException(ex);
                    return;
                }
            }

            try
            {
                // create the material asset and override any file that may already exist
                AssetDatabase.CreateAsset(mat, file);
            }
            catch (Exception ex)
            {
                // report error
                this.showErrorText = true;
                this.errorText = ex.Message;
            }
        }

        /// <summary>
        /// Draw the selection preview showing what the user has selected.
        /// </summary>
        private void DrawSelectionPreview()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            GUILayout.BeginHorizontal();

            // draw a label indicating that this is a preview
            GUILayout.Label(local.Get("Preview"));

            GUILayout.FlexibleSpace();

            // draw scale values selector
            this.previewScaleIndex = EditorGUILayout.Popup(this.previewScaleIndex, this.scaleValues);

            GUILayout.EndHorizontal();

            // if there is no texture asset or selection we can just exit
            if (this.materialControls.TextureAsset == null)
            {
                return;
            }

            // wrap preview in a scroll view in case preview selection is larger
            this.selectionPreviewScroll = EditorGUILayout.BeginScrollView(this.selectionPreviewScroll, true, true);

            // ensure texture references are available
            if (this.materialControls.TextureAsset != null && this.previewTexture != null)
            {
                var rect = this.mainPreview.FreeFormRectangle;
                rect.x = 0;
                rect.y = 0;
                if (!this.mainPreview.FreeForm)
                {
                    // get the selection size and tile spacing
                    var size = this.mainPreview.GetSelectionSize();
                    var spacing = this.mainPreview.Spacing;

                    // setup a rectangle the same size as the selection
                    rect = new Rect(0, 0, (size.X * (this.materialControls.TileWidth + spacing)) - spacing, (size.Y * (this.mainPreview.TileHeight + spacing)) - spacing);
                }

                try
                {
                    // scale output rectangle by the selected scale value index plus one
                    rect.width *= this.previewScaleIndex + 1;
                    rect.height *= this.previewScaleIndex + 1;

                    // draw texture with material applied
                    EditorGUI.DrawPreviewTexture(rect, this.previewTexture, this.materialControls.Material, ScaleMode.ScaleToFit);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Updates the preview texture data.
        /// </summary>
        private void UpdateTilePreviewTexture()
        {
            // if there is no texture asset we can just exit
            if (this.materialControls.TextureAsset == null || this.readableTexture == null)
            {
                return;
            }

            // check if no tiles selected or does not need update
            if (this.mainPreview.SelectedTiles.Count == 0 || !this.previewNeedsUpdate)
            {
                return;
            }

            // get first tile rect
            var pos = this.mainPreview.SelectedTiles[0];

            // get the selection size
            var size = this.mainPreview.GetSelectionSize();

            var tileWidth = this.materialControls.TileWidth;
            var tileHeight = this.materialControls.TileHeight;
            var spacing = this.materialControls.Spacing;
            //   var inset = this.materialControls.Inset;
            var offset = this.materialControls.StartSpacing ? spacing : 0;

            // setup a rectangle the same size as the selection
            var rect = new Rect(
                (pos.X * (tileWidth + spacing)) - offset,
                (pos.Y * (tileHeight + spacing)) - offset,
                (size.X * (tileWidth + spacing)) - spacing,
                (size.Y * (tileHeight + spacing)) - spacing);

            // rect.Inflate(-inset, -inset);
            // draw the pixels onto the preview texture
            var horiz = this.orientationIndex == 2 || this.orientationIndex == 3;
            var vert = this.orientationIndex == 1 || this.orientationIndex == 3;
            this.previewTexture.Draw(this.readableTexture, rect, Vector2.zero, horiz, !vert);

            // preview no longer need an update
            this.previewNeedsUpdate = false;
        }

        /// <summary>
        /// Updates the preview texture data.
        /// </summary>
        private void UpdateFreeFormPreviewTexture()
        {
            // if there is no texture asset we can just exit
            if (this.materialControls.TextureAsset == null || this.readableTexture == null)
            {
                return;
            }

            // check if no freeform rectangle selected or does not need update
            var freeformRect = this.mainPreview.FreeFormRectangle;
            if (!this.previewNeedsUpdate || freeformRect.width < 1 || freeformRect.height < 1)
            {
                return;
            }

            // draw the pixels onto the preview texture
            var horiz = this.orientationIndex == 2 || this.orientationIndex == 3;
            var vert = this.orientationIndex == 1 || this.orientationIndex == 3;
            this.previewTexture.Draw(this.readableTexture, freeformRect, Vector2.zero, horiz, !vert);

            // preview no longer need an update
            this.previewNeedsUpdate = false;
        }

        /// <summary>
        /// Clones a texture into a readable texture asset and converts it into a <see cref="GenericImage"/> type. Then remove the cloned texture.
        /// </summary>
        /// <param name="sourceTexture">The non readable texture typically selected from the asset browser window that will have a
        /// temporary readable copy of it self made then promptly destroyed.</param>
        private void SetupSelectedTextureAsset(Texture2D sourceTexture)
        {
            // if no texture provided just exit
            if (sourceTexture == null)
            {
                return;
            }

            // get the file path to the source texture file
            var file = AssetDatabase.GetAssetPath(sourceTexture);

            // if no file returned just exit
            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            // get the directory that the file is in
            var filePath = Path.GetDirectoryName(file);

            // construct a temp filename for the new readable texture file
            var tempFile = Path.Combine(filePath, Path.GetFileNameWithoutExtension(file) + "_NewFileNameTempPewpzIRReadableTex");
            tempFile = Path.ChangeExtension(tempFile, Path.GetExtension(file));

            try
            {
                // if the temp filename already exists delete it
                if (File.Exists(tempFile))
                {
                    AssetDatabase.DeleteAsset(tempFile);
                }

                // make a copy of the file source texture file
                File.Copy(file, tempFile);

                // import the new temp asset texture
                AssetDatabase.ImportAsset(tempFile, ImportAssetOptions.Default);

                // attempt to load the new temporary texture asset
                var tmpTex = AssetDatabase.LoadAssetAtPath(tempFile, typeof(Texture2D)) as Texture2D;

                // attempt to convert the readable texture into a generic image type
                this.readableTexture = tmpTex.ToGenericImage();

                // destroy the temp texture reference
                UnityEngine.Object.DestroyImmediate(tmpTex, true);
            }
            catch (Exception)
            {
                // catch any errors that may have occurred during processing
                try
                {
                    // try to ensure that the temp texture asset is removed
                    AssetDatabase.DeleteAsset(tempFile);
                }
                catch
                {
                }
            }

            try
            {
                // if the temp texture file exists attempt to delete it
                if (File.Exists(tempFile))
                {
                    // try to ensure that the temp texture asset is removed
                    AssetDatabase.DeleteAsset(tempFile);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Updates the size of the preview texture if needed.
        /// </summary>
        private void UpdateTilePreviewTextureSize()
        {
            var tileWidth = this.materialControls.TileWidth;
            var tileHeight = this.materialControls.TileHeight;
            var spacing = this.materialControls.Spacing;

            // if there is no texture asset we can just exit
            if (this.materialControls.TextureAsset == null)
            {
                return;
            }

            // get selection size and calculate needed preview size
            var size = this.mainPreview.GetSelectionSize();
            var neededPreviewWidth = (int)(size.X * (tileWidth + spacing)) - spacing;
            var neededPreviewHeight = (int)(size.Y * (tileHeight + spacing)) - spacing;

            // check if no preview texture or the tile size is different then the preview texture dimensions 
            if (this.previewTexture == null || this.previewTexture.width != neededPreviewWidth || this.previewTexture.height != neededPreviewHeight)
            {
                // recreate the preview texture
                this.previewTexture = new Texture2D(neededPreviewWidth, neededPreviewHeight, TextureFormat.ARGB32, false);

                // the size has changed so update the pixel data
                this.UpdateTilePreviewTexture();
            }
        }

        /// <summary>
        /// Updates the size of the preview texture if needed.
        /// </summary>
        private void UpdateFreeFormPreviewTextureSize()
        {
            // if there is no texture asset we can just exit
            if (this.materialControls.TextureAsset == null)
            {
                return;
            }

            // get the freeform rectangle
            var freeformRect = this.mainPreview.FreeFormRectangle;

            // if the rectangle is too small just exit
            if (freeformRect.width < 1 || freeformRect.height < 1)
            {
                return;
            }

            // check if no preview texture or the freeform rectangle size is different then the preview texture dimensions 
            if (this.previewTexture == null || this.previewTexture.width != freeformRect.width || this.previewTexture.height != freeformRect.height)
            {
                // recreate the preview texture
                this.previewTexture = new Texture2D((int)freeformRect.width, (int)freeformRect.height, TextureFormat.ARGB32, false);

                // the size has changed so update the pixel data
                this.UpdateFreeFormPreviewTexture();
            }
        }

        /// <summary>
        /// Used to initialize the window.
        /// </summary>
        [MenuItem("Codefarts/Tile Mapping Utilities/Tile Material Creation")]
        private static void ShowWindow()
        {
            // get the window, show it, and hand it focus
            try
            {
                // create the window and show it
                var window = GetWindow<TileMaterialCreationWindow>();
                window.Show();
                window.Focus();
                window.Repaint();

                // setup the window default states from settings
                var settings = SettingsManager.Instance;
                window.materialControls.TileWidth = settings.GetSetting(GlobalConstants.TileMaterialCreationDefaultWidthKey, 32);
                window.materialControls.TileHeight = settings.GetSetting(GlobalConstants.TileMaterialCreationDefaultHeightKey, 32);
                window.selectOutputFolderControl.ShowAsList = settings.GetSetting(GlobalConstants.TileMaterialCreationAsListKey, false);
                window.materialControls.FreeForm = settings.GetSetting(GlobalConstants.TileMaterialCreationFreeformKey, false);
                window.materialControls.MaterialColor = settings.GetSetting(GlobalConstants.DefaultTileMaterialCreationColorKey, Color.white);
            }
            catch (Exception ex)
            {
                // log error in the console if something went wrong
                Debug.LogError(ex.Message);
            }
        }
    }
}