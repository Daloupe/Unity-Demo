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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Codefarts.GeneralTools.Editor.Utilities;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// The mesh to code window that allows converting a selected prefab with a mesh filter into C# code.
    /// </summary>
    public class MeshToCodeWindow : EditorWindow
    {
        /// <summary>
        /// Position of scroll view.
        /// </summary>
        private Vector2 scrollPosition;

        /// <summary>
        /// A reference to the selected prefab.
        /// </summary>
        private GameObject prefab;

        /// <summary>
        /// String that will hold the generated C# code.
        /// </summary>
        private string code;

        /// <summary>
        /// Holds the value of the Optimize check box.
        /// </summary>
        private bool optimize;

        /// <summary>
        /// Holds the checked state of the Round Vertexes check box.
        /// </summary>
        private bool roundVertexes;

        /// <summary>
        /// Holds the number of decimal places to round off the vertex values.
        /// </summary>
        private int vertexDecimals;

        /// <summary>
        /// Holds the checked state of the round normals check box.
        /// </summary>
        private bool roundNorms;

        /// <summary>
        /// Holds the number of decimal places to round off the normal values.
        /// </summary>
        private int normalDecimals;

        /// <summary>
        /// Provides a menu in the Unity menu system for showing this window.
        /// </summary>
        [MenuItem("Codefarts/General Utilities/Mesh To Code")]
        public static void ShowWindow()
        {
            // get the window, show it, and hand it focus
            try
            {
                var window = GetWindow<MeshToCodeWindow>();
                window.title = "Mesh To Code";
                window.Show();
                window.Focus();
                window.Repaint();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Draws the window controls.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
// ReSharper disable UnusedMember.Local
       private void OnGUI()
// ReSharper restore UnusedMember.Local
        {
            // start the scroll view  
            this.prefab = EditorGUILayout.ObjectField("Prefab", this.prefab, typeof(GameObject), true) as GameObject;

            // place spacing between controls
            GUILayout.Space(4);

            // draw a check box to determine whether to use optimization.
            this.optimize = GUILayout.Toggle(this.optimize, "Optimize");

            // place spacing between controls
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            this.roundVertexes = GUILayout.Toggle(this.roundVertexes, "Round vertices");
            if (this.roundVertexes)
            {
                this.vertexDecimals = EditorGUILayout.IntField("Decimal places", this.vertexDecimals);
                this.vertexDecimals = this.vertexDecimals < 0 ? 0 : this.vertexDecimals;
            }

            GUILayout.EndHorizontal();

            // place spacing between controls
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            this.roundNorms = GUILayout.Toggle(this.roundNorms, "Round normals");
            if (this.roundNorms)
            {
                this.normalDecimals = EditorGUILayout.IntField("Decimal places", this.normalDecimals);
                this.normalDecimals = this.normalDecimals < 0 ? 0 : this.normalDecimals;
            }

            GUILayout.EndHorizontal();

            // place spacing between controls
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate"))
            {
                this.GenerateCode();
            }

            // place spacing between controls
            GUILayout.Space(4);

            // add a copy to clipboard button
            if (GUILayout.Button("Copy to clipboard"))
            {
                ClipboardHelper.Clipboard = this.code;
            }

            GUILayout.EndHorizontal();

            // draw generated code
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
            this.code = GUILayout.TextArea(string.IsNullOrEmpty(this.code) ? string.Empty : this.code, int.MaxValue);
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Generates C# from the specified prefab.
        /// </summary>
        private void GenerateCode()
        {
            // if no prefab just return
            if (this.prefab == null)
            {
                this.code = "No prefab";
                return;
            }

            // check if a mesh filter is present
            var filter = this.prefab.GetComponent<MeshFilter>();
            if (filter == null)
            {
                this.code = "No mesh filter";
                return;
            }

            var tempMesh = new Mesh();
            tempMesh.vertices = filter.sharedMesh.vertices;
            tempMesh.normals = filter.sharedMesh.normals;
            tempMesh.uv = filter.sharedMesh.uv;
            tempMesh.triangles = filter.sharedMesh.triangles;

            // check if user wants to optimize first
            if (this.optimize)
            {
                MeshUtility.Optimize(tempMesh);
            }

            this.code = "Mesh Generate()\r\n";
            this.code += "{\r\n";
            this.code += "    var mesh = new Mesh();\r\n";
            this.code += "\r\n";

            // vertices
            this.code += "    var vertices = new Vector3[]\r\n";
            this.code += "    {\r\n";

            this.code += string.Join(string.Empty, tempMesh.vertices.Select(vector => this.GetVector3String(vector, this.roundVertexes, this.vertexDecimals)).ToArray());

            this.code += "    };\r\n";
            this.code += "\r\n";

            // normals
            this.code += "    var normals = new Vector3[]\r\n";
            this.code += "    {\r\n";

            this.code += string.Join(string.Empty, tempMesh.normals.Select(vector => this.GetVector3String(vector, this.roundNorms, this.normalDecimals)).ToArray());

            this.code += "    };\r\n";
            this.code += "\r\n";

            // uv cords
            this.code += "    var uv = new Vector2[]\r\n";
            this.code += "    {\r\n";

            this.code += string.Join(string.Empty, tempMesh.uv.Select(vector => string.Format("        new Vector2({0}f, {1}f),\r\n", vector.x, vector.y)).ToArray());

            this.code += "    };\r\n";
            this.code += "\r\n";

            // triangles        
            this.code += "    var triangles = new int[]\r\n";
            this.code += "    {\r\n";

            var triangles = tempMesh.triangles;
            for (int i = 0; i < triangles.Length; i += 3)
            {
                this.code += string.Format("        {0}, {1}, {2},\r\n", triangles[i], triangles[i + 1], triangles[i + 2]);
            }

            this.code += "    };\r\n";
            this.code += "\r\n";

            this.code += "    mesh.vertices = vertices;\r\n";
            this.code += "    mesh.normals = normals;\r\n";
            this.code += "    mesh.uv = uv;\r\n";
            this.code += "    mesh.triangles = triangles;\r\n";
            this.code += "    return mesh;\r\n";
            this.code += "}\r\n";
        }

        /// <summary>
        /// Gets the rounded values of a <see cref="Vector3"/> type as a C# encoded string.
        /// </summary>
        /// <param name="vector">The source vector to be converted.</param>
        /// <param name="round">If true the values from <see cref="vector"/> will be rounded.</param>
        /// <param name="decimals">The number of decimal places to round.</param>
        /// <returns>Returns the rounded values of the <see cref="vector"/> parameter as a C# encoded string.</returns>
        private string GetVector3String(Vector3 vector, bool round, int decimals)
        {
            if (round)
            {
                return string.Format(
                       "        new Vector3({0}f, {1}f, {2}f),\r\n",
                       Math.Round(vector.x, decimals),
                       Math.Round(vector.y, decimals),
                       Math.Round(vector.z, decimals));
            }

            return string.Format("        new Vector3({0}f, {1}f, {2}f),\r\n", vector.x, vector.y, vector.z);
        }
    }
}