namespace Codefarts.GeneralTools.Editor.Utilities
{
    using System;
    using System.IO;

    /// <summary>
    /// Provides a class for caching a list of folders.
    /// </summary>
    public class FolderCache
    {
        /// <summary>
        /// Used to hold a list of folder names.
        /// </summary>
        private string[] cachedFolders;

        /// <summary>
        /// Used by <see cref="UpdateCachedFolders"/> when determining whether or not to update the <see cref="cachedFolders"/> array.
        /// </summary>
        private DateTime lastFolderUpdate;

        /// <summary>
        /// Gets or Sets the number of seconds that must elapse before the cache is refreshed.
        /// </summary>
        /// <remarks>the default is 2 seconds.</remarks>
        public int Seconds { get; set; }

        /// <summary>
        /// Gets or Sets the root folder where folder caching starts.
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// Gets or Sets the folder search options when rebuilding the cache.
        /// </summary>
        /// <remarks>The default value is <see cref="SearchOption.AllDirectories"/>.</remarks>
        public SearchOption Options { get; set; }


        /// <summary>
        /// Default constructor.
        /// </summary>
        public FolderCache()
        {
            this.Options = SearchOption.AllDirectories;
        }

        /// <summary>
        /// Return the list of folders found under the projects Assets folder
        /// </summary>
        /// <returns>Returns a list of asset folders.</returns>
        /// <remarks>This method caches the asset folder list for X number of seconds to prevent retrieval of folders at every request. 
        /// Since requests can potentially occur many times a second caching is used to help reduce the frequency of any possible lag or UI unresponsiveness.</remarks>
        /// <seealso cref="Seconds"/>
        public string[] GetFolders()
        {
            this.Seconds = 2;
            this.UpdateCachedFolders(this.Seconds);
            return this.cachedFolders;
        }

        /// <summary>
        /// Updates the <see cref="cachedFolders"/> array with the list of folders found under the projects Assets folder.
        /// </summary>
        /// <param name="seconds">The number of seconds between updates.</param>
        private void UpdateCachedFolders(double seconds)
        {
            // check if it's time to update the list of folders
            if (DateTime.Now <= this.lastFolderUpdate + TimeSpan.FromSeconds(seconds))
            {
                return;
            }

            // get all asset folders
            this.cachedFolders = GetDirectories(this.RootFolder, "*.*", this.Options);
            for (var i = 0; i < this.cachedFolders.Length; i++)
            {
                this.cachedFolders[i] = this.cachedFolders[i].Substring(this.RootFolder.Length).Replace("\\", "/");
            }

            // record the time of the update
            this.lastFolderUpdate = DateTime.Now;
        }

        /// <summary>
        /// Builds an array of folders & sub folders.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="pattern">The search string to match against the names of files in path. The parameter cannot end in two periods 
        /// ("..") or contain two periods ("..") followed by System.IO.Path.DirectorySeparatorChar or System.IO.Path.AltDirectorySeparatorChar,
        /// nor can it contain any of the characters in System.IO.Path.InvalidPathChars.
        ///</param>
        /// <param name="searchOptions">One of the System.IO.SearchOption values that specifies whether the search operation should include 
        /// all subdirectories or only the current directory.
        ///</param>
        /// <returns>A String array of directories that match the search pattern.</returns>
        public static string[] GetDirectories(string path, string pattern, SearchOption searchOptions)
        {
            // check if searching for all directories
            if (searchOptions == SearchOption.AllDirectories)
            {
                // add start paths to list
                var list = Directory.GetDirectories(path, pattern);
                var index = 0;
                var count = list.Length;

                // process list and add folders to end of list
                while (index < count)
                {
                    var directories = Directory.GetDirectories(list[index++], pattern);
                    if (directories.Length > 0)
                    {
                        // check if we need more space to store the directories
                        if (count + directories.Length > list.Length - 1)
                        {
                            Array.Resize(ref list, list.Length + directories.Length+ 1000);
                        }

                        // add directories to end of the list
                        foreach (var directory in directories)
                        {
                            list[count++] = directory;
                        }
                    }
                    
                    // trim unused index from end of the array
                    if (list.Length > count)
                    {
                        Array.Resize(ref list, count);
                    }
                }

                return list;
            }

            // just return initial list of folder with no sub folders
            return Directory.GetDirectories(path, pattern);
        }
    }
}