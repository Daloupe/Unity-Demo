namespace Codefarts.GeneralTools.Editor.Windows
{
    using System.Globalization;
    using System.Linq;

    using Codefarts.GeneralTools.Editor.Controls;
    using Codefarts.GeneralTools.Editor.Models;

    using UnityEditor;
    using UnityEngine;
    using Codefarts.GeneralTools.Common;

    public class SceneAssetSaver : EditorWindow
    {
        public enum SearchType
        {
            Materials,
        }

        private readonly Table<AssetModel> table;

        public SearchType SearchForType { get; set; }

        private readonly SelectOutputFolderControl outputFolder;

        private string message;

        public SceneAssetSaver()
        {
            this.table = new Table<AssetModel> { RowHeight = 20 };
            this.outputFolder = new SelectOutputFolderControl();
        }

        /// <summary>
        /// Called by unity to draw the window.
        /// </summary>
        public void OnGUI()
        {
            this.CleanTableModel();

            GUILayout.BeginVertical();

            this.outputFolder.Draw();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Search"))
            {
                this.DoSearch();
            }

            GUILayout.Label("Search for");
            var value = (SearchType)EditorGUILayout.EnumPopup(this.SearchForType);
            if (value != this.SearchForType)
            {
                this.SearchForType = value;
                this.table.Model = null;
            }

            if (string.IsNullOrEmpty(this.message))
            {
                GUILayout.Label(this.message, "ErrorLabel");
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            this.table.Draw();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Removes any entries that are no longer available.
        /// </summary>
        private void CleanTableModel()
        {
            if (this.table.Model == null)
            {
                return;
            }

            var model = (AssetTableModel)this.table.Model;
            if (model.Elements == null)
            {
                return;
            }

            var index = 0;
            while (index < model.Elements.Count - 1)
            {
                var item = model.Elements[index];
                var obj = EditorUtility.InstanceIDToObject(item.ID);
                if (obj == null)
                {
                    model.Elements.RemoveAt(index);
                }
                else
                {
                    try
                    {
                        if (item.GameObject.transform != null)
                        {
                            index++;
                        }
                    }
                    catch
                    {
                        model.Elements.RemoveAt(index);
                    }
                }
            }
        }

        /// <summary>
        /// Searches for objects in the scene that were generated at run time.
        /// </summary>
        private void DoSearch()
        {
            this.message = null;
            switch (this.SearchForType)
            {
                case SearchType.Materials:
                    // get all renderer's in the scene
                    var renderer = FindSceneObjectsOfType(typeof(MeshRenderer)) as MeshRenderer[];

                    // search for materials that have no associated disk based asset
                    var result = (from r in renderer
                                  where r.sharedMaterials != null && r.sharedMaterials.Length > 0
                                  from mat in r.sharedMaterials
                                  where mat != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mat))
                                  select new AssetModel
                                  {
                                      ID = mat.GetInstanceID(),
                                      Name = mat.name,
                                      Reference = mat,
                                      GameObject = r.gameObject,
                                      Callback = this.DrawSaveButton ,
                                      GameObjectCallback = this.DrawSelectOwner
                                  }).ToList();

                    if (result.Count > 0)
                    {
                        // assign model to the table
                        var model = new AssetTableModel(result);
                        this.table.Model = model;
                    }
                    else
                    {
                        this.message = "No results found!";
                    }
                    break;
            }
        }

        private void DrawSelectOwner(int row, int column, ITableModel<AssetModel> tableModel)
        {
            var options = new[] { GUILayout.MaxHeight(GUI.skin.label.CalcHeight(new GUIContent("Tj"), 1) - 1) };
            if (this.table.RowHeight > 0)
            {
                options = new[] { GUILayout.MaxHeight(this.table.RowHeight - 1) };
            }

            var model = ((AssetTableModel)tableModel).Elements[row];

            var text = model.GameObject.name;
            text = string.IsNullOrEmpty(text) ? "<no name>" : text;
            if (GUILayout.Button(text, options))
            {
                Selection.objects = new[] { model.GameObject };
                foreach (SceneView view in SceneView.sceneViews)
                {
                    view.FrameSelected();
                }
            }
        }

        private void DrawSaveButton(int row, int column, ITableModel<AssetModel> tableModel)
        {
            var options = new[] { GUILayout.MaxHeight(GUI.skin.label.CalcHeight(new GUIContent("Tj"), 1) - 1) };
            if (this.table.RowHeight > 0)
            {
                options = new[] { GUILayout.MaxHeight(this.table.RowHeight - 1) };
            }

            if (GUILayout.Button("Save", options))
            {
                var model = (AssetTableModel)tableModel;
                this.SaveMaterialAsset(model.Elements[row]);
                model.Elements.RemoveAt(row);
                if (model.Elements.Count == 0)
                {
                    this.table.Model = null;
                }
            }
        }

        private void SaveMaterialAsset(AssetModel model)
        {
            var file = System.IO.Path.Combine(this.outputFolder.OutputPath, model.Name);
            file = System.IO.Path.Combine("Assets", file);
            var number = 1;
            var numberString = string.Empty;
            while (System.IO.File.Exists(System.IO.Path.ChangeExtension(file + numberString, ".mat")))
            {
                number++;
                numberString = number.ToString(CultureInfo.InvariantCulture);
            }

            file = System.IO.Path.ChangeExtension(file + numberString, ".mat");

            AssetDatabase.CreateAsset(model.Reference as Material, file);
        }

        /// <summary>
        /// Shows the Scene Asset Saver window.
        /// </summary>
        [MenuItem("Codefarts/General Utilities/Scene Asset Saver")]
        public static void ShowWindow()
        {
            var window = GetWindow<SceneAssetSaver>("Scene Asset Saver", true);
            window.Show();
        }
    }
}
