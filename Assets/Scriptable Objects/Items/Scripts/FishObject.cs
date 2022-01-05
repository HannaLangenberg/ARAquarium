using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Fish Object", menuName = "Inventory System/Items/Fish")]
    public class FishObject : ItemObject
    {
        public string biologicalName;
        public string species;
        public string length;
        public int minCount;
        public void Awake()
        {
            type = ItemType.Fish;
        }
    }
}