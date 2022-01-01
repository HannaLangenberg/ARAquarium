using System;
using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Plant Object", menuName = "Inventory System/Items/Plant")]
    public class PlantObject : ItemObject
    {
        public string biologicalName;
        public string species;
        public string height;
        public void Awake()
        {
            type = ItemType.Plant;
        }
    }
}