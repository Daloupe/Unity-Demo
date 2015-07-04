/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/

namespace Codefarts.GeneralTools.Editor.Windows
{
    using System;
    using System.Linq;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// Provides a window where you can select objects by typing in there Instance ID.
    /// </summary>
    public class SelectByIdWindow : EditorWindow
    {
        private string instanceIDs = string.Empty;

        /// <summary>
        /// Called by unity to draw the window ui.
        /// </summary>
// ReSharper disable UnusedMember.Local
        private void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            GUILayout.BeginVertical();
            GUI.SetNextControlName("txtIds");
            this.instanceIDs = GUILayout.TextArea(this.instanceIDs, int.MaxValue, GUILayout.ExpandHeight(true));
           // var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            GUILayout.BeginHorizontal();
           
            GUI.SetNextControlName("btnPaste");
            //if (GUILayout.Button("Paste", GUILayout.Width(125)))
            //{
            //    //var editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            //    ////  editor.Paste();
            //    //foreach (var c in ClipboardHelper.clipBoard.ToCharArray())
            //    //{
            //    //    editor.Insert(c);
            //    //}  
            //    if (te != null )
            //    {
            //     Debug.Log(string.Format("can paste {0}", te.CanPaste()));
            //       te.SelectAll();
            //        te.Copy();
            //        te.Paste();
            //        te.Paste();
            //        te.Paste();
            //    }
            //}
            //if (GUI.GetNameOfFocusedControl() == "txtIDs")
            //{
            //    var te = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            //    if (te != null && te.CanPaste())
            //    {
            //        te.Paste();
            //    }
            //}

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Select", GUILayout.Width(125)))
            {
                // get ids
                try
                {
                    var data = from x in this.instanceIDs.Trim().Split(new[] { ",", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                               select int.Parse(x.Trim());
                    Selection.objects = data.Select(x => EditorUtility.InstanceIDToObject(x)).ToArray();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(ex.Message);
                }

            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        [MenuItem("Codefarts/General Utilities/Select by ID")]
        public static void ShowWindow()
        {
            // get the window, show it, and hand it focus
            var window = GetWindow<SelectByIdWindow>();
            window.title = "Select ID";
            window.Show();
            window.Focus();
            window.Repaint();
        }
    }
}