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
    /// Provides a utility window for renaming selected asset files.
    /// </summary>
    public class MassAssetRenameWindow : EditorWindow
    {
        /// <summary>
        /// Holds the state of the remove from start check box.
        /// </summary>
        bool chkRemoveFromStart;

        /// <summary>
        /// Holds the state of the add to start check box.
        /// </summary>
        bool chkAddToStart;

        /// <summary>
        /// Holds the state of the remove from end check box.
        /// </summary>
        bool chkRemoveFromEnd;

        /// <summary>
        /// Holds the state fo the add to end check box.
        /// </summary>
        bool chkAddToEnd;

        /// <summary>
        /// Holds the state fo the output to console check box.
        /// </summary>
        bool chkConsole;   

        /// <summary>
        /// Holds the state of the preserve labels check box.
        /// </summary>
        bool chkPreserveLabels;

        /// <summary>
        /// Holds the string that will be added to the start of the asset file name.
        /// </summary>
        string textAddToStart;

        /// <summary>
        /// Holds the number of characters to be removed from the start of the asset file name.
        /// </summary>
        int intRemoveFromStart;

        /// <summary>
        /// Holds the string that will be appended to the end of the asset file name.
        /// </summary>
        string textAddToEnd;
       
        /// <summary>
        /// Holds the number of characters to be removed from the end of the asset file name.
        /// </summary>
        int intRemoveFromEnd;


        /// <summary>
        /// Called by unity to draw the window controls.
        /// </summary>
        public void OnGUI()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            // step 1
            GUILayout.BeginHorizontal();
            this.chkRemoveFromStart = GUILayout.Toggle(this.chkRemoveFromStart, string.Empty, GUILayout.MaxWidth(25));
            this.intRemoveFromStart = EditorGUILayout.IntField(local.Get("Step1RemoveFromStart"), this.intRemoveFromStart);
            this.intRemoveFromStart = this.intRemoveFromStart < 1 ? 1 : this.intRemoveFromStart;
            GUILayout.EndHorizontal();

            // step 2
            GUILayout.BeginHorizontal();
            this.chkAddToStart = GUILayout.Toggle(this.chkAddToStart, string.Empty, GUILayout.MaxWidth(25));
            this.textAddToStart = EditorGUILayout.TextField(local.Get("Step2AddToStart"), this.textAddToStart);
            GUILayout.EndHorizontal();

            // step 3
            GUILayout.BeginHorizontal();
            this.chkRemoveFromEnd = GUILayout.Toggle(this.chkRemoveFromEnd, string.Empty, GUILayout.MaxWidth(25));
            this.intRemoveFromEnd = EditorGUILayout.IntField(local.Get("Step3RemoveFromEnd"), this.intRemoveFromEnd);
            this.intRemoveFromEnd = this.intRemoveFromEnd < 1 ? 1 : this.intRemoveFromEnd;
            GUILayout.EndHorizontal();

            // step 4
            GUILayout.BeginHorizontal();
            this.chkAddToEnd = GUILayout.Toggle(this.chkAddToEnd, string.Empty, GUILayout.MaxWidth(25));
            this.textAddToEnd = EditorGUILayout.TextField(local.Get("Step4AddToEnd"), this.textAddToEnd);
            GUILayout.EndHorizontal();

            // step 5
            GUILayout.BeginHorizontal();
            this.chkPreserveLabels = GUILayout.Toggle(this.chkPreserveLabels, string.Empty, GUILayout.MaxWidth(25));
            GUILayout.Label(local.Get("Step5PreserveLabels"));
            GUILayout.EndHorizontal();

            // output to console option 
            GUILayout.BeginHorizontal();
            this.chkConsole = GUILayout.Toggle(this.chkConsole, string.Empty, GUILayout.MaxWidth(25));
            GUILayout.Label(local.Get("OutputToConsoleJustTest"));
            GUILayout.EndHorizontal();

            GUILayout.Label(local.Get("ErrorsReportedToConsole"));

            // provide a button to rename the assets
            if (GUILayout.Button(this.chkConsole ? local.Get( "Test" ): local.Get("Rename"), GUILayout.Height(30)))
            {
                this.DoRenameAssets();
            }

            // flexible space to prevent button stretching
            GUILayout.FlexibleSpace();
        }

        /// <summary>
        /// Renames the selected asset files.
        /// </summary>
        private void DoRenameAssets()
        {
            // loop through each selected asset file
            foreach (var item in Selection.objects)
            {
                // attempt to get the asset path for the selected asset 
                var selectedAssetPath = AssetDatabase.GetAssetPath(item);
                if (selectedAssetPath == string.Empty)
                {
                    continue; // not a saved asset so skip to next selection
                }

                                      // attempt to generate a unique asset path
                var dummyPath = System.IO.Path.Combine(selectedAssetPath, "fakemonkehpewperzweeeweeeweeeIPFREELY.asset");
                var assetPath = AssetDatabase.GenerateUniqueAssetPath(dummyPath);

                // if asset at that path already exists (witch it should not) skip to next selected asset
                if (assetPath != string.Empty)
                {
                    continue;
                }

                // couldn't generate a path, current asset must be a file
                var newName = item.name;

                // do step 1
                var count = this.intRemoveFromStart;
                if (count > newName.Length) count = newName.Length;
                if (this.chkRemoveFromStart) newName = newName.Remove(0, count);

                // do step 2
                if (this.chkAddToStart) newName = this.textAddToStart + newName;

                // do step 3
                count = this.intRemoveFromEnd;
                if (count > newName.Length) count = newName.Length;
                if (this.chkRemoveFromEnd) newName = newName.Remove(newName.Length - count, count);

                // do step 4
                if (this.chkAddToEnd) newName += this.textAddToEnd;

                if (this.chkConsole)
                {
                    var folder = System.IO.Path.GetDirectoryName(selectedAssetPath);
                    var ext = System.IO.Path.GetExtension(selectedAssetPath);
                    var newFileName = System.IO.Path.Combine(folder, newName + ext);
                    var local = LocalizationManager.Instance;
                    Debug.Log(string.Format(local.Get("OriginalFile"), selectedAssetPath));
                    Debug.Log(string.Format(local.Get("NewFile"), newFileName));
                }
                else
                {
                    // save asset labels
                    var labels = AssetDatabase.GetLabels(item);
                    
                    // rename the asset
                    var output = AssetDatabase.RenameAsset(selectedAssetPath, newName);
                   
                    // report output if not null or empty
                    if (!string.IsNullOrEmpty(output))
                    {
                        Debug.LogError(output);
                    }
                    else
                    {
                        // restore labels
                        AssetDatabase.SetLabels(item, labels);
                    }
                }

            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Codefarts/General Utilities/Mass Asset Rename")]
        public static void ShowMassAssetRenameWindow()
        {
            // get the window, show it, and hand it focus
            var local = LocalizationManager.Instance;
            GetWindow<MassAssetRenameWindow>(local.Get("MassAssetRename")).Show();
        }
    }
}