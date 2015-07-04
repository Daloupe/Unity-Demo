namespace Codefarts.GeneralTools.Editor.Models
{
    using System.Collections.Generic;

    using Codefarts.GeneralTools.Common;

    public class FilterTableModel : ITableModel<FilterModel>
    {
        private IList<FilterModel> elements; 

        public IList<FilterModel> Elements
        {
            get { return this.elements; }
            set
            {
                this.elements = value;
            }
        }

        public int GetColumnCount()
        {
            return 5;
        }

        public int GetRowCount()
        {
            return this.elements == null ? 0 : this.elements.Count;
        }

        public bool UseHeaders()
        {
            return true;
        }

        public string GetColumnName(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return "Allow";
                case 1:
                    return "Extension";
                case 2:
                    return "Search";
                case 3:
                    return "Replace";
                default:
                    return string.Empty;
            }
        }

        public int GetColumnWidth(int columnIndex)
        {

            switch (columnIndex)
            {
                case 0:
                    return 20;
                case  1:
                    return 25;
                case 4:
                    return 60;
                default:
                    return 0;
            }
        }

        public object GetValue(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= this.elements.Count)
            {
                return null;
            }

            var el = this.elements[rowIndex];
            switch (columnIndex)
            {
                case 0:
                    return el.Allow;
                case 1:
                    return el.Extension;
                case 2:
                    return el.Search;
                case 3:
                    return el.Replace;
                case 4:
                    return el.RemoveCallback;
            }

            return "Unknown";
        }

        public bool CanEdit(int rowIndex, int columnIndex)
        {
            return true;
        }

        public void SetValue(int rowIndex, int columnIndex, object value)
        {
            if (rowIndex < 0 || rowIndex >= this.elements.Count)
            {
                return;
            }

            var el = this.elements[rowIndex];
            switch (columnIndex)
            {
                case 0:
                    el.Allow = (bool)value;
                    break;
                case 1:
                    el.Extension = ((string)value).Trim();
                    break;
                case 2:
                    el.Search = (string)value;
                    break;
                case 3:
                    el.Replace = (string)value;
                    break;
            }
        }
    }
}
