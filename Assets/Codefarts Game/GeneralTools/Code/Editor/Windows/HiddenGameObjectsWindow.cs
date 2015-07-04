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
    using System.Collections.Generic;

    using Codefarts.Localization;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// Provides a tool windows for searching for hidden game objects and making them visible again.
    /// </summary>
    public class HiddenGameObjectsWindow : EditorWindow
    {
        private class GameObjectModel
        {
            public GameObject Object;

            public bool IsChecked;
        }

        /// <summary>
        /// Used to determine if the selection count has changed.
        /// </summary>
        private int lastCount;

        /// <summary>
        ///  Holds the scroll value of the hidden game objects list.
        /// </summary>
        private Vector2 scroll;

        /// <summary>
        /// Used to store a list of game objects that are hidden.
        /// </summary>
        private readonly List<GameObjectModel> items;

        
        public HiddenGameObjectsWindow()
        {
            this.items = new List<GameObjectModel>();
        }
     
        /// <summary>
        /// Called when the hierarchy changes and refreshes the list.
        /// </summary>
        private void OnHierarchyChange()
        {
            this.RefreshList( );
        }

        /// <summary>
        /// Called when the project changes and refreshes the list.
        /// </summary>
        private void OnProjectChange()
        {
            this.RefreshList( );
        }

        /// <summary>
        /// Handles the click event for the None button.
        /// </summary>
        private void SetCheckedState(bool state)
        {
            // go through each item in the list and clear the check box state for each item
            foreach (var child in this.items)
            {
                child.IsChecked = state;
            }
        }
              
        /// <summary>
        /// Handles the click event for the Refresh button.
        /// </summary>
        private void RefreshList()
        {
            // remove all children from the list
            this.items.Clear();

            // get all objects in the scene
            var objects = FindObjectsOfType(typeof(GameObject));

            // find and add all hidden game object to the list
            foreach (GameObject obj in objects)
            {
                if ((obj.hideFlags & HideFlags.HideInHierarchy) == HideFlags.HideInHierarchy)
                {
                    this.items.Add(new GameObjectModel { Object = obj });
                }
            }

            // ensure to repaint the window
            this.Repaint();
        }

        /// <summary>
        /// Handles the click event for the show button.
        /// </summary>
        private void ShowSelectedItems()
        {
            // process each check item in the list and show the object and remove it from the list
            var list = new Stack<GameObjectModel>();
            foreach (var item in this.items)
            {
                if (!item.IsChecked)
                {
                    continue;
                }

                // hidden flag may have changed since last refresh so ensure that the object is hidden before making it visible again
                if ((item.Object.hideFlags & HideFlags.HideInHierarchy) == HideFlags.HideInHierarchy)
                {
                    item.Object.hideFlags &= ~HideFlags.HideInHierarchy;
                    list.Push(item);
                }
            }

            // repaint hierarchy and project windows
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();

            // remove checked items from the list
            while (list.Count > 0)
            {
                this.items.Remove(list.Pop());
            }
        }

        /// <summary>
        /// Handles the click event for the Hide button.
        /// </summary>
        private void HideSelectedItems()
        {
            // toggle off hidden flag on selected game objects
            foreach (var obj in Selection.gameObjects)
            {
                obj.hideFlags |= HideFlags.HideInHierarchy;
            }

            // clear selection & repaint hierarchy and project windows
            Selection.objects = new Object[0];
            EditorApplication.RepaintHierarchyWindow();
            EditorApplication.RepaintProjectWindow();

            // simulate clicking the refresh button
            this.RefreshList();
        }

        /// <summary>
        /// Called by unity when the window needs to update.
        /// </summary>
        private void Update()
        {
            if (Selection.objects != null && this.lastCount != Selection.objects.Length)
            {
                this.RefreshList();
                this.lastCount = Selection.objects.Length;
            }
        }

        /// <summary>
        /// Called by unity when the window needs to redraw.
        /// </summary>
        private void OnGUI()
        {
            // get reference to localization manager
            var local = LocalizationManager.Instance;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            this.scroll = GUILayout.BeginScrollView(this.scroll,false,true);
            foreach (var item in this.items)
            {
                item.IsChecked = GUILayout.Toggle(item.IsChecked, item.Object.name);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.Space(8);

            GUILayout.BeginVertical(GUILayout.MaxWidth(100));
            GUILayout.Space(8);

            if (GUILayout.Button(local.Get("Refresh")))
            {
                this.RefreshList();
            }
            GUILayout.Space(8);

            if (GUILayout.Button(local.Get("All")))
            {
                this.SetCheckedState(true);
            }
            if (GUILayout.Button(local.Get("None")))
            {
                this.SetCheckedState(false);
            }
            GUILayout.Space(8);

            if (GUILayout.Button(local.Get("Hide")))
            {
                this.HideSelectedItems();
            }
            if (GUILayout.Button(local.Get("Show")))
            {
                this.ShowSelectedItems();
            }
            GUILayout.Space(8);

            if (GUILayout.Button(local.Get("Close")))
            {
               this.Close();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// When the window is enabled auto refresh the window.
        /// </summary>
        private void OnEnable()
        {
            this.RefreshList();
        }

        /// <summary>
        /// Provides unity menu item to open the window.
        /// </summary>
        [MenuItem("Codefarts/General Utilities/Hidden Game Objects")]
        private static void ShowHiddenGameObjectsWindow()
        {
            // get the window, show it, and hand it focus
            var local = LocalizationManager.Instance;
            GetWindow<HiddenGameObjectsWindow>(local.Get("HiddenGameObjects")).Show();
        }
    }
}