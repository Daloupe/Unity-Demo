/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor
{
    using Codefarts.CoreProjectCode.Settings;

    /// <summary>
    /// Provides various keys as global constant values for use with the settings system.
    /// </summary>
    public class GlobalConstants
    {
        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the border size used to determine
        /// how fast the panning speed is when scene view auto panning is enabled. 
        /// </summary>
        public const string SceneViewAutoPanSpeedKey = "GeneralTools.SceneViewAutoPan.Speed";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the border size used to determine
        /// when to start auto panning when scene view auto panning is enabled. 
        /// </summary>
        public const string SceneViewAutoPanSizeKey = "GeneralTools.SceneViewAutoPan.Size";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to whether scene view auto panning is enabled. 
        /// </summary>
        public const string SceneViewAutoPanEnabledKey = "GeneralTools.SceneViewAutoPan.Enabled";
      
        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the available shaders that will be listed. 
        /// </summary>
        public const string TileMaterialCreationShadersKey = "GeneralTools.TileMaterialCreation.Shaders";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the default state of the free form
        /// checkbox in the tile material creation window. 
        /// </summary>
        public const string TileMaterialCreationFreeformKey = "GeneralTools.TileMaterialCreation.Freeform";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the default state of the tile width field in the tile material creation window. 
        /// </summary>
        public const string TileMaterialCreationDefaultWidthKey = "GeneralTools.TileMaterialCreation.DefaultWidth";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the default state of the tile height field in the tile material creation window. 
        /// </summary>
        public const string TileMaterialCreationDefaultHeightKey = "GeneralTools.TileMaterialCreation.DefaultHeight";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to whether the default state of the "As List" check box is checked in the tile material creation window. 
        /// </summary>
        public const string TileMaterialCreationAsListKey = "GeneralTools.TileMaterialCreation.AsList";

        /// <summary>
        /// Provides a unique id key for storing and retrieving <see cref="IValues{TKey}"/> settings relating to the default color displayed in the tile material creation window. 
        /// </summary>
        public const string DefaultTileMaterialCreationColorKey = "GeneralTools.TileMaterialCreation.DefaultColor";

    }
}
