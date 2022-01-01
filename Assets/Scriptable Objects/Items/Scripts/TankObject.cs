using UnityEngine;

namespace Scriptable_Objects.Items.Scripts
{
    [CreateAssetMenu(fileName = "New Tank Object", menuName = "Inventory System/Items/Tank")]
    public class TankObject : ItemObject
    {
        public string width, height, depth;
        public string volume;

        [Header("--Not For Display--")]
        public float decorScaleMin;
        public float decorScaleMax;
        public float plantScaleMax;
        public float maxSpawnRadius;

        public void Awake()
        {
            type = ItemType.Tank;
        }
    }
}