using UnityEngine.UI;
using Scriptable_Objects.Items.Scripts;

namespace UI
{
    /// <summary>
    /// Class <c>TankDetails</c> inherits from Details. Stores a DecorObject, which is a specific TankObject, and
    /// additional serializable UI elements. 
    /// </summary>
    public class TankDetails : Details
    {
        public TankObject tankObject;

        public Text width, height, depth, volume;

        /// <summary>
        /// Whenever a details panel in the UI is opened or updated Start updates its UI elements to
        /// display the information stored in the TankObject. 
        /// </summary>
        private void Start()
        {
            tankObject = (TankObject) itemObject;

            width.text = tankObject.width;
            height.text = tankObject.height;
            depth.text = tankObject.depth;
            volume.text = tankObject.volume;
        }
        /// <summary>
        /// Called if the details panel needs to be refreshed or opened.
        /// Calls UpdateChanges in Details and Start in TankDetails to update common and additional UI.
        /// </summary>
        /// <param name="details"></param>
        public void UpdateChanges(Details details)
        {
            details.UpdateChanges();
            Start();
        }
        /// <summary>
        /// Sets the corresponding inventorySlot for the currently selected tank.
        /// </summary>
        public void SelectThisTank()
        {
            player.GetComponent<TankSelection>().SelectActiveSlot();
        }
    }
}