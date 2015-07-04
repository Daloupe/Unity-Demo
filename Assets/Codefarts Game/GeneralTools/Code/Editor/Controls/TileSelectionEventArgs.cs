/*
<copyright>
  Copyright (c) 2012 Codefarts
  All rights reserved.
  contact@codefarts.com
  http://www.codefarts.com
</copyright>
*/
namespace Codefarts.GeneralTools.Editor.Controls
{
    using System;
    using System.Collections.Generic;
    using Codefarts.GeneralTools.Common;

    public class TileSelectionEventArgs : EventArgs
    {
        public TileSelectionStatus Status { get; set; }

        public List<Point> TileLocations { get; set; }

        public Point Min { get; set; }
        public Point Max { get; set; }
    }
}