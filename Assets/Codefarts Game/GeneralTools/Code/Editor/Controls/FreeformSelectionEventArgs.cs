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

    using UnityEngine;

    public class FreeformSelectionEventArgs : EventArgs
    {
        public TileSelectionStatus Status { get; set; }

        public Rect SelectionRectangle { get; set; }
    }
}