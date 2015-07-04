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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    using Codefarts.GeneralTools.Common;
    using Codefarts.GeneralTools.Editor.Controls;
    using Codefarts.GeneralTools.Editor.Models;

    using UnityEditor;

    using UnityEngine;

    using Vector2 = UnityEngine.Vector2;

    /// <summary>
    /// Provides a window for adding or removing synchronizations for files, folders or project files.
    /// </summary>
    public class UnityAssetSync : EditorWindow
    {
        /// <summary>
        /// Holds a reference to a table control.
        /// </summary>
        private readonly Table<FilterModel> filterTable;

        /// <summary>
        /// Holds a reference to a table control.
        /// </summary>
        private readonly Table<CodeSyncModel> entriesTable;

        /// <summary>
        /// Holds the asset sync model. 
        /// </summary>
        private readonly CodeSyncTableModel tableModel;

        /// <summary>
        /// Holds the filter model.
        /// </summary>
        private readonly FilterTableModel filterTableModel;

        /// <summary>
        /// Holds the scroll value for the table.
        /// </summary>
        private Vector2 scrollPosition;

        /// <summary>
        /// Holds the current file name for the entries.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Records if entries have changed.
        /// </summary>
        private bool isDirty;

        /// <summary>
        /// Holds an array of code file extensions.
        /// </summary>
        readonly string[] codeFileExtensions = new[] { ".cs", ".js", ".boo" };

        /// <summary>
        /// Used to determine if unity has recompiled scripts.
        /// </summary>
        private RecompileClass recompile;

        /// <summary>
        /// Private class used to determine if unity has recompiled scripts.
        /// </summary>
        private class RecompileClass
        {
        }

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public UnityAssetSync()
        {
            this.tableModel = new CodeSyncTableModel();
            this.filterTableModel = new FilterTableModel();
            this.entriesTable = new Table<CodeSyncModel> { AlwaysDraw = true, Model = this.tableModel, RowHeight = 18 };
            this.filterTable = new Table<FilterModel> { AlwaysDraw = true, Model = this.filterTableModel, RowHeight = 18, TableName = "Filters" };
            this.filterTable.Model = this.filterTableModel;
        }

        /// <summary>
        /// handles th click event for the add button.
        /// </summary>
        /// <param name="type"></param>
        private void AddClick(CodeSyncType type)
        {
            if (this.filterTableModel.Elements != null)
            {
                this.filterTableModel.Elements.Add(new FilterModel { RemoveCallback = this.OnRemoveFilterCallback });
                return;
            }

            // add a new model to the table
            var model = new CodeSyncModel();
            model.SourceCallback = OnSourceCallback;
            model.DestinationCallback = OnDestinationCallback;
            model.RemoveCallback = OnRemoveCallback;
            model.SyncTypeCallback = OnSyncTypeCallback;
            model.FilterCallback = OnFilterCallback;
            model.SyncType = type;
            this.tableModel.Elements.Add(model);
            this.isDirty = true;
        }

        /// <summary>
        /// Handles the callback for drawing the sync type popup.
        /// </summary>
        /// <param name="row">The cell row of the model in question.</param>
        /// <param name="column">The cell column of the model in question.</param>
        /// <param name="model">Reference to the model responsible for accessing table cells.</param>
        private void OnSyncTypeCallback(int row, int column, ITableModel<CodeSyncModel> model)
        {
            var item = ((CodeSyncTableModel)model).Elements[row];
            var value = (CodeSyncType)EditorGUILayout.EnumPopup(item.SyncType, GUILayout.MaxHeight(this.entriesTable.RowHeight - 1));

            // check if selection changed
            if (value == item.SyncType)
            {
                return;
            }

            // determine what to do based on selection
            switch (value)
            {
                case CodeSyncType.File:
                    // ensure source file exists before setting source
                    if (!string.IsNullOrEmpty(item.Source) && !File.Exists(item.Source))
                    {
                        item.Source = string.Empty;
                    }
                    break;

                case CodeSyncType.Folder:
                    // Check if existing source refers to a file and if go use the file path
                    if (!string.IsNullOrEmpty(item.Source) && File.Exists(item.Source))
                    {
                        item.Source = Path.GetDirectoryName(item.Source);
                    }
                    // ensure source folder exists before setting source
                    else if (!string.IsNullOrEmpty(item.Source) && !Directory.Exists(item.Source))
                    {
                        // prevent non existent folders from being specified
                        item.Source = string.Empty;
                    }
                    break;

                case CodeSyncType.Project:
                    // ensure source file exists before setting source
                    if (!string.IsNullOrEmpty(item.Source) && !File.Exists(item.Source))
                    {
                        // prevent non existent file from being specified
                        item.Source = string.Empty;
                    }

                    if (!string.IsNullOrEmpty(item.Source))
                    {
                        // ensure filename extension is what we are looking for
                        var extension = Path.GetExtension(item.Source);
                        if (extension != ".csproj" && extension != ".unityproj")
                        {
                            item.Source = string.Empty;
                        }
                    }

                    break;
            }

            // set sync type for the model
            item.SyncType = value;
        }

        /// <summary>
        /// Handles displaying a remove button for removing entries.
        /// </summary>
        /// <param name="row">The cell row of the model in question.</param>
        /// <param name="column">The cell column of the model in question.</param>
        /// <param name="model">Reference to the model responsible for accessing table cells.</param>
        private void OnRemoveCallback(int row, int column, ITableModel<CodeSyncModel> model)
        {
            // if button clicked remove button at row
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(60), GUILayout.MaxHeight(this.entriesTable.RowHeight - 1)))
            {
                var m = (CodeSyncTableModel)model;
                m.Elements.RemoveAt(row);
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Handles displaying a remove button for removing entries.
        /// </summary>
        /// <param name="row">The cell row of the model in question.</param>
        /// <param name="column">The cell column of the model in question.</param>
        /// <param name="model">Reference to the model responsible for accessing table cells.</param>
        private void OnRemoveFilterCallback(int row, int column, ITableModel<FilterModel> model)
        {
            // if button clicked remove button at row
            if (GUILayout.Button("Remove", GUILayout.MaxWidth(60), GUILayout.MaxHeight(this.filterTable.RowHeight)))
            {
                var m = (FilterTableModel)model;
                m.Elements.RemoveAt(row);
                this.isDirty = true;
            }
        }

        /// <summary>
        /// Handles the callback for drawing the asset source location.
        /// </summary>
        /// <param name="row">The cell row of the model in question.</param>
        /// <param name="column">The cell column of the model in question.</param>
        /// <param name="model">Reference to the model responsible for accessing table cells.</param>
        private void OnSourceCallback(int row, int column, ITableModel<CodeSyncModel> model)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // get entry and provide a text field for it
            var item = ((CodeSyncTableModel)model).Elements[row];
            item.Source = GUILayout.TextField(string.IsNullOrEmpty(item.Source) ? string.Empty : item.Source, GUILayout.MinWidth(100));

            // provide a button to select a asset using a dialog window.
            if (GUILayout.Button("...", GUILayout.MaxWidth(32), GUILayout.MaxHeight(this.entriesTable.RowHeight - 1)))
            {
                // Determine what to do based on sync type
                switch (item.SyncType)
                {
                    case CodeSyncType.File:
                        // show a open file dialog for file
                        var file = EditorUtility.OpenFilePanel("Source File", string.Empty, string.Empty);
                        if (file.Length != 0)
                        {
                            item.Source = file;
                        }
                        break;

                    case CodeSyncType.Folder:
                        // show a open folder dialog
                        var folder = EditorUtility.OpenFolderPanel("Source Folder", string.Empty, string.Empty);
                        if (folder.Length != 0)
                        {
                            item.Source = folder;
                        }
                        break;

                    case CodeSyncType.Project:
                        // show a open file dialog for project files
                        var projectFile = EditorUtility.OpenFilePanel("Source Project", string.Empty, "*.csproj;*.unityproj");
                        if (projectFile.Length != 0)
                        {
                            item.Source = projectFile;
                        }
                        break;
                }

            }
            GUILayout.EndHorizontal();
        }


        /// <summary>
        /// Handles the callback for drawing the asset destination location.
        /// </summary>
        /// <param name="row">The cell row of the model in question.</param>
        /// <param name="column">The cell column of the model in question.</param>
        /// <param name="model">Reference to the model responsible for accessing table cells.</param>
        private void OnDestinationCallback(int row, int column, ITableModel<CodeSyncModel> model)
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            // save project folder
            var projectFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");

            // get entry and provide a text field for it
            var item = ((CodeSyncTableModel)model).Elements[row];
            var value = GUILayout.TextField(string.IsNullOrEmpty(item.Destination) ? string.Empty : item.Destination, GUILayout.MinWidth(100));

            // determine if value was changed
            if (value != item.Destination)
            {
                // prevent destination from starting with directory separator character
                var index = 0;
                while (index < value.Length - 1 &&
                      (value.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)) ||
                       value.StartsWith(Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)) ||
                       value[index] == ' '))
                {
                    if (value[index] == ' ')
                    {
                        index++;
                    }
                    else
                    {
                        value = value.Remove(index, 1);
                    }
                }

                // build a destination path
                var path = Path.GetFullPath(Path.Combine("Assets", value));

                // ensure the destination path is inside the projects Asset folder
                if (!path.StartsWith(projectFolder))
                {
                    value = item.Destination;
                }

                item.Destination = value;
            }

            // provide a button to set the destination folder using a dialog window
            if (GUILayout.Button("...", GUILayout.MaxWidth(32), GUILayout.MaxHeight(this.entriesTable.RowHeight - 1)))
            {
                // show folder select dialog
                var folder = EditorUtility.OpenFolderPanel("Destination Folder", projectFolder, string.Empty);
                if (folder.Length != 0)
                {
                    folder = folder.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                    // ensure selected folder is under the Asset folder hierarchy
                    if (folder.StartsWith(projectFolder))
                    {
                        item.Destination = folder.Remove(0, projectFolder.Length);

                        // remove leading directory separator char if there is one
                        if (item.Destination.StartsWith(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)) ||
                           item.Destination.StartsWith(Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture)))
                        {
                            item.Destination = item.Destination.Remove(0, 1);
                        }
                    }
                    else
                    {
                        // display notice of bad destination folder selected
                        EditorUtility.DisplayDialog("Information", "Must select a destination within the projects assets folder.", "Accept");
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called by unity to draw the window ui.
        /// </summary>
        public void OnGUI()
        {
            // check if recompile variable is null and a filename has been set and still exists 
            if (this.recompile == null && !string.IsNullOrEmpty(this.fileName) && File.Exists(this.fileName))
            {
                // if yes then reload data and create a reference to a recompile class 
                this.LoadData(false);
                this.recompile = new RecompileClass();
            }

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            // draw the button along the top of the window
            this.DrawHeaderButtons();

            if (this.filterTableModel.Elements != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Done", GUILayout.MaxHeight(25), GUILayout.Height(25)))
                {
                    this.filterTableModel.Elements = null;
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            // draw the table
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, false);
            if (this.filterTableModel.Elements != null)
            {
                this.filterTable.Draw();
            }
            else
            {
                this.entriesTable.Draw();
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws action buttons at the top of the window.
        /// </summary>
        private void DrawHeaderButtons()
        {
            // draw the path and filename along the top
            GUILayout.BeginHorizontal();
            GUILayout.Label("File Name: ");
            GUILayout.TextField(this.fileName ?? string.Empty);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // draw the buttons
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Sync Now", GUILayout.MinWidth(75), GUILayout.MinHeight(32)))
            {
                //if (this.HasChanges() && EditorUtility.DisplayDialog("Information", "Unsaved changes must b saved before running sync.","ok"))
                //{

                //}
                this.DoSync(this.tableModel.Elements);
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("New", GUILayout.MinHeight(32)))
            {
                this.StartNewList();
            }

            if (GUILayout.Button("Load", GUILayout.MinHeight(32)))
            {
                this.LoadData(true);
            }

            if (this.HasChanges() && GUILayout.Button("Save", GUILayout.MinHeight(32)))
            {
                this.SaveData(false);
            }

            if (GUILayout.Button("Save As", GUILayout.MinHeight(32)))
            {
                this.SaveData(true);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add", GUILayout.MinWidth(60), GUILayout.MinHeight(32)))
            {
                this.AddClick(CodeSyncType.File);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Starts a new sync list.
        /// </summary>
        private void StartNewList()
        {
            this.tableModel.Elements.Clear();
            if (this.filterTableModel.Elements != null)
            {
                this.filterTableModel.Elements.Clear();
                this.filterTableModel.Elements = null;
            }
            this.fileName = null;
            this.isDirty = false;
        }

        /// <summary>
        /// Loads sync entries from a xml file.
        /// </summary>
        private void LoadData(bool showDialog)
        {
            var file = showDialog ? string.Empty : this.fileName;
            CodeSyncModel[] syncModels;
            if (this.TryToLoadSyncData(showDialog, ref file, out syncModels))
            {
                return;
            }

            // start a new sync list
            this.StartNewList();

            // add models to the table model list
            foreach (var model in syncModels)
            {
                this.tableModel.Elements.Add(model);
            }

            // set the file name 
            this.fileName = file;
        }

        /// <summary>
        /// Attempts to load the sync data from a xml file.
        /// </summary>
        /// <param name="showDialog">If true will show a open file dialog so the user can select a xml sync file.</param>
        /// <param name="file">The file that the user selected to load.</param>
        /// <param name="syncModels">The sync data that was loaded.</param>
        /// <returns>Returns True if successful.</returns>
        private bool TryToLoadSyncData(bool showDialog, ref string file, out CodeSyncModel[] syncModels)
        {
            syncModels = null;
            file = showDialog ? null : file;
            if (showDialog)
            {
                // prompt user to select a sync file
                file = EditorUtility.OpenFilePanel("Load sync data", string.Empty, "xml");
                if (file.Length == 0)
                {
                    return true;
                }
            }

            // enforce directory separation characters (no alt chars)
            file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

            // Attempt to load the xml document
            var doc = new XmlDocument();
            try
            {
                doc.Load(file);
            }
            catch (Exception)
            {
                // report to user that could not read file
                EditorUtility.DisplayDialog("Error", "There was a problem reading the selected file.", "Close");
                return true;
            }

            // check to see if the root document name is what we expect
            if (doc.DocumentElement == null || doc.DocumentElement.LocalName != "assetsync")
            {
                EditorUtility.DisplayDialog("Error", "No root node or root element did not have the name 'assetsync'.", "Close");
                return true;
            }

            // query the xml elements and attempt to generate a model fo each entry
            var results = from item in doc.DocumentElement.ChildNodes.OfType<XmlNode>()
                          where item.LocalName == "entry" && item.ChildNodes.Count > 2
                          let type = item.SelectSingleNode("type")
                          let source = item.SelectSingleNode("source")
                          let destination = item.SelectSingleNode("destination")
                          let filters = item.SelectSingleNode("filters")
                          where type != null && source != null && destination != null
                          select new CodeSyncModel
                          {
                              SyncType = (CodeSyncType)Enum.Parse(typeof(CodeSyncType), type.InnerText),
                              Source = source.InnerText,
                              Destination = destination.InnerText,
                              IsDirty = false,
                              SourceCallback = this.OnSourceCallback,
                              DestinationCallback = this.OnDestinationCallback,
                              RemoveCallback = this.OnRemoveCallback,
                              SyncTypeCallback = this.OnSyncTypeCallback,
                              FilterCallback = this.OnFilterCallback,
                              Filters = filters != null ? (from f in filters.ChildNodes.OfType<XmlNode>()
                                                           where f.LocalName == "filter"
                                                           let attributes = f.Attributes
                                                           where attributes != null
                                                           let allow = attributes["allow"]
                                                           let ext = f.SelectSingleNode("extension")
                                                           let src = f.SelectSingleNode("search")
                                                           let rep = f.SelectSingleNode("replace")
                                                           where allow != null && ext != null && src != null && rep != null
                                                           select new FilterModel { Allow = bool.Parse(allow.InnerText), Extension = ext.InnerText, Search = src.InnerText, Replace = rep.InnerText, RemoveCallback = this.OnRemoveFilterCallback }).ToList() : null
                          };

            try
            {
                syncModels = results.ToArray();
            }
            catch (Exception)
            {
                EditorUtility.DisplayDialog("Error", "The xml file may contain errors, missing values or values that can not be understood.", "Close");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the callback for settings filtering the asset destination location.
        /// </summary>
        /// <param name="row">The cell row of the model in question.</param>
        /// <param name="column">The cell column of the model in question.</param>
        /// <param name="model">Reference to the model responsible for accessing table cells.</param>
        private void OnFilterCallback(int row, int column, ITableModel<CodeSyncModel> model)
        {
            var syncModel = ((CodeSyncTableModel)model).Elements[row];
            // provide a button to set the filtering info
            var text = syncModel.Filters.Count.ToString(CultureInfo.InvariantCulture);
            if (GUILayout.Button(text, GUILayout.MaxWidth(55), GUILayout.MaxHeight(this.entriesTable.RowHeight - 1)))
            {
                this.filterTableModel.Elements = syncModel.Filters;
            }
        }

        /// <summary>
        /// Save the current entries to disk.
        /// </summary>
        private void SaveData(bool saveNew)
        {
            // if filename not set or saveNew is true then prompt user to specify a save location
            if (string.IsNullOrEmpty(this.fileName) || saveNew)
            {
                // prompt user
                var file = EditorUtility.SaveFilePanel("Save sync data", string.Empty, "SyncData.xml", "xml");
                if (file.Length == 0)
                {
                    // user canceled so just exit
                    return;
                }

                // enforce directory separation characters (no alt chars)
                file = file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

                // save file name
                this.fileName = file;
            }

            // generate xml markup from the model data
            var data = "<?xml version=\"1.0\"?>\r\n<assetsync>\r\n{0}\r\n</assetsync>";
            var entries = string.Join("\r\n",
                            (from item in this.tableModel.Elements
                             let filters = string.Join("\r\n", item.Filters.Select(f =>
                                 string.Format("            <filter allow=\"{0}\">\r\n" +
                                               "                <extension><![CDATA[{1}]]></extension>\r\n" +
                                               "                <search><![CDATA[{2}]]></search>\r\n" +
                                               "                <replace><![CDATA[{3}]]></replace>\r\n" +
                                               "            </filter>\r\n", f.Allow, f.Extension, f.Search, f.Replace)).ToArray())
                             select string.Format(
                                 "    <entry>\r\n" +
                                 "        <type>{0}</type>\r\n" +
                                 "        <source>{1}</source>\r\n" +
                                 "        <destination>{2}</destination>\r\n" +
                                 "        <filters>\r\n{3}" +
                                 "        </filters>\r\n" +
                                 "    </entry>", Enum.GetName(typeof(CodeSyncType), item.SyncType), item.Source, item.Destination, filters)).ToArray());
            data = string.Format(data, entries);

            // save data to a xml file
            File.WriteAllText(this.fileName, data);

            // clear the isdirty flags for each model 
            foreach (var model in this.tableModel.Elements)
            {
                model.IsDirty = false;
            }

            this.isDirty = false;

            // refresh the asset data base to ensure any changes are picked up
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Return true if one or more of the entries has changed.
        /// </summary>
        /// <returns></returns>
        public bool HasChanges()
        {
            return this.isDirty || this.tableModel.Elements.Any(model => model.IsDirty) ||
                (this.filterTableModel.Elements != null && this.filterTableModel.Elements.Any(model => model.IsDirty));
        }

        /// <summary>
        /// Synchronizes files by copying them from there source locations into the projects destination locations.
        /// </summary> 
        private void DoSync(IEnumerable<CodeSyncModel> models)
        {
            // get path to project assets folder
            var projectFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assets");

            // Process each element in the table model
            foreach (var model in models)
            {
                // build destination path
                var destinationPath = Path.Combine(projectFolder, string.IsNullOrEmpty(model.Destination) ? string.Empty : model.Destination);

                switch (model.SyncType)
                {
                    case CodeSyncType.File:
                        // skip it if not specified
                        if (string.IsNullOrEmpty(model.Source))
                        {
                            continue;
                        }

                        // ensure source file exists
                        if (File.Exists(model.Source))
                        {
                            this.DoFilteredFileCopy(model.Source, Path.Combine(projectFolder, model.Destination), model.Filters);
                        }
                        break;

                    case CodeSyncType.Folder:
                        // ensure source directory exists
                        if (string.IsNullOrEmpty(model.Source) || !Directory.Exists(model.Source))
                        {
                            continue;
                        }

                        // copy directory structure
                        var filterModel = model.Filters;
                        CopyDirectories(model.Source, destinationPath, true, true, (source, destination, progress) =>
                            {
                                // we will handle file copying our selves so perform a filtered file copy and return false
                                // to prevent the CopyDirectories method from copying the file.
                                this.DoFilteredFileCopy(source, destination, filterModel);
                                return false;
                            });
                        break;

                    case CodeSyncType.Project:
                        // ensure source file exists
                        if (string.IsNullOrEmpty(model.Source))
                        {
                            //continue;
                        }

                        break;
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Copies a file to the destination if it passes all filtering rules.
        /// </summary>
        /// <param name="source">The source file to be copied.</param>
        /// <param name="destinationPath">The destination path where the source file will be copied to.</param>
        /// <param name="filters">The list of filter conditions to be satisfied before file copy will be allowed.</param>
        private void DoFilteredFileCopy(string source, string destinationPath, IEnumerable<FilterModel> filters)
        {
            // check filtering rules
            StringBuilder fileData = null;
            foreach (var filter in filters)
            {
                var extensionMatch = Path.GetExtension(source) == filter.Extension;
                var extensionSpecified = !string.IsNullOrEmpty(filter.Extension);
                if (extensionSpecified && extensionMatch && !filter.Allow)
                {
                    // matched extension but not allowing so return
                    return;
                }

                if (codeFileExtensions.Contains(filter.Extension))
                {
                    if (fileData == null)
                    {
                        fileData = new StringBuilder(File.ReadAllText(source));
                    }

                    fileData.Replace(filter.Search, filter.Replace);
                }
            }

            // check if the source has a file name
            var file = Path.GetFileName(source);
            if (file == null)
            {
                // no file name so skip entry
                return;
            }

            // create destination folder
            Directory.CreateDirectory(destinationPath);
            var destinationFileName = Path.Combine(destinationPath, file);

            if (fileData == null)
            {
                // copy the file and overwrite if already exists
                File.Copy(source, destinationFileName, true);
            }
            else
            {
                File.WriteAllText(destinationFileName, fileData.ToString());
            }
        }

        /// <summary>
        /// Copies a directory structure to the destination.
        /// </summary>
        /// <param name="source">The directory structure to be copied.</param>
        /// <param name="destination">The destination where the directory structure will be copied to.</param>
        /// <param name="copySubDirectories">true to copy all subdirectories.</param>
        /// <param name="overwriteFiles">true if the destination files can be overwritten; otherwise, false.</param>
        /// <param name="callback">Provides a callback function for reporting progress. </param>
        /// <remarks><p>The callback invoked just before a file copy occurs providing a way of being notified.</p>
        /// <p>The callback parameter order is source file, destination file, progress.</p>
        /// <p>If the callback is specified it should return true to allow the file copy to occur.</p> 
        /// <p>The progress parameter reports progress from 0 to 100. Values to the left of the decimal represent folder copy progress and values to the
        /// right of the decimal from 0.000 to 0.99 represent the current file copy progress for the folder that is being copied.</p>
        /// <p>To get the current file copy progress as a value from 0 to 100 use the formula fileProgress = progress - 100 * 100.</p></remarks>
        public static void CopyDirectories(string source, string destination, bool copySubDirectories, bool overwriteFiles, Func<string, string, float, bool> callback)
        {
            // ensure source folder exists
            if (!Directory.Exists(source))
            {
                throw new DirectoryNotFoundException("The path specified in source is invalid (for example, it is on an unmapped drive).");
            }

            // create destination folder
            Directory.CreateDirectory(destination);

            // get all files in source and copy them to destination folder
            var files = Directory.GetFiles(source);
            var progress = 0f; // used to report the progress from 0 to 100

            // set up action to copy files
            var fileProcessor = new Action<float, string[], string>((folderProgress, filesToCopy, folder) =>
                {
                    // copy files
                    for (var i = 0; i < filesToCopy.Length; i++)
                    {
                        // get file
                        var file = filesToCopy[i];

                        // set default result
                        var result = true;

                        // build destination filename
                        var fileName = Path.GetFileName(file);
                        if (fileName == null) // should never happen
                        {
                            return;
                        }

                        fileName = Path.Combine(folder, fileName);

                        // check if callback specified
                        if (callback != null)
                        {
                            // store result from callback
                            result = callback(file, fileName, progress);
                        }

                        // if result is true we are allowed to copy the file
                        if (result)
                        {
                            File.Copy(file, fileName, overwriteFiles);
                        }

                        // (folder progress * 100) + file progress
                        progress = folderProgress + ((float)i / filesToCopy.Length);
                    }
                });

            // copy initial files
            fileProcessor(0, files, destination);

            // check to copy sub directories
            if (!copySubDirectories)
            {
                return;
            }

            // get the folder tree for the source folder
            var folders = Directory.GetDirectories(source, "*.*", SearchOption.AllDirectories);

            // process each sub folder
            for (var index = 0; index < folders.Length; index++)
            {
                // get folder and increment index
                var folder = folders[index];

                // get files
                files = Directory.GetFiles(folder);

                // crop source root from destination and build destination folder path
                folder = folder.Remove(0, source.Length);
                folder = Path.Combine(destination, folder);

                // create destination folder
                Directory.CreateDirectory(folder);

                // process file copying
                fileProcessor((index / folders.Length) * 100, files, folder);
            }
        }

        /// <summary>
        /// Shows the asset sync window.
        /// </summary>
        [MenuItem("Codefarts/General Utilities/Asset Sync")]
        public static void ShowWindow()
        {
            var window = GetWindow<UnityAssetSync>("Asset Sync");
            window.Show();
            window.Focus();
        }
    }
}
