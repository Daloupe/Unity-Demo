namespace Codefarts.GeneralTools.Editor.Models
{
    using System;
    using System.Collections.Generic;

    using Codefarts.GeneralTools.Common;

    public class CodeSyncTableModel : ITableModel<CodeSyncModel>
    {
        private IList<CodeSyncModel> elements = new List<CodeSyncModel>();

        public IList<CodeSyncModel> Elements
        {
            get { return this.elements; }
            set
            {
                this.elements = value;
            }
        }

        public CodeSyncTableModel(IList<CodeSyncModel> elements)
        {
            this.elements = elements;
        }

        public CodeSyncTableModel()
        {
            
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
                    return "Type";
                case 1:
                    return "Source";
                case 2:
                    return "Destination";
                case 3:
                    return "Filters";
            }

            return string.Empty;
        }

        public int GetColumnWidth(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return 67;
                case 3:
                    return 55;
                case 4:
                    return 60;
                
            }
            return 0;
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
                    return el.SyncTypeCallback; 
                case 1:
                    return el.SourceCallback;
                case 2:
                    return el.DestinationCallback;
                case 3:
                    return el.FilterCallback;
                case 4:
                    return el.RemoveCallback;
            }

            return "Unknown";
        }

        public bool CanEdit(int rowIndex, int columnIndex)
        {
            //if (rowIndex < 0 || rowIndex >= this.elements.Count)
            //{
            //    return null;
            //}

            //var el = this.elements[rowIndex];
            //switch (columnIndex)
            //{
            //    case 0:
            //        this.saveButton.Row = rowIndex;
            //        return this.saveButton;
            //    case 1:
            //        return rowIndex;
            //    case 2:
            //        return el.Name;
            //    case 3:
            //        return el.GameObject.name;
            //}

            return false;
        }

        public void SetValue(int rowIndex, int columnIndex, object value)
        {
            //if (rowIndex < 0 || rowIndex >= this.elements.Count)
            //{
            //    return;
            //}

            //var el = this.elements[rowIndex];
            //switch (columnIndex)
            //{
            //    case 0:
            //        //remove button column so do nothing
            //        break;

            //    case 1:
            //        // index do nothing
            //        break;

            //    case 2:
            //        // set name
            //        var behavior = el as AssetModel;
            //        if (behavior != null)
            //        {
            //            behavior.Name = value.ToString();
            //        }
            //        break;
            //}
        }
    }
}