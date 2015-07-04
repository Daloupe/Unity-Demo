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

    public class SceneViewDrawService
    {
        private static SceneViewDrawService service;

        private SceneViewArgs args = new SceneViewArgs();

        public event EventHandler<SceneViewArgs> OnSceneGui;

        public static SceneViewDrawService Instance
        {
            get
            {
                if (service == null)
                {
                    service = new SceneViewDrawService();
                    SceneView.onSceneGUIDelegate += service.OnGui;
                }

                return service;
            }
        }

        private void OnGui(SceneView sceneview)
        {
            if (sceneview == null)
            {
                return;
            }

            if (this.OnSceneGui != null)
            {
                this.args.View = sceneview;
                this.OnSceneGui(this, this.args);
            }
        }
    }
}