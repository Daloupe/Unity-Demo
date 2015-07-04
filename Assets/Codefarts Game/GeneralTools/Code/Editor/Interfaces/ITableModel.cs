/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/

namespace Codefarts.GeneralTools.Common
{
    using System;

    public interface ITableModel<T>
    {
      //  void SetTableEntries(IList<T> _entries);
        int GetColumnCount();
        int GetRowCount();
        bool UseHeaders();

        string GetColumnName(int columnIndex);
        int GetColumnWidth(int columnIndex);
        object GetValue(int rowIndex, int columnIndex);
        bool CanEdit(int rowIndex, int columnIndex);

        void SetValue(int rowIndex, int columnIndex, Object value);
    }
}
