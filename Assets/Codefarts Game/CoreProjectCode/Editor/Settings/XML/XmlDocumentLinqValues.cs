/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.CoreProjectCode.Settings.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;

    /// <summary>
    /// Provides a XML file based <see cref="IValues{TKey}"/> repository.
    /// </summary>
    public class XmlDocumentLinqValues : IValues<string>
    {
        #region Fields

        /// <summary>
        /// The data store.
        /// </summary>
        private Dictionary<string, object> dataStore = new Dictionary<string, object>();

#if !UNITY_WEBPLAYER
        /// <summary>
        /// The last write time.
        /// </summary>
        private DateTime lastWriteTime = DateTime.MinValue;
        
#endif
        private DateTime lastReadTime = DateTime.MinValue;

        /// <summary>
        /// Gets an array of setting names.
        /// </summary>
        /// <returns>Returns an array of setting names.</returns>
        public string[] GetValueKeys()
        {
            this.Read();
            return this.dataStore.Keys.ToArray();
        }

        public event EventHandler<ValueChangedEventArgs<string>> ValueChanged;
        private readonly ValueChangedEventArgs<string> eventArgs = new ValueChangedEventArgs<string>();

        private int readDelayInSeconds;


        public int ReadDelayInSeconds
        {
            get
            {
                return this.readDelayInSeconds;
            }
            set
            {
                this.readDelayInSeconds = Math.Max(0, value);
            }
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDocumentLinqValues"/> class.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public XmlDocumentLinqValues(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            var directoryName = Path.GetDirectoryName(fileName);
            if (directoryName != null && directoryName.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new Exception("Invalid path characters detected!");
            }

            var name = Path.GetFileName(fileName);
            if (name != null && name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                throw new Exception("Invalid filename characters detected!");
            }

            this.readDelayInSeconds = 5;
            this.FileName = fileName;

            this.Read();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get setting.
        /// </summary>
        /// <param name="key">
        /// The name.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public T GetValue<T>(string key)
        {
            Type type = typeof(T);
            //if (type != typeof(string))
            //{
            //    throw new ArgumentException("Generic type T must be of type string.");
            //}

            this.Read();
            return (T)Convert.ChangeType(this.dataStore[key], type);
        }

        /// <summary>
        /// Returns true if a setting with the specified name exists.
        /// </summary>
        /// <param name="name">The name of the setting to check for.</param>
        /// <returns>Returns true if a setting with the specified name exists.</returns>
        /// <remarks><see cref="name"/> is case sensitive.</remarks>
        public bool HasValue(string name)
        {
            this.Read();
            return this.dataStore.ContainsKey(name);
        }

        public void RemoveValue(string name)
        {
            this.dataStore.Remove(name);
            this.Write();
        }

        /// <summary>
        /// The set setting.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void SetValue(string name, object value)
        {
            //if (!(value is string))
            //{
            //    throw new ArgumentException("'value' argument must be of type string.");
            //}

            if (!this.dataStore.ContainsKey(name))
            {
                this.dataStore.Add(name, null);
            }

            this.dataStore[name] = value.ToString();

            this.Write();
            this.DoSettingChanged(name, value);
        }

        protected void DoSettingChanged(string name, object value)
        {
            if (this.ValueChanged != null)
            {
                this.eventArgs.Name = name;
                this.eventArgs.Value = value;
                this.ValueChanged(this, this.eventArgs);
                this.eventArgs.Value = null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads values into the <see cref="dataStore"/> filed using linq.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        /// <exception cref="FileLoadException">
        /// </exception>
        private void Read()
        {
            // only update reading settings file every 5 seconds
            if (DateTime.Now < this.lastReadTime + TimeSpan.FromSeconds(this.readDelayInSeconds))
            {
                return;
            }

            if (!File.Exists(this.FileName))
            {
                throw new FileNotFoundException("Could not find settings file.", this.FileName);
            }

#if !UNITY_WEBPLAYER
            var info = new FileInfo(this.FileName);
            var writeTime = info.LastWriteTime;

            // check if the file has been written to since last read attempt
            if (writeTime <= this.lastWriteTime)
            {
                return;
            } 
#endif

            var results = XmlDocumentSettingsHelpers.ReadSettings(this.FileName, true);

            this.dataStore = results.ToDictionary(k => k.Key, v => v.Value);

#if !UNITY_WEBPLAYER
            this.lastWriteTime = writeTime;
#endif
            this.lastReadTime = DateTime.Now;
        }

        /// <summary>
        /// Saves current values in the <see cref="dataStore"/> to a xml file using linq.
        /// </summary>
        private void Write()
        {
            var directoryName = Path.GetDirectoryName(this.FileName);

            if (directoryName != null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            var doc = new XmlDocument();
            var declaration = doc.CreateXmlDeclaration("1.0", null, null);

            var settings = doc.CreateElement("settings");
            doc.AppendChild(settings);
            doc.InsertBefore(declaration, doc.DocumentElement);

            // read existing settings file values
            var existingValues = XmlDocumentSettingsHelpers.ReadSettings(this.FileName, true);

            var comparer = EqualityComparerCallback<KeyValuePair<string, object>>.Compare((x, y) => string.CompareOrdinal(x.Key, y.Key) == 0);
            var entries = this.dataStore.Union(existingValues, comparer);

            var nodesToWrite = entries.OrderBy(x => x.Key).Select(x =>
            {
                var entry = doc.CreateElement("entry");
                entry.InnerText = x.Value.ToString();
                var key = doc.CreateAttribute("key");
                key.InnerText = x.Key;
                entry.Attributes.Append(key);
                return entry;
            });

            foreach (var node in nodesToWrite)
            {
                settings.AppendChild(node);
            }

                doc.Save(this.FileName);
#if !UNITY_WEBPLAYER
                this.lastWriteTime = File.GetLastWriteTime(this.FileName);
#endif
        }

        #endregion

        /// <summary>
        /// Finalizes an instance of the <see cref="XmlDocumentLinqValues"/> class. 
        /// </summary>
        ~XmlDocumentLinqValues()
        {
            this.Write();
        }
    }
}