using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    public class TankDetails : Details
    {
        public TankObject tankObject;

        public Text width, height, depth, volume;

        private void Start()
        {
            tankObject = (TankObject) itemObject;

            width.text = tankObject.width;
            height.text = tankObject.height;
            depth.text = tankObject.depth;
            volume.text = tankObject.volume;
        }
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
        public void SelectThisTank()
        {
            player.GetComponent<TankSelection>().SelectActiveSlot();
        }
    }
}