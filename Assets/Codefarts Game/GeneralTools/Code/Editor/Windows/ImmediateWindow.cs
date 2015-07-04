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
    using System.CodeDom.Compiler;
    using System.Reflection;

    using Microsoft.CSharp;

    using UnityEditor;

    using UnityEngine;

    /// <summary>
    /// Provides an immediate window where the user can input C# code and run it directly from within the unity editor.
    /// </summary>
    public class ImmediateWindow : EditorWindow
    {
        // script text
        private string scriptText = string.Empty;

        // cache of last method we compiled so repeat executions only incur a single compilation
        private MethodInfo lastScriptMethod;

        // position of scroll view
        private Vector2 scrollPosition;

        void ValidateScript()
        {
            // create and configure the code provider
            var codeProvider = new CSharpCodeProvider();
            var options = new CompilerParameters();
            options.GenerateInMemory = true;
            options.GenerateExecutable = false;

            // bring in system libraries
            options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Core.dll");

            // bring in Unity assemblies
            options.ReferencedAssemblies.Add(typeof(EditorWindow).Assembly.Location);
            options.ReferencedAssemblies.Add(typeof(Transform).Assembly.Location);

            // compile an assembly from our source code
            var result = codeProvider.CompileAssemblyFromSource(options, string.Format(scriptFormat, this.scriptText));

            // log any errors we got
            if (result.Errors.Count > 0)
            {
                foreach (CompilerError error in result.Errors)
                {
                    // the magic -11 on the line is to compensate for usings and class wrapper around the user script code.
                    // subtracting 11 from it will give the user the line numbers in their code.
                    Debug.LogError(string.Format("Immediate Compiler Error ({0}): {1}", error.Line - 11, error.ErrorText));
                }
                this.lastScriptMethod = null;
            }

                // otherwise use reflection to pull out our action method so we can invoke it
            else
            {
                var type = result.CompiledAssembly.GetType("ImmediateWindowCodeWrapper");
                this.lastScriptMethod = type.GetMethod("PerformAction", BindingFlags.Public | BindingFlags.Static);
            }
        }


        void HandleValidateAndExecuteButtons()
        {
            GUILayout.BeginHorizontal();

            // show the execute button
            if (GUILayout.Button("Validate"))
            {
                // if our script method needs compiling
                this.ValidateScript();
                if (this.lastScriptMethod != null) Debug.Log("No script errors! :D");
            }

            // show the execute button
            if (GUILayout.Button("Execute"))
            {
                // if we have a compiled method, invoke it
                this.ValidateScript();
                if (this.lastScriptMethod != null) this.lastScriptMethod.Invoke(null, null);
            }

            GUILayout.EndHorizontal();
        }

        void HandleSaveLoadButtons()
        {
            EditorGUILayout.BeginHorizontal();
            // show the execute button
            if (GUILayout.Button("Save"))
            {
                var fileName = EditorUtility.SaveFilePanel("Save Snippet", "", "", "txt");
                if (fileName.Length > 0) System.IO.File.WriteAllText(fileName, this.scriptText);

            }

            // show the execute button
            if (GUILayout.Button("Load"))
            {
                var fileName = EditorUtility.OpenFilePanel("Load Snippet", "", "txt");
                if (fileName.Length > 0)
                {
                    this.scriptText = System.IO.File.ReadAllText(fileName);
                    GUIUtility.keyboardControl = 0;
                    GUIUtility.hotControl = 0;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Called by unity to draw the window ui.
        /// </summary>
        void OnGUI()
        {
            // start the scroll view
            this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);

            // show the script field
            this.scriptText = EditorGUILayout.TextArea(this.scriptText);
            EditorGUILayout.EndScrollView();

            this.HandleValidateAndExecuteButtons();
            this.HandleSaveLoadButtons();
        }

        [MenuItem("Codefarts/General Utilities/Immediate")]
        public static void ShowImmediateWindow()
        {
            // get the window, show it, and hand it focus
            try
            {
                var window = GetWindow<ImmediateWindow>();
                window.title = "Immediate";
                window.Show();
                window.Focus();
                window.Repaint();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        // script we wrap around user entered code
        static readonly string scriptFormat = @"
                using UnityEngine;
                using UnityEditor;
                using System.Collections;
                using System.Collections.Generic;
                using System.Text;
                using System;
                public static class ImmediateWindowCodeWrapper
                {{
                    public static void PerformAction()
                    {{
                        // user code goes here
                        {0};
                    }}
                }}";
    }
}