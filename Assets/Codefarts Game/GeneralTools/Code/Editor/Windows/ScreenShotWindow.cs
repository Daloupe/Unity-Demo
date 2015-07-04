namespace Codefarts.GeneralTools.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class ScreenShotWindow : EditorWindow
    {
        [MenuItem("Codefarts/General Utilities/Screeshots/Take Screen Shot")]
        public static void TakeScreenShot()
        {
            var file = AssetDatabase.GenerateUniqueAssetPath("Assets/Screenshot.png");
            Application.CaptureScreenshot(file);
            Debug.Log(string.Format("Saved screen shot: {0}", file));
        }
    }
}
