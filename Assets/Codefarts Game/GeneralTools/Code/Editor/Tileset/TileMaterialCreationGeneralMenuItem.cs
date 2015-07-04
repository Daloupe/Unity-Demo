/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.TileSets
{
    using Codefarts.GeneralTools.Common;
    using Codefarts.GeneralTools.Editor;

    using Codefarts.Localization;

    /// <summary>
    /// Provides methods for drawing tile material creation settings.
    /// </summary>
    public class TileMaterialCreationGeneralMenuItem 
    {
        public static  void Draw()
        {
            var local = LocalizationManager.Instance;
           
            SettingHelpers.DrawSettingsColorPicker(GlobalConstants.DefaultTileMaterialCreationColorKey, local.Get("SETT_DefaultTileMaterialCreationColor"), Color.White);
            SettingHelpers.DrawSettingsCheckBox(GlobalConstants.TileMaterialCreationAsListKey, local.Get("SETT_TileMaterialCreationAsList"), false);
            SettingHelpers.DrawSettingsIntField (GlobalConstants.TileMaterialCreationDefaultWidthKey, local.Get("SETT_TileMaterialCreationDefaultTileWidth"), 32);
            SettingHelpers.DrawSettingsIntField(GlobalConstants.TileMaterialCreationDefaultHeightKey, local.Get("SETT_TileMaterialCreationDefaultTileHeight"), 32);
            SettingHelpers.DrawSettingsTextBox(GlobalConstants.TileMaterialCreationShadersKey, local.Get("SETT_TileMaterialCreationShaders"), "Diffuse\r\nTransparent/Diffuse\r\nTransparent/Cutout/Diffuse\r\nTransparent/Cutout/Soft Edge Unlit");
        }
    }
}