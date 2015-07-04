namespace Codefarts.GeneralTools.Editor.Models
{
    using System;
    using System.Collections.Generic;

    using Codefarts.GeneralTools.Common;

    public enum CodeSyncType
    {
        File,
        Folder,
        Project,
    }

    public class CodeSyncModel
    {
        private string destination;

        private string source;

        private CodeSyncType syncType;

        public CodeSyncType SyncType
        {
            get
            {
                return this.syncType;
            }
            set
            {
                if (value == this.syncType)
                {
                    return;
                }
                this.syncType = value;
                this.IsDirty = true;
            }
        }

        public Action<int, int, ITableModel<CodeSyncModel>> SyncTypeCallback { get; set; }

        public string Source
        {
            get
            {
                return this.source;
            }
            set
            {
                if (value == this.source)
                {
                    return;
                }
                this.source = value;
                this.IsDirty = true;
            }
        }

        public Action<int, int, ITableModel<CodeSyncModel>> SourceCallback { get; set; }

        public string Destination
        {
            get
            {
                return this.destination;
            }
            set
            {
                if (value == this.destination)
                {
                    return;
                }
                this.destination = value;
                this.IsDirty = true;
            }
        }

        public Action<int, int, ITableModel<CodeSyncModel>> DestinationCallback { get; set; }

        public Action<int, int, ITableModel<CodeSyncModel>> RemoveCallback { get; set; }
        public Action<int, int, ITableModel<CodeSyncModel>> FilterCallback { get; set; }

        public bool IsDirty { get; set; }
        public IList<FilterModel> Filters { get; set; }

        public CodeSyncModel()
        {
            this.Filters=new List<FilterModel>();
            this.source = string.Empty;
            this.destination = string.Empty;
        }
    }
}
