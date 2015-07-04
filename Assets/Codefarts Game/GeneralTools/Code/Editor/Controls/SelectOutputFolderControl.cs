namespace Codefarts.GeneralTools.Editor.Controls
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Codefarts.GeneralTools.Editor.Utilities;
    using Codefarts.Localization;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// Used to draw controls that allow the user to select a folder
    /// </summary>
    public class SelectOutputFolderControl
    {
        /// <summary>
        /// Used to determine weather or not to show the output
        /// </summary>
        public bool ShowAsList { get; set; }

        /// <summary>
        /// Holds a reference to the output path where materials will be generated.
        /// </summary>
        private string outputPath = String.Empty;

        /// <summary>
        /// Stores the current output folder selection index is <see cref="ShowAsList"/> is true.
        /// </summary>
        private int selectedFolderIndex;

        /// <summary>
        /// Used to hold a last of asset folders
        /// </summary>
        private readonly FolderCache folderCache;

        /// <summary>
        /// Gets or Sets a value indicating whether or not the "AsList" check box will be visible.
        /// </summary>
        public bool ShowAsListCheckBox { get; set; }

        /// <summary>
        /// Provides an event handler for when the output path changes.
        /// </summary>
        public EventHandler OutputPathChanged;

        /// <summary>
        /// Gets a path to the folder that has been specified.
        /// </summary>
        public string OutputPath
        {
            get
            {
                return this.outputPath;
            }
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SelectOutputFolderControl()
        {
            // setup folder cache
            this.folderCache = new FolderCache { RootFolder = "Assets/", Seconds = 2 };
            this.ShowAsListCheckBox = true;
        }

        /// <summary>
        /// Used to select a output path where materials will be saved to.
        /// </summary>
        private void DoSelectOutputPath()
        {
            var local = LocalizationManager.Instance;

            // prompt the user to select a path
            var tempPath = EditorUtility.OpenFolderPanel(local.Get("SelectOutputPath"), this.outputPath, String.Empty);
            this.SetOutputPath(tempPath);
        }

        /// <summary>
        /// Sets the output path.
        /// </summary>
        /// <param name="path">The value to set the output path to.</param>
        /// <remarks>Invalid characters in <see cref="path"/> will be removed and will also prevent rooted paths, and paths that are 
        /// outside of the projects "Assets" folder.</remarks>
        public void SetOutputPath(string path)
        {
            path = path == null ? string.Empty : path.Trim();
            var invalidPathChars = System.IO.Path.GetInvalidPathChars();
            var index = path.IndexOfAny(invalidPathChars);
            while (index != -1)
            {
                path = path.Remove(index, 1);
                index = path.IndexOfAny(invalidPathChars);
            }

            if (System.IO.Path.IsPathRooted(path))
            {
                path = string.Empty;
            }

            // get assets full path
            if (!string.IsNullOrEmpty(path))
            {
                var rootPath = System.IO.Path.GetFullPath(this.folderCache.RootFolder);
                var specifiedPath = System.IO.Path.GetFullPath(System.IO.Path.Combine("Assets", path));
                if (!specifiedPath.StartsWith(rootPath))
                {
                    path = string.Empty;
                }

            }
            if (path != this.outputPath)
            {
                this.outputPath = path;
                if (this.OutputPathChanged != null)
                {
                    this.OutputPathChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Used to setup the output folder controls.
        /// </summary>
        public void Draw()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;
            GUILayout.BeginVertical();

            // provide controls for selecting a output path
            GUILayout.BeginHorizontal();
            GUILayout.Label(local.Get("OutputPath"));
            GUILayout.FlexibleSpace();
            var asList = false;
            if (this.ShowAsListCheckBox)
            {
                asList = GUILayout.Toggle(this.ShowAsList, local.Get("AsList"));
            }
            GUILayout.EndHorizontal();


            // check is wanting to display as list or text field
            if (asList)
            {
                // get list of project asset folders for popup list
                var folders = this.folderCache.GetFolders().Union(new[] { "<root>" }).OrderBy(s => s).ToArray();

                // if previous state was text field attempt to find the specified folder name in the list and set the selection index to match
                if (!this.ShowAsList)
                {
                    // check if output path is specified
                    this.outputPath = this.outputPath.Trim();
                    if (String.IsNullOrEmpty(this.outputPath))
                    {
                        // reset selected folder index and attempt to set output path from the array of available folders
                        this.selectedFolderIndex = 0;
                        if (folders.Length != 0)
                        {
                            this.SetOutputPath(string.Empty);// folders[this.selectedFolderIndex]);
                        }
                    }
                    else
                    {
                        // try to find user specified folder from the output path field in the popup list of folders and set the index to it
                        for (var i = 0; i < folders.Length; i++)
                        {
                            if (String.Compare(this.outputPath, folders[i], true, CultureInfo.InvariantCulture) != 0)
                            {
                                continue;
                            }

                            this.selectedFolderIndex = i;
                            break;
                        }
                    }
                }

                // draw popup list  of project folders
                var index = EditorGUILayout.Popup(this.selectedFolderIndex, folders);
                if (index != this.selectedFolderIndex)
                {
                    this.selectedFolderIndex = index;
                    this.SetOutputPath(this.selectedFolderIndex == 0 ? string.Empty : folders[this.selectedFolderIndex]);
                }
            }
            else
            {
                // check if user wants to show text field and previous state was to show list
                string tempPath;
                if (this.ShowAsList)
                {
                    // set output folder based on list selection
                    var folders = this.folderCache.GetFolders().Union(new[] { "<root>" }).OrderBy(x => x).ToArray();
                    this.SetOutputPath(this.selectedFolderIndex == 0 ? string.Empty : folders[this.selectedFolderIndex]);
                }

                GUILayout.BeginHorizontal();
                // display output folder controls
                tempPath = GUILayout.TextField(this.outputPath);
                this.SetOutputPath(tempPath);

                if (GUILayout.Button("...", GUILayout.MaxWidth(40)))
                {
                    this.DoSelectOutputPath();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            // save as list state
            this.ShowAsList = asList;
        }
    }
}