/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.SceneViewAutoPan
{
    using Codefarts.CoreProjectCode.Settings;
    using Codefarts.GeneralTools.Editor;

    using Codefarts.Localization;

    /// <summary>
    /// Provides method9s0 for drawing scene view auto pan settings.
    /// </summary>
    public class SceneViewAutoPanGeneralMenuItem
    {
        public static void Draw()
        {
            var local = LocalizationManager.Instance;
            var settings = SettingsManager.Instance;

            SettingHelpers.DrawSettingsCheckBox(GlobalConstants.SceneViewAutoPanEnabledKey, local.Get("SETT_SceneViewAutoPanEnabled"), false,
               () =>
               {
                   // setup auto panning service
                   Services.SceneViewAutoPan.Instance.Enabled = settings.GetSetting(GlobalConstants.SceneViewAutoPanEnabledKey, false);
               });

            SettingHelpers.DrawSettingsIntField(GlobalConstants.SceneViewAutoPanSizeKey, local.Get("SETT_SceneViewAutoPanSize"), 64, 1, 1000,
            () =>
            {
                // setup auto panning service
                Services.SceneViewAutoPan.Instance.Size = settings.GetSetting(GlobalConstants.SceneViewAutoPanSizeKey, 64);
            });

           SettingHelpers.DrawSettingsFloatField (GlobalConstants.SceneViewAutoPanSpeedKey, local.Get("SETT_SceneViewAutoPanSpeed"), 1, 0, 1000,
           ()=>
             {
                    // setup auto panning service
                 Services.SceneViewAutoPan.Instance.Speed = settings.GetSetting(GlobalConstants.SceneViewAutoPanSpeedKey, 1);
                });

            Services.SceneViewAutoPan.Instance.Enabled = settings.GetSetting(GlobalConstants.SceneViewAutoPanEnabledKey, false);
            Services.SceneViewAutoPan.Instance.Size = settings.GetSetting(GlobalConstants.SceneViewAutoPanSizeKey, 64);
            Services.SceneViewAutoPan.Instance.Speed = settings.GetSetting(GlobalConstants.SceneViewAutoPanSpeedKey, 1);
        }
    }
}