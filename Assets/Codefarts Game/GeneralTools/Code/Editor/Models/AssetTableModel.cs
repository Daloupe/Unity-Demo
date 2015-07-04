/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/

namespace Codefarts.GeneralTools.Editor.Models
{
    using System;
    using System.Collections.Generic;

    using Codefarts.GeneralTools.Common;

    public class AssetTableModel : ITableModel<AssetModel>
    {
        private IList<AssetModel> elements;

        public IList<AssetModel> Elements
        {
            get { return this.elements; }
            set
            {
                this.elements = value;
            }
        }

        public AssetTableModel(IList<AssetModel> elements)
        {
            this.elements = elements;
        }

        public AssetTableModel()
        {

        }

        public int GetColumnCount()
        {
            return 4;
        }

        public int GetRowCount()
        {
            return this.elements == null ? 0 : this.elements.Count;
        }

        public bool UseHeaders()
        {
            return true;
        }

        public String GetColumnName(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return "ID";
                case 2:
                    return "Name";
                case 3:
                    return "Owner";
            }
            return "Unknown";
        }

        public int GetColumnWidth(int columnIndex)
        {
            //switch (columnIndex)
            //{
            //    case 0:
            //        return 45;
            //    case 1:
            //        return 32;
            //    case 2:
            //        return 0;
            //    case 3:
            //        return 0;
            //}
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
                    return el.Callback; 
                case 1:
                    return el.ID;
                case 2:
                    return el.Name;
                case 3:
                    return el.GameObjectCallback;
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
