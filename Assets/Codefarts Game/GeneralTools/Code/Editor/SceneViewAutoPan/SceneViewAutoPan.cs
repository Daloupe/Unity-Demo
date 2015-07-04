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
      using Codefarts.CoreProjectCode;
    using Codefarts.CoreProjectCode.Services;
      using Codefarts.GeneralTools.Editor.SceneViewAutoPan;
      using Codefarts.Localization;

      using UnityEditor;

    using UnityEngine;

    [InitializeOnLoad]
    public class SceneViewAutoPan
    {
        private static SceneViewAutoPan service;

      //    private DateTime lastUpdate;

        public static SceneViewAutoPan Instance
        {
            get
            {
                if (service == null)
                {
                    service = new SceneViewAutoPan();
                    SceneViewDrawService.Instance.OnSceneGui += service.OnGui;
                }

                return service;
            }
        }

        static SceneViewAutoPan()
        {
            EditorCallbackService.Instance.Register(
                () =>
                {
                    var manager = UnityPreferencesManager.Instance;
                    var local = LocalizationManager.Instance;

                    // include scene view auto panning
                    manager.Register(local.Get("SETT_SceneViewAutoPan"), SceneViewAutoPanGeneralMenuItem.Draw);
                });
        }

        /// <summary>
        /// Gets or sets whether the scene view auto panning service is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        public int Size { get; set; }

        private void OnGui(object sender, SceneViewArgs e)
        {
            if (!this.Enabled)
            {
                return;
            }

            // check if view parameter or mouse over view is null
            if (e.View == null || SceneView.mouseOverWindow == null)
            {
                return;
            }

            if (e.View.GetInstanceID() != SceneView.mouseOverWindow.GetInstanceID())
            {
                return;
            }


            var current = Event.current;

            if (current.isKey && current.keyCode == KeyCode.F)
            {
                return;
            }

            var mpos = current.mousePosition;
            var min = e.View.camera.ViewportToScreenPoint(Vector3.zero);
            var max = e.View.camera.ViewportToScreenPoint(Vector3.one);

            //  var worldPosCenter = sceneView.camera.ViewportToWorldPoint(Vector3.one / 2);
            //var worldPosA = sceneView.camera.ScreenToViewportPoint(new Vector3(mpos.x, mpos.y, 0));
            //worldPosA = sceneView.camera.ViewportToWorldPoint(worldPosA);


            var pos = Vector3.zero;
            if (mpos.x < min.x + this.Size)
            {
                pos += -e.View.camera.transform.right;
            }

            if (mpos.x > max.x - this.Size)
            {
                pos += e.View.camera.transform.right;
            }

            if (mpos.y < min.y + this.Size)
            {
                pos += e.View.camera.transform.up;
            }

            if (mpos.y > max.y - this.Size)
            {
                pos += -e.View.camera.transform.up;
            }

            // var delta = DateTime.Now - this.lastUpdate;
           // this.lastUpdate = DateTime.Now;
            if (pos == Vector3.zero)
            {
                return;
            }

            //  pos = pos.normalized * ((float)delta.TotalSeconds * this.Speed);
            pos = pos.normalized * (Time.fixedDeltaTime * this.Speed);

            e.View.LookAt(e.View.pivot + pos, e.View.camera.transform.rotation, e.View.size);
        }

        public float Speed { get; set; }
    }
}
