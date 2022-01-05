using Scriptable_Objects.Inventory.Scripts;
using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    public enum ItemType
    {
        Tank,
        Fish,
        Plant,
        Decor,
        Default
    }

    public abstract class ItemObject : ScriptableObject
    {
        public string tag;
        public new string name;
        public Sprite thumbnail;
        public GameObject prefab;
        public ItemType type;
        [TextArea(5, 10)] public string furtherInformation;
        public FishObject[] disabledFish;
        public InventorySlot[] disabledFishInventorySlots;
    }
}
