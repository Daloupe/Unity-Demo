namespace Codefarts.GeneralTools.Editor.Models
{
    using System;

    using Codefarts.GeneralTools.Common;

    public class FilterModel
    {
        private string replace;

        private string search;

        private string extension;

        public string Extension
        {
            get
            {
                return this.extension;
            }
            set
            {
                if (this.extension == value)
                {
                    return;
                }
                this.extension = value;
                this.IsDirty = true;
            }
        }

        public bool Allow { get; set; }

        public string Search
        {
            get
            {
                return this.search;
            }
            set
            {
                if (this.search == value)
                {
                    return;
                }
                this.search = value;
                this.IsDirty = true;
            }
        }

        public string Replace
        {
            get
            {
                return this.replace;
            }
            set
            {
                if (this.replace == value)
                {
                    return;
                }
                this.replace = value;
                this.IsDirty = true;
            }
        }

        public Action<int, int, ITableModel<FilterModel>> RemoveCallback { get; set; }

        public bool IsDirty { get; set; }

        public FilterModel()
        {
            this.Extension = string.Empty;
            this.Search = string.Empty;
            this.Replace = string.Empty;
        }
    }
}