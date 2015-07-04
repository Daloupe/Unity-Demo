/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/

namespace Codefarts.GeneralTools.Editor.Utilities
{
    using System;
    using System.Reflection;

    using UnityEngine;

    public class ClipboardHelper
    {
        private static PropertyInfo systemCopyBufferProperty;
        private static PropertyInfo GetSystemCopyBufferProperty()
        {
            if (systemCopyBufferProperty == null)
            {
                var T = typeof(GUIUtility);
                systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
                if (systemCopyBufferProperty == null)
                {
                    throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
                }
            }

            return systemCopyBufferProperty;
        }

        public static string Clipboard
        {
            get
            {
                var P = GetSystemCopyBufferProperty();
                return (string)P.GetValue(null, null);
            }

            set
            {
                var P = GetSystemCopyBufferProperty();
                P.SetValue(null, value, null);
            }
        }
    }
}