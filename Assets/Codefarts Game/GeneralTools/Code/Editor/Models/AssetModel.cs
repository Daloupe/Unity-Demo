namespace Codefarts.GeneralTools.Editor.Models
{
    using System;

    using Codefarts.GeneralTools.Common;

    using UnityEngine;

    using Object = UnityEngine.Object;

    public class AssetModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public Object Reference { get; set; }

        public GameObject GameObject { get; set; }
        public Action<int, int, ITableModel<AssetModel>> GameObjectCallback { get; set; }

        public Action<int, int, ITableModel<AssetModel>> Callback { get; set; }
    }
}