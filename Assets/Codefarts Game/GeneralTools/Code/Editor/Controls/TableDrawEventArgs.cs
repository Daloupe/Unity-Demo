namespace Codefarts.GeneralTools.Editor.Controls
{
    using System;

    using Codefarts.GeneralTools.Common;

    /// <summary>
    /// Provides event argumnts for <see cref="Table{T}"/> cells.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    public class TableDrawEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Gets or sets the row of the of the cell to be drawn.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column of the cell to be drawn.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ITableModel{T}"/> reference containing information about the table model.
        /// </summary>
        public ITableModel<T> Model { get; set; }
    }
}