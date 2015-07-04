/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/

namespace Codefarts.GeneralTools.Editor.Controls
{
    using System;

    using Codefarts.GeneralTools.Common;

    using UnityEditor;

    using UnityEngine;

    public class Table<T>
    {
        public string TableName { get; set; }

        public Action AddButton;
        public Action RemoveButton;

        public EventHandler<TableDrawEventArgs<T>> BeforeDrawCell;
        public EventHandler<TableDrawEventArgs<T>> AfterDrawCell;
        public EventHandler<TableDrawEventArgs<T>> BeforeDrawRow;
        public EventHandler<TableDrawEventArgs<T>> AfterDrawRow;
        public EventHandler<TableDrawEventArgs<T>> BeforeDrawColumn;
        public EventHandler<TableDrawEventArgs<T>> AfterDrawColumn;

        public ITableModel<T> Model { get; set; }

        public bool AlwaysDraw { get; set; }

        public int RowHeight { get; set; }
        public Table(string tableName)
            : this(tableName, null)
        {
            this.TableName = tableName;
        }

        public Table(ITableModel<T> model)
            : this(string.Empty, model)
        {
            this.Model = model;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Table()
        {

        }

        public Table(String tableName, ITableModel<T> model)
            : this()
        {
            this.TableName = tableName;
            this.Model = model;
        }

        /// <summary>
        /// Draws the table.
        /// </summary>
        public void Draw()
        {
            // if there is no model don't draw.
            if (this.Model == null && !this.AlwaysDraw)
            {
                return;
            }

            GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            this.DrawTableName();

            if (this.Model != null)
            {
                var options = new GUILayoutOption[0];
                if (this.RowHeight > 0)
                {
                    options = new[] { GUILayout.Height(this.RowHeight),GUILayout.MaxHeight(this.RowHeight), GUILayout.MinHeight(this.RowHeight) };
                }

                GUILayout.BeginHorizontal();
                for (var column = 0; column < this.Model.GetColumnCount(); column++)
                {
                    this.DoBeforeDrawColumn(column);
                    var colOptions = new GUILayoutOption[0];
                    var columnWidth = this.Model.GetColumnWidth(column);
                    if (columnWidth > 0)
                    {
                        colOptions = new[] { GUILayout.MaxWidth(columnWidth), GUILayout.MinWidth(columnWidth) };
                    }

                    GUILayout.BeginVertical("box", colOptions);
                    if (this.Model.UseHeaders())
                    {
                        GUILayout.BeginHorizontal("box");
                        GUILayout.Label(this.Model.GetColumnName(column));
                        GUILayout.EndHorizontal();
                    }

                    this.DoBeforeDrawRow(column);
                    for (var row = 0; row < this.Model.GetRowCount(); row++)
                    {
                        this.DrawCell(row, column, options);
                    }
                    this.DoAfterDrawRow(column);

                    GUILayout.EndVertical();
                    this.DoAfterDrawColumn(column);
                }
                GUILayout.EndHorizontal();
            }

            this.DrawAddRemoveButtons();

            GUILayout.EndVertical();
        }

        private void DrawTableName()
        {
            if (!string.IsNullOrEmpty(this.TableName))
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(this.TableName);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        private void DrawAddRemoveButtons()
        {
            GUILayout.BeginHorizontal();

            if (this.AddButton != null && this.AddButton.GetInvocationList().Length != 0)
            {
                if (GUILayout.Button("Add"))
                {
                    this.AddButton();
                }
            }

            if (this.RemoveButton != null && this.RemoveButton.GetInvocationList().Length != 0)
            {
                if (GUILayout.Button("Remove"))
                {
                    this.RemoveButton();
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawCell(int row, int column, GUILayoutOption[] options)
        {
            this.DoBeforeDrawCell(row, column);

            var obj = this.Model.GetValue(row, column);

            var callback = obj as Action<int, int, ITableModel<T>>;
            if (callback != null)
            {
                callback(row, column, this.Model);
                return;
            }

            if (!this.Model.CanEdit(row, column))
            {
                GUILayout.Label(obj == null ? string.Empty : obj.ToString(), options);
                return;
            }

            this.DrawControlForValue(row, column, options, obj);

            this.DoAfterDrawCell(row, column);
        }

        private void DrawControlForValue(int row, int column, GUILayoutOption[] options, object obj)
        {
            // draw controls based on the type
            var text = obj as string;
            if (text != null)
            {
                this.Model.SetValue(row, column, EditorGUILayout.TextField(text, options));
            }
            else if (obj is int)
            {
                this.Model.SetValue(row, column, EditorGUILayout.IntField((int)obj, options));
            }
            else if (obj is float)
            {
                this.Model.SetValue(row, column, EditorGUILayout.FloatField((float)obj, options));
            }
            else if (obj is bool)
            {
                this.Model.SetValue(row, column, GUILayout.Toggle((bool)obj, string.Empty, options));
            }
            else
            {
                var texture = obj as Texture2D;
                if (texture != null)
                {
                    this.Model.SetValue(row, column, EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, options));
                }
                else
                {
                    var gameObject = obj as GameObject;
                    if (gameObject != null)
                    {
                        this.Model.SetValue(row, column, EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false, options));
                    }
                    else
                    {
                        var material = obj as Material;
                        if (material != null)
                        {
                            this.Model.SetValue(row, column, EditorGUILayout.ObjectField(material, typeof(Material), false, options));
                        }
                    }
                }
            }
        }

        private void DoBeforeDrawCell(int row, int column)
        {
            if (this.BeforeDrawCell != null)
            {
                this.BeforeDrawCell(this, new TableDrawEventArgs<T> { Column = column, Row = row, Model = this.Model });
            }
        }

        private void DoAfterDrawCell(int row, int column)
        {
            if (this.AfterDrawCell != null)
            {
                this.AfterDrawCell(this, new TableDrawEventArgs<T> { Column = column, Row = row, Model = this.Model });
            }
        }

        private void DoBeforeDrawRow(int column)
        {
            if (this.BeforeDrawRow != null)
            {
                this.BeforeDrawRow(this, new TableDrawEventArgs<T> { Column = column, Row = -1, Model = this.Model });
            }
        }

        private void DoAfterDrawRow(int column)
        {
            if (this.AfterDrawRow != null)
            {
                this.AfterDrawRow(this, new TableDrawEventArgs<T> { Column = column, Row = -1, Model = this.Model });
            }
        }

        private void DoBeforeDrawColumn(int column)
        {
            if (this.BeforeDrawColumn != null)
            {
                this.BeforeDrawColumn(this, new TableDrawEventArgs<T> { Column = column, Row = -1, Model = this.Model });
            }
        }

        private void DoAfterDrawColumn(int column)
        {
            if (this.AfterDrawColumn != null)
            {
                this.AfterDrawColumn(this, new TableDrawEventArgs<T> { Column = column, Row = -1, Model = this.Model });
            }
        }
    }
}