/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.Services
{
    using System;

    using UnityEditor;

    public class SceneViewArgs : EventArgs
    {
        public SceneView View { get; internal set; }
    }
}