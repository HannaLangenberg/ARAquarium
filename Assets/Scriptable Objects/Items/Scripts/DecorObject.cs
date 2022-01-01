using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Decor Object", menuName = "Inventory System/Items/Decor")]
    public class DecorObject : ItemObject
    {
        public string width, height, depth;
        public void Awake()
        {
            type = ItemType.Decor;
        }
    }
}