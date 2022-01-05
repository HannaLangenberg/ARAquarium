using System.Linq;
using Display;
using Scriptable_Objects.Inventory.Scripts;
using UnityEngine;

namespace UI
{
    public class HandleStats : MonoBehaviour
    {
   
        /**
     * Function attached to Minus button on DetailsPanel. Forwards add operation to class intern function of
     * InventoryObject. Uses the returned InventorySlot to display the changes. Lastly refreshes UI.
     * Decision variable 1 → remove
     */
        public void Remove(InventorySlot inventorySlot, InventoryObject inventoryToDisplay)
        {
            if ((inventorySlot.item.tag.Equals("Discus") ||
                 inventorySlot.item.tag.Equals("Wetterschmerle")) &&
                !(inventorySlot.amount > 0))
            {
                EnableFish(inventorySlot, inventoryToDisplay);
            }
        }
        
        private void EnableFish(InventorySlot inventorySlot, InventoryObject inventoryToDisplay)
        {
            if (!inventoryToDisplay.Equals(GetComponent<Player>().fishInventory)) return;

            foreach (var fishInventorySlot in inventorySlot.item.disabledFishInventorySlots)
            {
                // slotHolder.transform.GetChild(fishInventorySlot.slotIndex).GetComponent<Image>().color = Color.white;
                GetComponent<Player>().fishInventory.container[fishInventorySlot.slotIndex].isActive = true;
            }
            
        }
        
        public void DisableFish(InventorySlot inventorySlot)
        {
            /*foreach (var fishInventorySlot in inventorySlot.item.disabledFishInventorySlots)
            {
                GetComponent<Player>().fishInventory.container[fishInventorySlot.slotIndex].isActive = false;
            }*/

            foreach (var fishInventorySlot in GetComponent<Player>().fishInventory.container)
            {
                if (inventorySlot.item.disabledFishInventorySlots.Contains(fishInventorySlot))
                {   
                    GetComponent<Player>().fishInventory.container[fishInventorySlot.slotIndex].isActive = false;
                }
                else
                {
                    GetComponent<Player>().fishInventory.container[fishInventorySlot.slotIndex].isActive = true;
                }
            }
        }
    }
}
