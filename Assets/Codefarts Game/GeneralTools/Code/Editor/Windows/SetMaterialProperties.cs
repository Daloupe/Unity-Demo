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
    using Codefarts.Localization;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// Provides a window that allows you to select material assets from the project window and set there color or main texture values.
    /// </summary>
    public class SetMaterialProperties : EditorWindow
    {
        /// <summary>
        /// Holds a value indicating whether or not the material color will be set.
        /// </summary>
        private bool setColor;

        /// <summary>
        /// Holds the color that the material will be set to.
        /// </summary>
        private Color color = Color.white;

        /// <summary>
        /// Holds a value indicating whether or not the material's main texture will be set.
        /// </summary>
        private bool setTexture;

        /// <summary>
        /// Holds a reference to a texture asset that will be used to replace the materials main texture reference.
        /// </summary>
        private Texture texture;


        /// <summary>
        /// Called by unity to draw the window controls.
        /// </summary>
        public void OnGUI()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            // color
            GUILayout.BeginHorizontal();
            this.setColor = GUILayout.Toggle(this.setColor, string.Empty, GUILayout.MaxWidth(25));
            this.color = EditorGUILayout.ColorField(local.Get("Color"), this.color);
            GUILayout.EndHorizontal();

            // texture
            GUILayout.BeginHorizontal();
            this.setTexture = GUILayout.Toggle(this.setTexture, string.Empty, GUILayout.MaxWidth(25));
            this.texture = EditorGUILayout.ObjectField(local.Get("Texture"), this.texture, typeof(Texture), false) as Texture;
            GUILayout.EndHorizontal();

            // provide a button to set the material values
            if (GUILayout.Button(local.Get("Set"), GUILayout.Height(30)))
            {
                this.SetProperties();
            }

            // flexible space so the button does not stretch
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// Sets the material values.
        /// </summary>
        private void SetProperties()
        {
            // Get a filtered selection of selected material assets. Will also search sub folders.
            var results = Selection.GetFiltered(typeof(Material), SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.ExcludePrefab);
            if (results == null || results.Length == 0)
            {
                return;
            }

            // get reference to the localization manager
            var local = LocalizationManager.Instance;

            // register an undo for the results
            Undo.RegisterUndo(results, local.Get("SetMaterialProperties"));
            foreach (var item in results)
            {
                // attempt to get the material selection reference
                var mat = item as Material;
                if (mat == null)
                {
                    continue;
                }

                // check if we want to set the color
                if (this.setColor)
                {
                    mat.color = this.color;
                }
                // check if we want to set the main texture
                if (this.setTexture)
                {
                    mat.mainTexture = this.texture;
                }
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Codefarts/General Utilities/Set Material Properties")]
        public static void ShowMassAssetRenameWindow()
        {
            // get the window, show it, and hand it focus
            var local = LocalizationManager.Instance;
            GetWindow<SetMaterialProperties>(local.Get("SetMaterialProperties")).Show();
        }
    }
}